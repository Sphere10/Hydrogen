using System.IO;

namespace Sphere10.Framework {

	public class ClusteredStorageHeader {
		public const int ByteLength = 256;
		public const int MerkleRootLength = 32;
		public const int MasterKeyLength = 32;

		internal const int VersionOffset = 0;
		internal const int PolicyOffset = sizeof(byte);
		internal const int RecordsOffset = PolicyOffset + sizeof(uint);
		internal const int ClusterSizeOffset = RecordsOffset + sizeof(int);
		internal const int TotalClustersOffset = ClusterSizeOffset + sizeof(int);
		internal const int MerkleRootOffset = TotalClustersOffset + sizeof(int);
		internal const int MasterKeyOffset = MerkleRootOffset + MerkleRootLength;

		private Stream _headerStream;
		private EndianBinaryReader _reader;
		private EndianBinaryWriter _writer;

		private byte? _version;
		private ClusteredStoragePolicy? _policy;
		private int? _records;
		private int? _clusterSize;
		private int? _totalClusters;
		
		private byte[] _merkleRoot;
		private byte[] _masterKey;
		

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

		public ClusteredStoragePolicy Policy {
			get {
				if (!_policy.HasValue) {
					_headerStream.Seek(PolicyOffset, SeekOrigin.Begin);
					_policy = (ClusteredStoragePolicy) _reader.ReadInt32();
				}
				return _policy.Value;
			}
			set {
				if (_policy == value)
					return;
				_policy = value;
				_headerStream.Seek(PolicyOffset, SeekOrigin.Begin);
				_writer.Write((int)_policy.Value);
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

		public byte[] MerkleRoot {
			get {
				if (_merkleRoot == null) {
					_headerStream.Seek(MerkleRootOffset, SeekOrigin.Begin);
					_merkleRoot = _reader.ReadBytes(MerkleRootLength);
				}
				return _merkleRoot;
			}
			set {
				if (ByteArrayEqualityComparer.Instance.Equals(_merkleRoot, value))
					return;
				_merkleRoot = value;
				_headerStream.Seek(MerkleRootOffset, SeekOrigin.Begin);
				_writer.Write(_merkleRoot);
			}
		}

		public byte[] MasterKey {
			get {
				if (_masterKey == null) {
					_headerStream.Seek(MasterKeyOffset, SeekOrigin.Begin);
					_masterKey = _reader.ReadBytes(MasterKeyLength);
				}
				return _masterKey;
			}
			set {
				if (ByteArrayEqualityComparer.Instance.Equals(_masterKey, value))
					return;
				_masterKey = value;
				_headerStream.Seek(MasterKeyOffset, SeekOrigin.Begin);
				_writer.Write(_masterKey);
			}
		}

		public void AttachTo(Stream rootStream, Endianness endianness) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.Argument(rootStream.Length >= ByteLength, nameof(rootStream), "Missing header");
			_headerStream = new BoundedStream(rootStream, 0, 255);
			_reader = new EndianBinaryReader(EndianBitConverter.For(endianness), _headerStream);
			_writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), _headerStream);
			_version = null;
			_policy = 0;
			_clusterSize = null;
			_totalClusters = null;
			_records = null;
			_merkleRoot = new byte[MerkleRootLength];
			_masterKey = new byte[MasterKeyLength];
		}

		public void CreateIn(byte version, Stream rootStream, int clusterSize, Endianness endianness) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.Argument(rootStream.Length == 0, nameof(rootStream), "Must be empty");
			rootStream.Seek(0, SeekOrigin.Begin);
			var writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), rootStream);
			writer.Write(version); // Version
			writer.Write((int)0); // Policy
			writer.Write((int)0); // Records
			writer.Write(clusterSize); // ClusterSize
			writer.Write((int)0); // TotalClusters 
			writer.Write(new byte[MerkleRootLength]); // MerkleRoot 
			writer.Write(new byte[MasterKeyLength]); // MasterKey

			writer.Write(Tools.Array.Gen<byte>(ByteLength - sizeof(byte) - sizeof(int) - sizeof(int) - sizeof(int) - sizeof(int) - MerkleRootLength - MasterKeyLength, 0)); // header padding
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

		public override string ToString() => $"[ClusteredStreamStorage] Version: {Version}, Cluster Size: {ClusterSize}, Total Clusters: {TotalClusters}, Records: {RecordsCount}, Policy: {Policy}, MerkleRoot: {MerkleRoot.ToHexString(true)}";

	}

}
