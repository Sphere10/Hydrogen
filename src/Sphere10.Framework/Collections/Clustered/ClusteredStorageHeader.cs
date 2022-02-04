using System.IO;

namespace Sphere10.Framework {

	public class ClusteredStorageHeader : IClusteredStorageHeader {
		public const int ByteLength = 256;

		internal const int VersionOffset = 0;
		internal const int RecordsOffset = sizeof(byte);
		internal const int ClusterSizeOffset = RecordsOffset + sizeof(int);
		internal const int TotalClustersOffset = ClusterSizeOffset + sizeof(int);

		private Stream _headerStream;
		private EndianBinaryReader _reader;
		private EndianBinaryWriter _writer;

		private byte? _version;
		private int? _clusterSize;
		private int? _totalClusters;
		private int? _records;

		public byte Version {
			get {
				if (!_version.HasValue) {
					_headerStream.Seek(VersionOffset, SeekOrigin.Begin);
					_version = _reader.ReadByte();
				}
				return _version.Value;
			}
			set {
				Guard.Argument(value == 1, nameof(value), "Only version 1 supported");
				if (_version == value)
					return;
				_clusterSize = value;
				_headerStream.Seek(VersionOffset, SeekOrigin.Begin);
				_writer.Write(_clusterSize.Value);
			}
		}

		public int RecordsCount {
			get {
				if (!_records.HasValue) {
					_headerStream.Seek(RecordsOffset, SeekOrigin.Begin);
					_records = _reader.ReadInt32();
				}
				return _records.Value;
			}
			set {
				if (_records == value)
					return;
				_records = value;
				_headerStream.Seek(RecordsOffset, SeekOrigin.Begin);
				_writer.Write(_records.Value);
			}
		}

		public int ClusterSize {
			get {
				if (!_clusterSize.HasValue) {
					_headerStream.Seek(ClusterSizeOffset, SeekOrigin.Begin);
					_clusterSize = _reader.ReadInt32();
				}
				return _clusterSize.Value;
			}
			set {
				if (_clusterSize == value)
					return;
				_clusterSize = value;
				_headerStream.Seek(ClusterSizeOffset, SeekOrigin.Begin);
				_writer.Write(_clusterSize.Value);
			}
		}

		public int TotalClusters {
			get {
				if (!_totalClusters.HasValue) {
					_headerStream.Seek(TotalClustersOffset, SeekOrigin.Begin);
					_totalClusters = _reader.ReadInt32();
				}
				return _totalClusters.Value;
			}
			set {
				if (_totalClusters == value)
					return;
				_totalClusters = value;
				_headerStream.Seek(TotalClustersOffset, SeekOrigin.Begin);
				_writer.Write(_totalClusters.Value);
			}
		}

		public void AttachTo(Stream rootStream, Endianness endianness) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.Argument(rootStream.Length >= ByteLength, nameof(rootStream), "Missing header");
			_headerStream = new BoundedStream(rootStream, 0, 255);
			_reader = new EndianBinaryReader(EndianBitConverter.For(endianness), _headerStream);
			_writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), _headerStream);
			_version = null;
			_clusterSize = null;
			_totalClusters = null;
			_records = null;
		}

		public void CreateIn(byte version, Stream rootStream, int clusterSize, Endianness endianness) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.Argument(rootStream.Length == 0, nameof(rootStream), "Must be empty");
			rootStream.Seek(0, SeekOrigin.Begin);
			var writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), rootStream);
			writer.Write(version); // Version
			writer.Write((int)0); // Records
			writer.Write(clusterSize); // ClusterSize
			writer.Write((int)0); // TotalClusters 
			writer.Write(Tools.Array.Gen<byte>(ByteLength - sizeof(byte) - sizeof(int) - sizeof(int) - sizeof(int), 0)); // header padding
			AttachTo(rootStream, endianness);
		}

		public void CheckHeaderIntegrity() {
			if (Version != 1)
				throw new CorruptDataException($"Corrupt header (Version field was {Version} bytes)");

			if (ClusterSize <= 0)
				throw new CorruptDataException($"Corrupt header (ClusterSize field was {ClusterSize} bytes)");

			if (TotalClusters < 0)
				throw new CorruptDataException($"Corrupt header (TotalClusters was {TotalClusters})");

			if (RecordsCount < 0)
				throw new CorruptDataException($"Corrupt header (Records field was {RecordsCount})");
		}

		public override string ToString() => $"[ClusteredStreamStorage] Version: {Version}, Cluster Size: {ClusterSize}, Total Clusters: {TotalClusters}, Records: {RecordsCount}";

	}

}
