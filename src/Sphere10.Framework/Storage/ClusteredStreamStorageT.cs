using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace Sphere10.Framework {

	/// <summary>
	/// An <see cref="IStreamStorageT{TStreamListing}"/> implementation which interlaces component streams together over a single logical stream using a clustering approach similar to how physical disk
	/// drives store data. This component can be used to store multiple streams over a single file, all of whom can be dynamically sized.  The implementationis optimized for arbitrarily large data
	/// scenarios without space/time/memory complexity issues and no load-time required. It is suitable as a general-purpose file-format for storing an application static and/or dynamic data.
	/// </summary>
	/// <typeparam name="TStreamListing">Type of BLOB listing (customizable)</typeparam>
	/// <typeparam name="TStreamContainerHeader">Type of BLOB header (customizable)</typeparam>
	/// <remarks>
	/// [HEADER] Version: 1, Cluster Size: 32, Total Clusters: 10, Listings: 5
	/// [Listings]
	///   0: [StreamListing] Size: 60, Start Cluster: 3
	///   1: [StreamListing] Size: 88, Start Cluster: 7
	///   2: [StreamListing] Size: 27, Start Cluster: 2
	///   3: [StreamListing] Size: 43, Start Cluster: 1
	///   4: [StreamListing] Size: 0, Start Cluster: -1
	/// [Clusters]
	///   0: [Cluster] Traits: First, Listing, Prev: -1, Next: 6, Data: 030000003c0000000700000058000000020000001b000000010000002b000000
	///   1: [Cluster] Traits: First, Data, Prev: 3, Next: 5, Data: 894538851b6655bb8d8a4b4517eaab2b22ada63e6e0000000000000000000000
	///   2: [Cluster] Traits: First, Data, Prev: 2, Next: -1, Data: 1e07b1f66b3a237ed9f438ec26093ca50dd05b798baa7de25f093f0000000000
	///   3: [Cluster] Traits: First, Data, Prev: 0, Next: 9, Data: ce178efbff3e3177069101b78453de5ca2d1a7d72c958485306fb400e0efc1f5
	///   4: [Cluster] Traits: Data, Prev: 8, Next: -1, Data: a3058b9856aaf271ab21153c040a05c15042abbf000000000000000000000000
	///   5: [Cluster] Traits: Data, Prev: 1, Next: -1, Data: 0000000000000000000000000000000000000000000000000000000000000000
	///   6: [Cluster] Traits: Listing, Prev: 0, Next: -1, Data: ffffffff00000000000000000000000000000000000000000000000000000000
	///   7: [Cluster] Traits: First, Data, Prev: 1, Next: 8, Data: 5aa2c04b9554fbe9425c2d52aa135ed8107bf9edbf44848326eb92cc9434b828
	///   8: [Cluster] Traits: Data, Prev: 7, Next: 4, Data: c612bcb3e59fd0d7d88240797e649b5020d5090682c0f3151e3c24a9c12e540d
	///   9: [Cluster] Traits: Data, Prev: 3, Next: -1, Data: 594ebf3d9241c837ffa3dea9ab0e550516ad18ed0f7b9c000000000000000000
	///
	///  Notes:
	///  - Header is fixed 256b, and can be expanded to include other data (passwords, merkle roots, etc)
	///  - Clusters are bi-directionally linked, to allow dynamic re-sizing on the fly 
	///  - Listings contain the meta-data of all the streams and the entire listings stream is also serialized over clusters.
	///  - Cluster traits distinguish listing clusters from stream clusters. 
	///  - Cluster 0, when allocated, is always the first listing cluster.
	///  - Listings always link to the (First | Data) cluster of their stream.
	///  - Clusters with traits (First | Data) re-purpose the Prev field to denote the listing.
	/// </remarks>
	public class ClusteredStreamStorage<TStreamContainerHeader, TStreamListing> : IStreamStorageT<TStreamListing>
		where TStreamContainerHeader : ClusteredStreamStorageHeader, new()
		where TStreamListing : IStreamListing, new() {
		private readonly Stream _stream;
		private readonly Endianness _endianness;
		private readonly TStreamContainerHeader _header;
		private readonly StreamPagedList<Cluster> _clusters;
		private readonly IItemSerializer<TStreamListing> _listingSerializer;
		private readonly FragmentProvider _listingsFragmentProvider;
		private readonly PreAllocatedList<TStreamListing> _listings;
		private int? _openListing;
		private FragmentProvider _openFragmentProvider;
		private Stream _openStream;
		private readonly object _lock;
		private int _version;

		public ClusteredStreamStorage(Stream rootStream, int clusterSize, IItemSerializer<TStreamListing> listingSerializer, Endianness endianness = Endianness.LittleEndian, ClusteredStreamCachePolicy listingsCachePolicy = ClusteredStreamCachePolicy.None) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.ArgumentInRange(clusterSize, 1, int.MaxValue, nameof(clusterSize));
			Guard.ArgumentNotNull(listingSerializer, nameof(listingSerializer));
			Guard.Argument(listingSerializer.IsStaticSize, nameof(listingSerializer), "Listings must be constant sized");
			_stream = rootStream;
			_endianness = endianness;
			_listingSerializer = listingSerializer;
			var clusterSerializer = new ClusterSerializer(clusterSize);
			_header = new TStreamContainerHeader();
			if (_stream.Length > 0) {
				_header.AttachTo(_stream, _endianness);
				CheckHeaderDataIntegrity((int)rootStream.Length, _header, clusterSerializer, _listingSerializer);
			} else {
				_header.CreateIn(1, _stream, clusterSize, _endianness);
			}
			Guard.Argument(ClusterSize == clusterSize, nameof(rootStream), $"Inconsistent cluster sizes (stream header had '{ClusterSize}')");
			AllListingsSize = _header.Listings * _listingSerializer.StaticSize;
			_clusters = new StreamPagedList<Cluster>(
				clusterSerializer,
				new NonClosingStream(new BoundedStream(rootStream, ClusteredStreamStorageHeader.ByteLength, long.MaxValue) { UseRelativeOffset = true, AllowResize = true }),
				_endianness
			) { IncludeListHeader = false };
			_listingsFragmentProvider = new FragmentProvider(this, listingsCachePolicy);
			_listings = new PreAllocatedList<TStreamListing>(
				new StreamPagedList<TStreamListing>(
					_listingSerializer,
						new FragmentedStream(_listingsFragmentProvider, _header.Listings * listingSerializer.StaticSize),
						_endianness
					) { IncludeListHeader = false },
				_header.Listings,
				PreAllocationPolicy.MinimumRequired,
				0
			);
			ClusterEnvelopeSize = clusterSerializer.StaticSize - ClusterSize;
			_lock = new object();
			_openFragmentProvider = null;
			_openStream = null;
			_openListing = null;
			_version = 0;
			ZeroClusterBytes = Tools.Array.Gen<byte>(clusterSize, 0);
			IntegrityChecks = true;
		}

		public int Count => _header.Listings;

		public ClusteredStreamCachePolicy DefaultStreamPolicy { get; set; } = ClusteredStreamCachePolicy.None;

		public IReadOnlyList<TStreamListing> Listings => _listings;

		public bool IntegrityChecks { get; set; }

		internal IReadOnlyList<Cluster> Clusters => _clusters;

		internal int ClusterSize => _header.ClusterSize;

		internal int ClusterEnvelopeSize { get; }

		internal int AllListingsSize { get; private set; }

		private ReadOnlyMemory<byte> ZeroClusterBytes { get; }

		#region Streams

		public byte[] ReadAll(int index) => Open(index).ReadAllAndDispose();

		public void AddBytes(ReadOnlySpan<byte> bytes) {
			using var stream = Add();
			stream.Write(bytes);
		}

		public void UpdateBytes(int index, ReadOnlySpan<byte> bytes) {
			using var stream = Open(index);
			stream.SetLength(0);
			stream.Write(bytes);
		}

		public void AppendBytes(int index, ReadOnlySpan<byte> bytes) {
			using var stream = Open(index);
			stream.Seek(stream.Length, SeekOrigin.Current);
			stream.Write(bytes);
		}

		public Stream Add() => Add(DefaultStreamPolicy);

		public Stream Add(ClusteredStreamCachePolicy cachePolicy) {
			lock (_lock) {
				CheckNotOpened();
				var listing = AddListing(out var index, CreateListing());  // the first listing add will allocate cluster 0 for the listings stream
				listing.Size = 0;
				listing.StartCluster = -1; // when opened and written, the first cluster will be allocated
				UpdateListing(index, listing);
				return Open(index, cachePolicy);
			}
		}

		public Stream Open(int index) => Open(index, DefaultStreamPolicy);

		public Stream Open(int index, ClusteredStreamCachePolicy cachePolicy) {
			CheckListingIndex(index);
			lock (_lock) {
				CheckNotOpened();
				_openFragmentProvider = new FragmentProvider(this, index, cachePolicy);
				_openStream = new FragmentedStream(_openFragmentProvider).OnDispose(() => { _openStream = null; _openListing = null; _openFragmentProvider = null; });
				_openListing = index;
				return _openStream;
			}
		}

		public void Remove(int index) {
			CheckListingIndex(index);
			lock (_lock) {
				CheckNotOpened();
				var listing = GetListing(index);
				if (listing.StartCluster != -1)
					RemoveClusterChain(listing.StartCluster);
				RemoveListing(index); // listing must be removed last, in case it deletes genesis cluster
			}
		}

		public Stream Insert(int index) => Insert(index, DefaultStreamPolicy);

		public Stream Insert(int index, ClusteredStreamCachePolicy cachePolicy) {
			CheckListingIndex(index);
			lock (_lock) {
				CheckNotOpened();
				InsertListing(index, CreateListing());
				return Open(index, cachePolicy);
			}
		}

		public void Swap(int first, int second) {
			CheckListingIndex(first);
			CheckListingIndex(second);
			
			if (first == second)
				return;

			lock(_lock) {
				CheckNotOpened();

				// Get listings
				var firstListing = GetListing(first);
				var secondListing = GetListing(second);

				// Swap listings
				UpdateListing(first, secondListing);
				UpdateListing(second, firstListing);
				
				// Update genesis-to-listing links in genesis clusters (if applicable)
				if (firstListing.StartCluster != -1) {
					FastWriteClusterPrev(firstListing.StartCluster, second);
				}

				if (secondListing.StartCluster != -1) {
					FastWriteClusterPrev(secondListing.StartCluster, first);
				}
			}
		}

		public void Clear(int index) {
			UpdateBytes(index, Array.Empty<byte>());
		}

		public void ClearAll() {
			lock (_lock) {
				CheckNotOpened();
				_header.Listings = 0;
				_header.TotalClusters = 0;
				_clusters.Clear();
				_listingsFragmentProvider.Reset();
			}
		}

		public void Optimize() {
			throw new NotImplementedException();
		}

		public override string ToString() => _header.ToString();

		public string ToStringFullContents() {
			var stringBuilder = new FastStringBuilder();
			stringBuilder.AppendLine(this.ToString());
			stringBuilder.AppendLine("Listings:");
			for (var i = 0; i < _listings.Count; i++) {
				var listing = GetListing(i);
				stringBuilder.AppendLine($"\t{i}: {listing}");
			}
			stringBuilder.AppendLine("Clusters:");
			for (var i = 0; i < _clusters.Count; i++) {
				var cluster = GetCluster(i);
				stringBuilder.AppendLine($"\t{i}: {cluster}");
			}

			return stringBuilder.ToString();
		}

		#endregion

		#region Listings

		private TStreamListing CreateListing() => new() { Size = 0, StartCluster = -1 };

		private TStreamListing GetListing(int index) {
			var listing = _listings.Read(index);
			if (IntegrityChecks)
				CheckListingIntegrity(index, listing);
			return listing;
		}

		private TStreamListing AddListing(out int index, TStreamListing listing) {
			_listings.Add(listing);
			_header.Listings++;
			index = _listings.Count - 1;
			return listing;
		}

		private void UpdateListing(int index, TStreamListing listing) {
			_listings.Update(index, listing);
			if (_openListing.HasValue && _openListing.Value == index)
				_openFragmentProvider.Reset();
		}

		private void InsertListing(int index, TStreamListing listing) {
			Debug.Assert(_openStream == null);
			// Update genesis clusters 
			for (var i = index + 1; i < _listings.Count; i++) {
				var shiftedListing = GetListing(i);
				if (shiftedListing.StartCluster != -1)
					FastWriteClusterPrev(shiftedListing.StartCluster, i + 1);
			}
			_listings.Insert(index, listing);
			_header.Listings++;
		}

		private void RemoveListing(int index) {
			for (var i = index + 1; i < _listings.Count; i++) {
				var higherListing = GetListing(i);
				if (higherListing.StartCluster != -1)
					FastWriteClusterPrev(higherListing.StartCluster, i - 1);
			}
			_listings.RemoveAt(index);
			_header.Listings--;
		}

		#endregion

		#region Clusters

		private int AllocateStartCluster(int listing, ClusterDataType clusterDataType) {
			var cluster = new Cluster {
				Traits = ClusterTraits.First | clusterDataType switch { ClusterDataType.Listing => ClusterTraits.Listing, ClusterDataType.Stream => ClusterTraits.Data },
				Prev = listing,
				Next = -1,
				Data = ZeroClusterBytes.ToArray(),
			};
			_clusters.Add(cluster);
			if (clusterDataType == ClusterDataType.Listing)
				_listingsFragmentProvider.Reset();

			_header.TotalClusters++;
			return _clusters.Count - 1;
		}

		private int AllocateNextCluster(int prevCluster, ClusterDataType clusterDataType) {
			var cluster = new Cluster {
				Traits = clusterDataType switch { ClusterDataType.Listing => ClusterTraits.Listing, ClusterDataType.Stream => ClusterTraits.Data },
				Prev = prevCluster,
				Next = -1,
				Data = ZeroClusterBytes.ToArray(),
			};
			_clusters.Add(cluster);
			FastWriteClusterNext(prevCluster, _clusters.Count - 1);
			_header.TotalClusters++;
			return _clusters.Count - 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Cluster GetCluster(int clusterIndex) {
			var cluster = _clusters[clusterIndex];
			if (IntegrityChecks) {
				CheckClusterTraits(clusterIndex, cluster.Traits);
				CheckClusterIndex(cluster.Next);
				if (!cluster.Traits.HasFlag(ClusterTraits.First | ClusterTraits.Data))
					CheckClusterIndex(cluster.Prev);
			}
			return cluster;
		}

		private void UpdateCluster(int clusterIndex, Cluster cluster) => _clusters[clusterIndex] = cluster;

		private void RemoveCluster(int clusterIndex) {
			var next = FastReadClusterNext(clusterIndex);
			Guard.Ensure(next == -1, $"Can only remove a cluster from end of cluster-linked chain (clusterIndex = {clusterIndex})");
			var traits = FastReadClusterTraits(clusterIndex);
			var prevPointsToListing = traits.HasFlag(ClusterTraits.First) && traits.HasFlag(ClusterTraits.Data);
			if (!prevPointsToListing) {
				var prev = FastReadClusterPrev(clusterIndex);
				if (prev >= _header.TotalClusters)
					throw new CorruptDataException(_header, clusterIndex, $"Prev index pointed to non-existent cluster {clusterIndex}");
				if (prev != -1)
					FastWriteClusterNext(prev, -1);  // prev.next points to deleted cluster, so make it point to nothing
			}
			MigrateTipClusterTo(clusterIndex);
			var tipClusterIX = _clusters.Count - 1;
			InvalidateCachedCluster(tipClusterIX);
			_clusters.RemoveAt(tipClusterIX);
			_header.TotalClusters--;
		}

		private void RemoveClusterChain(int startCluster) {
			var clustersRemoved = 0;
			var cluster = startCluster;
			Guard.Ensure(FastReadClusterTraits(cluster).HasFlag(ClusterTraits.First), "Cluster not a start cluster");

			while (cluster != -1) {
				var nextCluster = FastReadClusterNext(cluster);
				CheckClusterIndex(nextCluster, "Corrupt data");
				var tipIX = _clusters.Count - clustersRemoved - 1;
				// if next is the tip about to be migrated to cluster, the next is cluster
				if (tipIX == nextCluster)
					nextCluster = cluster;
				MigrateTipClusterTo(tipIX, cluster);
				InvalidateCachedCluster(tipIX);
				InvalidateCachedCluster(cluster);
				clustersRemoved++;
				cluster = nextCluster;
			}
			_clusters.RemoveRange(_clusters.Count - clustersRemoved, clustersRemoved);
			_header.TotalClusters -= clustersRemoved;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ClusterTraits FastReadClusterTraits(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, 0, 1, out var bytes);
			var traits = (ClusterTraits)bytes[0];
			if (IntegrityChecks)
				CheckClusterTraits(clusterIndex, traits);
			return traits;
		}

		internal void FastWriteClusterTraits(int clusterIndex, ClusterTraits traits) {
			_clusters.WriteItemBytes(clusterIndex, 0, new byte[(byte)traits]);
		}

		internal int FastReadClusterPrev(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, sizeof(byte), 4, out var bytes);
			var prevCluster = _clusters.Reader.BitConverter.ToInt32(bytes);
			if (IntegrityChecks)
				CheckLinkedCluster(clusterIndex, prevCluster);
			return prevCluster;
		}

		internal void FastWriteClusterPrev(int clusterIndex, int prev) {
			var bytes = _clusters.Writer.BitConverter.GetBytes(prev);
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte), bytes);
		}

		private int FastReadClusterNext(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, sizeof(byte) + sizeof(int), 4, out var bytes);
			var nextValue = _clusters.Reader.BitConverter.ToInt32(bytes);
			if (IntegrityChecks)
			CheckLinkedCluster(clusterIndex, nextValue);
			return nextValue;
		}

		internal void FastWriteClusterNext(int clusterIndex, int next) {
			var bytes = _clusters.Writer.BitConverter.GetBytes(next);
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte) + sizeof(int), bytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void FastWriteClusterData(int clusterIndex, int offset, ReadOnlySpan<byte> data) {
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte) + sizeof(int) + sizeof(int) + offset, data);
		}

		#endregion

		#region Aux methods

		private void CheckHeaderDataIntegrity(int rootStreamLength, ClusteredStreamStorageHeader header, IItemSerializer<Cluster> clusterSerializer, IItemSerializer<TStreamListing> listingSerializer) {
			var clusterEnvelopeSize = clusterSerializer.StaticSize - header.ClusterSize;
			var listingClusters = (int)Math.Ceiling(header.Listings*listingSerializer.StaticSize / (float)header.ClusterSize); 
			if (header.TotalClusters < listingClusters)
				throw new CorruptDataException($"Inconsistency in {nameof(ClusteredStreamStorageHeader.TotalClusters)}/{nameof(ClusteredStreamStorageHeader.Listings)}");
			var minStreamSize = header.TotalClusters * (header.ClusterSize + clusterEnvelopeSize) + ClusteredStreamStorageHeader.ByteLength;
			if (rootStreamLength < minStreamSize)
				throw new CorruptDataException($"Stream too small (header gives minimum size {minStreamSize} but was {rootStreamLength})");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckListingIndex(int index, string msg = null)
			=> Guard.Argument(0 <= index && index < _listings.Count, nameof(index), msg ?? "Index out of bounds");

		private void CheckListingIntegrity(int index, TStreamListing listing) {
			if (listing.Size == 0) {
				if (listing.StartCluster != -1)
					throw new CorruptDataException(_header, $"Empty listing {index} should have start cluster -1 but was {listing.StartCluster}");
			} else if (!(0 <= listing.StartCluster && listing.StartCluster < _header.TotalClusters))
				throw new CorruptDataException(_header, $"Listing {index} pointed to to non-existent cluster {listing.StartCluster}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckClusterIndex(int index, string msg = null)
			=> Guard.Argument(index == -1 || (0 <= index && index < _clusters.Count), nameof(index), msg ?? "Cluster index out of bounds (or not -1)");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckLinkedCluster(int sourceCluster, int linkedCluster, string msg = null) {
			Guard.Argument(sourceCluster >= 0, nameof(sourceCluster), "Must be greater than or equal to 0");

			if (linkedCluster == -1)
				return;

			if (sourceCluster == linkedCluster)
				throw new CorruptDataException(_header, sourceCluster, $"Cluster links to itself {sourceCluster}");

			if (!(0 <= linkedCluster && linkedCluster < _header.TotalClusters))
				throw new CorruptDataException(_header, sourceCluster, msg ?? $"Cluster {sourceCluster} pointed to non-existent cluster {linkedCluster}");
		}

		private void CheckClusterTraits(int cluster, ClusterTraits traits) {
			var bTraits = (byte)traits;
			if (bTraits == 0 || bTraits > 7 || ((traits & ClusterTraits.Data) > 0 && (traits & ClusterTraits.Listing) > 0))
				throw new CorruptDataException(_header, cluster, "Invalid traits");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckNotOpened() {
			if (_openStream != null)
				throw new InvalidOperationException("A stream is already opened");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MigrateTipClusterTo(int to) => MigrateTipClusterTo(_clusters.Count - 1, to);

		private void MigrateTipClusterTo(int tipIndex, int to) {
			// Moved the right-most cluster into 'to' so that right-most can be deleted (since 'to' is now available).
			Guard.ArgumentInRange(to, 0, _clusters.Count - 1, nameof(to));
			Guard.Argument(to <= tipIndex, nameof(to), $"Cannot be greater than {nameof(tipIndex)}");

			// If migrating to self, return
			if (to == tipIndex)
				return;

			var tipCluster = GetCluster(tipIndex);
			var toCluster = GetCluster(to);
			toCluster.Traits = tipCluster.Traits;
			toCluster.Prev = tipCluster.Prev;
			toCluster.Next = tipCluster.Next;
			tipCluster.Data.AsSpan().CopyTo(toCluster.Data);
			if (tipCluster.Traits.HasFlag(ClusterTraits.First)) {
				var listingIndex = tipCluster.Prev; // convention, Prev points to Listing index in first cluster of BLOB
				if (listingIndex < _listings.Count) {
					var updatedListing = _listings[listingIndex];
					updatedListing.StartCluster = to;
					UpdateListing(listingIndex, updatedListing);
				}
			} else {
				// Update tip's previous cluster to point to new location of tip
				FastWriteClusterNext(tipCluster.Prev, to);
			}
			if (tipCluster.Next != -1)
				FastWriteClusterPrev(tipCluster.Next, to);

			_clusters.Update(to, toCluster);
		}

		private void InvalidateCachedCluster(int cluster) {
			_listingsFragmentProvider.InvalidateCluster(cluster);
			if (_openFragmentProvider != null) 
				_openFragmentProvider.InvalidateCluster(cluster);
		}

		#endregion

		#region Inner Types

		internal class FragmentProvider : IStreamFragmentProvider {
			private readonly ClusteredStreamStorage<TStreamContainerHeader, TStreamListing> _parent;
			private readonly FragmentCache _fragmentCache;
			private readonly ClusterDataType _clusterDataType;
			private int _currentFragment;
			private int _currentCluster;
			private readonly int _listingIndex;
			private TStreamListing _listing;
			

			public FragmentProvider(ClusteredStreamStorage<TStreamContainerHeader, TStreamListing> parent, ClusteredStreamCachePolicy cachePolicy)
				: this(parent, ClusterDataType.Listing, -1, cachePolicy) {
			}

			public FragmentProvider(ClusteredStreamStorage<TStreamContainerHeader, TStreamListing> parent, int listingIndex, ClusteredStreamCachePolicy cachePolicy)
				: this(parent, ClusterDataType.Stream, listingIndex, cachePolicy) {
			}

			private FragmentProvider(ClusteredStreamStorage<TStreamContainerHeader, TStreamListing> parent, ClusterDataType clusterDataType, int listingIndex, ClusteredStreamCachePolicy cachePolicy) {
				_parent = parent;
				_clusterDataType = clusterDataType;
				if (_clusterDataType == ClusterDataType.Stream)
					Guard.ArgumentInRange(listingIndex, 0, _parent._header.Listings, nameof(listingIndex));
				_listingIndex = listingIndex;
				_fragmentCache = new FragmentCache(cachePolicy);
				Reset();
			}

			public long TotalBytes => _clusterDataType switch {
				ClusterDataType.Listing => _parent.AllListingsSize,
				ClusterDataType.Stream => _listing.Size,
				_ => throw new ArgumentOutOfRangeException()
			};

			public int FragmentCount => (int)Math.Ceiling(TotalBytes / (float)_parent.ClusterSize);

			public ReadOnlySpan<byte> GetFragment(int index) {
				Guard.ArgumentInRange(index, 0, FragmentCount - 1, nameof(index));
				TraverseToFragment(index);
				return _parent.GetCluster(_currentCluster).Data;
			}

			public bool TryMapStreamPosition(long position, out int fragmentIndex, out int fragmentPosition) {
				fragmentIndex = (int)(position / _parent.ClusterSize);
				fragmentPosition = (int)(position % _parent.ClusterSize);
				return true;
			}

			public void UpdateFragment(int fragmentIndex, int fragmentPosition, ReadOnlySpan<byte> updateSpan) {
				TraverseToFragment(fragmentIndex);
				var cluster = _parent.GetCluster(_currentCluster);
				updateSpan.CopyTo(cluster.Data.AsSpan(fragmentPosition));
				_parent.UpdateCluster(_currentCluster, cluster);
			}

			public bool TrySetTotalBytes(long length, out int[] newFragments, out int[] deletedFragments) {
				var oldLength = TotalBytes;
				var newTotalClusters = (int)Math.Ceiling(length / (float)_parent.ClusterSize);
				var oldTotalClusters = FragmentCount;
				var currentTotalClusters = oldTotalClusters;
				var newFragmentsL = new List<int>();
				var newClustersL = new List<int>();
				var deletedFragmentsL = new List<int>();
				TraverseToEnd();
				if (newTotalClusters > currentTotalClusters) {
					// add new clusters
					while (currentTotalClusters < newTotalClusters) {
						_currentCluster = currentTotalClusters == 0 ? _parent.AllocateStartCluster(_listingIndex, _clusterDataType) : _parent.AllocateNextCluster(_currentCluster, _clusterDataType);
						_currentFragment++;
						_fragmentCache.SetCluster(_currentFragment, _currentCluster);
						currentTotalClusters++;
						newFragmentsL.Add(_currentFragment);
						newClustersL.Add(_currentCluster);
					}
				} else if (newTotalClusters < currentTotalClusters) {
					// remove clusters from tip
					while (currentTotalClusters > newTotalClusters) {
						var deleteCluster = _currentCluster;
						var deleteFragment = _currentFragment;
						_fragmentCache.InvalidateCluster(_currentCluster);
						TryStepBack();

						// Remember the current position after step back because RemoveCluster may reset this when shuffling listing clusters
						var rememberCurrentCluster = _currentCluster;
						var rememberCurrentFragment = _currentFragment;
						// Also remember if current cluster is tip cluster, because on a shuffle it is moved and new cluster position is deleteCluster
						var isTip = rememberCurrentCluster == _parent._clusters.Count - 1;
						_parent.RemoveCluster(deleteCluster);
						// Restore remembered position (note: when previous is tip, it got moved to deleted position when deleted was removed)
						_currentCluster = isTip ? deleteCluster : rememberCurrentCluster;
						_currentFragment = rememberCurrentFragment;
						currentTotalClusters--;
						deletedFragmentsL.Add(deleteFragment);
					}
				}

				// Erase unused portion of tip cluster when shrinking stream
				if (length < oldLength) {
					var unusedTipClusterBytes = (int)(newTotalClusters * _parent.ClusterSize - length);
					if (unusedTipClusterBytes > 0)
						_parent.FastWriteClusterData(_currentCluster, _parent.ClusterSize - unusedTipClusterBytes, _parent.ZeroClusterBytes.Span.Slice(..unusedTipClusterBytes));
				}

				// Update listing if applicable
				if (_clusterDataType == ClusterDataType.Stream) {
					_listing.Size = (int)length;
					if (_listing.Size == 0)
						_listing.StartCluster = -1;
					if (oldTotalClusters == 0 && newTotalClusters > 0)
						_listing.StartCluster = newClustersL[0];
					_parent.UpdateListing(_listingIndex, _listing);
				} else {
					_parent.AllListingsSize = (int)length;
				}

				newFragments = newFragmentsL.ToArray();
				deletedFragments = deletedFragmentsL.ToArray();
				return true;
			}

			internal void Reset() {
				ResetClusterPointer();
				_fragmentCache.Clear();
			}

			internal void ResetClusterPointer() {
				switch (_clusterDataType) {
					case ClusterDataType.Listing:
						_currentFragment = _parent._header.Listings > 0 ? 0 : -1;
						_currentCluster = _parent._header.Listings > 0 ? 0 : -1;
						break;
					case ClusterDataType.Stream:
						_listing = _parent.GetListing(_listingIndex);
						_currentFragment = _listing.StartCluster > 0 ? 0 : -1;
						_currentCluster = _listing.StartCluster > 0 ? _listing.StartCluster : -1;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			internal void InvalidateCluster(int cluster) {
				if (cluster == _currentCluster)
					ResetClusterPointer();
				_fragmentCache.InvalidateCluster(cluster);
			}

			private void TraverseToStart() {
				// TODO: start moving backward from known smallest fragment 
				while (TryStepBack()) ;
			}

			private void TraverseToEnd() {
				// TODO: start moving forward from known greatest fragment
				var steps = 0;
				while (TryStepForward()) {
					CheckSteps(steps++);
				}
			}

			private void TraverseToFragment(int index) {
				if (_currentFragment == index)
					return;
				
				if (_fragmentCache.TryGetCluster(index, out var cluster)) {
					_currentFragment = index;
					_currentCluster = cluster;
					return;
				}

				var steps = 0;
				if (index < _currentFragment) {
					while (_currentFragment != index) {
						if (!TryStepBack())
							throw new InvalidOperationException($"Unable to seek to fragment {index}");
						CheckSteps(steps++);
					}
				} else if (index > _currentFragment)
					while (_currentFragment != index) {
						if (!TryStepForward())
							throw new InvalidOperationException($"Unable to seek to fragment {index}");
						CheckSteps(steps++);
					}
			}

			private bool TryStepBack() {
				if (_currentFragment <= 0)
					return false;

				_currentFragment--;
				// note: _currentCluster still points to current at this point
				if (!_fragmentCache.TryGetCluster(_currentFragment, out var prevCluster)) {
					_currentCluster = _parent.FastReadClusterPrev(_currentCluster);
					_fragmentCache.SetCluster(_currentFragment, _currentCluster);
				} else _currentCluster = prevCluster;

				return true;
			}

			private bool TryStepForward() {
				if (_currentFragment < 0)
					return false;

				if (!_fragmentCache.TryGetCluster(_currentFragment + 1, out var nextCluster)) {
					nextCluster = _parent.FastReadClusterNext(_currentCluster);
				}

				if (nextCluster == _currentCluster)
					return false;

				if (nextCluster == -1)
					return false;

				_currentFragment++;
				_currentCluster = nextCluster;
				_fragmentCache.SetCluster(_currentFragment, _currentCluster);
				return true;
			}

			private void CheckSteps(int steps) {
				if (steps > FragmentCount)
					throw new CorruptDataException(_parent._header, $"Unable to traverse the cluster-chain due to cyclic dependency (detected at cluster {_currentCluster})");
			}

		}

		internal class FragmentCache {
			private readonly IDictionary<int, int> _fragmentToClusterMap;
			private readonly IDictionary<int, int> _clusterToFragmentMap;

			public FragmentCache(ClusteredStreamCachePolicy policy) {
				if (policy == ClusteredStreamCachePolicy.Scan)
					throw new NotSupportedException(policy.ToString());

				_fragmentToClusterMap = new Dictionary<int, int>();
				_clusterToFragmentMap = new Dictionary<int, int>();
				Policy = policy;
			}

			public ClusteredStreamCachePolicy Policy { get; }

			public bool TryGetCluster(int fragment, out int cluster) {
				if (Policy == ClusteredStreamCachePolicy.None) {
					cluster = default;
					return false;
				}
				return _fragmentToClusterMap.TryGetValue(fragment, out cluster);
			}

			public void SetCluster(int fragment, int cluster) {
				if (Policy == ClusteredStreamCachePolicy.None)
					return;

				_fragmentToClusterMap[fragment] = cluster;
				_clusterToFragmentMap[cluster] = fragment;
			}

			public void Invalidate(int fragment, int cluster) {
				if (Policy == ClusteredStreamCachePolicy.None)
					return;

				_fragmentToClusterMap.Remove(fragment);
				_clusterToFragmentMap.Remove(cluster);
			}
			
			public void InvalidateFragment(int fragment) {
				if (Policy == ClusteredStreamCachePolicy.None)
					return;

				if (_fragmentToClusterMap.TryGetValue(fragment, out var cluster))
					Invalidate(fragment, cluster);
			}

			public void InvalidateCluster(int cluster) {
				if (Policy == ClusteredStreamCachePolicy.None) 
					return;

				if (_clusterToFragmentMap.TryGetValue(cluster, out var fragment))
					Invalidate(fragment, cluster);
			}

			public void Clear() {
				if (Policy == ClusteredStreamCachePolicy.None)
					return;
				_clusterToFragmentMap.Clear();
				_fragmentToClusterMap.Clear();
			}

		}

		#endregion
	}

}
