using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	public class StreamMappedDynamicClusteredList<T> : RangedListBase<T> {

		private const int HeaderSize = 256;
		private const int ListingSize = sizeof(int) + sizeof(int);

		private readonly int _clusterDataSize;
		private readonly IObjectSerializer<T> _serializer;
		private readonly StreamMappedPagedList<Cluster> _clusters;

		private int _listingGenesisIndex;
		private int _storageGenesisIndex;
		private int _count;
		private int _storageClusterTail;

		private readonly BoundedStream _headerStream;
		private readonly Stream _innerStream;

		private IExtendedList<int> _listingOffsets;
		private List<int> _freeClusters;

		public StreamMappedDynamicClusteredList(int clusterDataSize, IObjectSerializer<T> serializer, Stream stream) {
			_clusterDataSize = clusterDataSize;
			_serializer = serializer;

			_innerStream = stream;
			_headerStream = new BoundedStream(stream, 0, HeaderSize - 1);
			_clusters = new StreamMappedPagedList<Cluster>(new ClusterSerializer(clusterDataSize), new NonClosingStream(new BoundedStream(stream, _headerStream.MaxAbsolutePosition +1, long.MaxValue)));
			_listingOffsets = new ExtendedList<int>();

			if (!RequiresLoad) {
				WriteHeader();
				Loaded = true;
			}
		}

		public override int Count { get; }

		public bool RequiresLoad => _innerStream.Length > 0 && !Loaded;

		public bool Loaded { get; private set; } = false;

		public override void AddRange(IEnumerable<T> items) {
			CheckState();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			throw new System.NotImplementedException();
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			throw new System.NotImplementedException();
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			throw new System.NotImplementedException();
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			throw new System.NotImplementedException();
		}

		public override void RemoveRange(int index, int count) {
			throw new System.NotImplementedException();
		}

		public void Load() {
			// if (RequiresLoad && !Loaded) {
			// 	throw new InvalidOperationException("List requires loading, use Load method");
			// }
			
			ReadHeader();
			
			for (int i = 0; i < _clusters.Count; i++) {
				_clusters.ReadItemRaw(i, 0, sizeof(int), out var bytes);
				ClusterTraits traits = (ClusterTraits)BitConverter.ToInt32(bytes);

				if (traits == ClusterTraits.Free) {
					_freeClusters.Add(i);
				}
			}
			
			Loaded = true;
		}

		private void AddItemToClusters(T item) {
			var clusters = new List<Cluster>();

			byte[] data;
			using (var stream = new MemoryStream()) {
				_serializer.Serialize(item, new EndianBinaryWriter(EndianBitConverter.Little, stream));
				data = stream.ToArray();
			}

			int[] clusterNumbers = GetFreeClusters(data.Length).ToArray();

			var segments = data.PartitionBySize(x => 1, _clusterDataSize)
				.ToList();

			for (var i = 0; i < segments.Count; i++) {
				var segment = segments[i].ToArray();
				var clusterData = new byte[_clusterDataSize];
				segment.CopyTo(clusterData, 0);

				clusters.Add(new Cluster {
					Traits = ClusterTraits.Used,
					Number = clusterNumbers[i],
					Data = clusterData,
					Next = segments.Count - 1 == i ? -1 : clusterNumbers[i + 1]
				});
			}

			foreach (var cluster in clusters)
				if (!_clusters.Any())
					_clusters.Add(cluster);
				else if (cluster.Number >= _clusters.Count)
					_clusters.Add(cluster);
				else
					_clusters[cluster.Number] = cluster;

			WriteHeader();
		}

		private T ReadItemFromClusters(int startCluster, int size) {
			int? next = startCluster;
			var remaining = size;

			var builder = new ByteArrayBuilder();

			while (next != -1) {
				var cluster = _clusters[next.Value];
				next = cluster.Next;

				if (cluster.Next < 0) {
					builder.Append(cluster.Data.Take(remaining).ToArray());
				} else {
					builder.Append(cluster.Data);
					remaining -= cluster.Data.Length;
				}
			}

			return _serializer.Deserialize(size,
				new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(builder.ToArray())));
		}

		private IEnumerable<int> GetFreeClusters(int dataSize) {

			int numberRequired = (int)Math.Ceiling((decimal)dataSize / _clusterDataSize);
			List<int> clusterNumbers = _freeClusters.Take(numberRequired)
				.ToList();

			_freeClusters.RemoveRangeSequentially(0, clusterNumbers.Count);

			for (int i = 0; i < clusterNumbers.Count - numberRequired; i++) {
				clusterNumbers.Add(_storageClusterTail++);
			}

			return clusterNumbers;
		}

		private void WriteHeader() {

			byte[] headerBytes = Tools.Array.Gen(HeaderSize, default(byte));

			new ByteArrayBuilder()
				.Append(EndianBitConverter.Little.GetBytes(_count))
				.Append(EndianBitConverter.Little.GetBytes(_listingGenesisIndex))
				.Append(EndianBitConverter.Little.GetBytes(_storageGenesisIndex))
				.Append(EndianBitConverter.Little.GetBytes(_storageClusterTail))
				.ToArray()
				.CopyTo(headerBytes, 0);

			var writer = new EndianBinaryWriter(EndianBitConverter.Little, _headerStream);
			_headerStream.Seek(0, SeekOrigin.Begin);
			writer.Write(headerBytes);
		}

		private void ReadHeader() {
			var reader = new EndianBinaryReader(EndianBitConverter.Little, _headerStream);
			_headerStream.Seek(0, SeekOrigin.Begin);
			
			_count = reader.ReadInt32();
			_listingGenesisIndex = reader.ReadInt32();
			_storageGenesisIndex = reader.ReadInt32();
			_storageClusterTail = reader.ReadInt32();
		}

		private void CheckState() {
			if (RequiresLoad) {
				throw new InvalidOperationException("List requires loading as stream contains existing data.");
			}
		}
	}
}
