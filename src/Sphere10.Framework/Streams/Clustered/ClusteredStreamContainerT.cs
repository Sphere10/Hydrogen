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
	/// An <see cref="IStreamContainer{TStreamListing}"/> implementation which interlaces component streams together over a single logical stream using a clustering approach similar to how physical disk
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
	public class ClusteredStreamContainerT<TStreamContainerHeader, TStreamListing> : IStreamContainer<TStreamListing>
		where TStreamContainerHeader : ClusteredStreamContainerHeader, new()
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

		public ClusteredStreamContainerT(Stream rootStream, int clusterSize, IItemSerializer<TStreamListing> listingSerializer, Endianness endianness = Endianness.LittleEndian) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.ArgumentInRange(clusterSize, 1, int.MaxValue, nameof(clusterSize));
			Guard.ArgumentNotNull(listingSerializer, nameof(listingSerializer));
			Guard.Argument(listingSerializer.IsStaticSize, nameof(listingSerializer), "Listings must be constant sized");
			_stream = rootStream;
			_endianness = endianness;
			_listingSerializer = listingSerializer;
			_header = new TStreamContainerHeader();
			if (_stream.Length > 0) {
				_header.AttachTo(_stream, _endianness);
			} else {
				_header.CreateIn(1, _stream, clusterSize, _endianness);
			}
			Guard.Argument(_header.Version == 1, nameof(rootStream), $"Unsupported version '{_header.Version}'");
			Guard.Argument(_header.ClusterSize == clusterSize, nameof(rootStream), $"Inconsistent cluster sizes (stream header had '{_header.ClusterSize}')");
			_clusters = new StreamPagedList<Cluster>(
				new ClusterSerializer(clusterSize),
				new NonClosingStream(new BoundedStream(rootStream, ClusteredStreamContainerHeader.ByteLength, long.MaxValue) { UseRelativeOffset = true, AllowResize = true }),
				_endianness
			);
			_listingsFragmentProvider = new FragmentProvider(this);
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
			ListingsSize = _header.Listings * _listingSerializer.StaticSize;
			_lock = new object();
			_openFragmentProvider = null;
			_openStream = null;
			_openListing = null;
			_version = 0;
			ZeroClusterBytes = Tools.Array.Gen<byte>(clusterSize, 0);
		}

		public virtual int Count => _header.Listings;

		public IReadOnlyList<TStreamListing> Listings => _listings;

		internal int Clusters => _clusters.Count;

		private int ListingsSize { get; set; }

		private ReadOnlyMemory<byte> ZeroClusterBytes { get; }

		#region Streams

		public byte[] ReadAll(int index) => Open(index).ReadAllAndDispose();

		public void CreateAllBytes(ReadOnlySpan<byte> bytes) {
			using var stream = Add();
			stream.Write(bytes);
		}

		public void OverwriteAllBytes(int index, ReadOnlySpan<byte> bytes) {
			using var stream = Open(index);
			stream.SetLength(0);
			stream.Write(bytes);
		}

		public void AppendAllBytes(int index, ReadOnlySpan<byte> bytes) {
			using var stream = Open(index);
			stream.Write(bytes);
		}

		public Stream Add() {
			lock (_lock) {
				CheckNotOpened();
				var listing = AddListing(out var index);  // the first listing add will allocate cluster 0 for the listings stream
				listing.Size = 0;
				listing.StartCluster = -1; // when opened and written, the first cluster will be allocated
				UpdateListing(index, listing);
				return Open(index); ;
			}
		}

		public Stream Open(int index) {
			lock (_lock) {
				CheckListingIndex(index);
				CheckNotOpened();
				_openFragmentProvider = new FragmentProvider(this, index);
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

		public void Insert(int index) {
			throw new NotImplementedException();
		}

		public void Swap(int first, int second) {
			throw new NotImplementedException();
		}

		public void Clear() {
			lock (_lock) {
				CheckNotOpened();
				_header.Listings = 0;
				_header.TotalClusters = 0;
				_clusters.Clear();
				_listingsFragmentProvider.ResetClusterTacking();
			}
		}

		public void Optimize() {
			throw new NotImplementedException();
		}

		public override string ToString() => _header.ToString();

		internal string ToStringWithContents() {
			var stringBuilder = new FastStringBuilder();
			stringBuilder.AppendLine(this.ToString());
			stringBuilder.AppendLine("Listings:");
			foreach (var (listing, i) in _listings.WithIndex()) {
				stringBuilder.AppendLine($"\t{i}: {listing}");
			}
			stringBuilder.AppendLine("Clusters:");
			foreach (var (cluster, i) in _clusters.WithIndex()) {
				stringBuilder.AppendLine($"\t{i}: {cluster}");
			}

			return stringBuilder.ToString();
		}

		#endregion

		#region Listings

		private TStreamListing GetListing(int index) {
			return _listings.Read(index);
		}

		private TStreamListing AddListing(out int index) {
			var listing = new TStreamListing();
			_listings.Add(listing);
			_header.Listings++;
			index = _listings.Count - 1;
			return listing;
		}

		private void UpdateListing(int index, TStreamListing listing) {
			_listings.Update(index, listing);
			if (_openListing.HasValue && _openListing.Value == index)
				_openFragmentProvider.ResetClusterTacking();
		}

		private void RemoveListing(int index) {
			for (var i = index + 1; i < _listings.Count; i++) {
				var higherListing = _listings[i];
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
				_listingsFragmentProvider.ResetClusterTacking();

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
		private Cluster GetCluster(int clusterIndex) => _clusters[clusterIndex];

		private void UpdateCluster(int clusterIndex, Cluster cluster) => _clusters[clusterIndex] = cluster;

		private void RemoveCluster(int clusterIndex) {
			var next = FastReadClusterNext(clusterIndex);
			Guard.Ensure(next == -1, $"Can only remove a cluster from end of cluster-linked chain (clusterIndex = {clusterIndex})");
			var traits = FastReadClusterTraits(clusterIndex);
			var prevPointsToListing = traits.HasFlag(ClusterTraits.First) && traits.HasFlag(ClusterTraits.Data);
			if (!prevPointsToListing) {
				var prev = FastReadClusterPrev(clusterIndex);
				if (prev != -1)
					FastWriteClusterNext(prev, -1);  // prev.next points to deleted cluster, so make it point to nothing
			}
			MigrateTipClusterTo(clusterIndex);
			_clusters.RemoveAt(^1);
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
				clustersRemoved++;
				cluster = nextCluster;
			}
			_clusters.RemoveRange(_clusters.Count - clustersRemoved, clustersRemoved);
			_header.TotalClusters -= clustersRemoved;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ClusterTraits FastReadClusterTraits(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, 0, 1, out var bytes);
			return (ClusterTraits)bytes[0];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FastWriteClusterTraits(int clusterIndex, ClusterTraits traits) {
			_clusters.WriteItemBytes(clusterIndex, 0, new byte[(byte)traits]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int FastReadClusterPrev(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, sizeof(byte), 4, out var bytes);
			return _clusters.Reader.BitConverter.ToInt32(bytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FastWriteClusterPrev(int clusterIndex, int prev) {
			var bytes = _clusters.Writer.BitConverter.GetBytes(prev);
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte), bytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int FastReadClusterNext(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, sizeof(byte) + sizeof(int), 4, out var bytes);
			return _clusters.Reader.BitConverter.ToInt32(bytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FastWriteClusterNext(int clusterIndex, int next) {
			var bytes = _clusters.Writer.BitConverter.GetBytes(next);
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte) + sizeof(int), bytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FastWriteClusterData(int clusterIndex, int offset, ReadOnlySpan<byte> data) {
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte) + sizeof(int) + sizeof(int) + offset, data);
		}

		#endregion

		#region Aux methods

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckListingIndex(int index, string msg = null)
			=> Guard.Argument(0 <= index && index < _listings.Count, nameof(index), msg ?? "Index out of bounds");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckClusterIndex(int index, string msg = null)
			=> Guard.Argument(index == -1 || (0 <= index && index < _clusters.Count), nameof(index), msg ?? "Cluster index out of bounds (or not -1)");

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

			var tipCluster = _clusters[tipIndex];
			var toCluster = _clusters[to];
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
			if (tipCluster.Traits.HasFlag(ClusterTraits.Listing))
				_listingsFragmentProvider.ResetClusterTacking();
			//else if (tipCluster.Traits.HasFlag(ClusterTraits.Data) && _openFragmentProvider != null)
			//	_openFragmentProvider.ResetClusterTacking();
		}

		#endregion

		#region Inner Types

		internal class FragmentProvider : IStreamFragmentProvider {
			private readonly ClusteredStreamContainerT<TStreamContainerHeader, TStreamListing> _parent;
			private readonly ClusterDataType _clusterDataType;
			private int _currentFragment;
			private int _currentCluster;
			private readonly int _listingIndex;
			private TStreamListing _listing;

			public FragmentProvider(ClusteredStreamContainerT<TStreamContainerHeader, TStreamListing> parent)
				: this(parent, ClusterDataType.Listing, -1) {
			}

			public FragmentProvider(ClusteredStreamContainerT<TStreamContainerHeader, TStreamListing> parent, int listingIndex)
				: this(parent, ClusterDataType.Stream, listingIndex) {
			}

			private FragmentProvider(ClusteredStreamContainerT<TStreamContainerHeader, TStreamListing> parent, ClusterDataType clusterDataType, int listingIndex) {
				_parent = parent;
				_clusterDataType = clusterDataType;
				if (_clusterDataType == ClusterDataType.Stream)
					Guard.ArgumentInRange(listingIndex, 0, _parent._header.Listings, nameof(listingIndex));
				_listingIndex = listingIndex;
				ResetClusterTacking();
			}

			public long TotalBytes => _clusterDataType switch {
				ClusterDataType.Listing => _parent.ListingsSize,
				ClusterDataType.Stream => _listing.Size,
				_ => throw new ArgumentOutOfRangeException()
			};

			public int FragmentCount => (int)Math.Ceiling(TotalBytes / (float)_parent._header.ClusterSize);

			public ReadOnlySpan<byte> GetFragment(int index) {
				Guard.ArgumentInRange(index, 0, FragmentCount - 1, nameof(index));
				TraverseToFragment(index);
				return _parent.GetCluster(_currentCluster).Data;
			}

			public bool TryMapStreamPosition(long position, out int fragmentIndex, out int fragmentPosition) {
				fragmentIndex = (int)(position / _parent._header.ClusterSize);
				fragmentPosition = (int)(position % _parent._header.ClusterSize);
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
				var newTotalClusters = (int)Math.Ceiling(length / (float)_parent._header.ClusterSize);
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
						currentTotalClusters++;
						newFragmentsL.Add(_currentFragment);
						newClustersL.Add(_currentCluster);
					}
				} else if (newTotalClusters < currentTotalClusters) {

					// remove clusters from tip
					while (currentTotalClusters > newTotalClusters) {
						var deleteCluster = _currentCluster;
						var deleteFragment = _currentFragment;
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
					var unusedTipClusterBytes = (int)(newTotalClusters * _parent._header.ClusterSize - length);
					if (unusedTipClusterBytes > 0)
						_parent.FastWriteClusterData(_currentCluster, _parent._header.ClusterSize - unusedTipClusterBytes, _parent.ZeroClusterBytes.Span.Slice(..unusedTipClusterBytes));
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
					_parent.ListingsSize = (int)length;
				}

				newFragments = newFragmentsL.ToArray();
				deletedFragments = deletedFragmentsL.ToArray();
				return true;
			}

			internal void ResetClusterTacking() {
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

			private void TraverseToStart() {
				// TODO: solve for case where deliberate data-corruption results in infinite loop
				while (TryStepBack()) ;
			}

			private void TraverseToEnd() {
				// TODO: solve for case where deliberate data-corruption results in infinite loop
				while (TryStepForward()) ;
			}

			private void TraverseToFragment(int index) {
				// TODO: solve for case where deliberate data-corruption results in infinite loop
				if (index < _currentFragment) {
					while (_currentFragment != index)
						if (!TryStepBack())
							throw new InvalidOperationException($"Unable to seek to fragment {index}");
				} else if (index > _currentFragment)
					while (_currentFragment != index)
						if (!TryStepForward())
							throw new InvalidOperationException($"Unable to seek to fragment {index}");
			}

			private bool TryStepBack() {
				if (_currentFragment <= 0)
					return false;
				_currentFragment--;
				_currentCluster = _parent.FastReadClusterPrev(_currentCluster);
				return true;
			}

			private bool TryStepForward() {
				if (_currentFragment < 0)
					return false;

				var nextCluster = _parent.FastReadClusterNext(_currentCluster);

				if (nextCluster == _currentCluster)
					return false;

				if (nextCluster == -1)
					return false;

				_currentFragment++;
				_currentCluster = nextCluster;
				return true;
			}

		}

		#endregion
	}

}
