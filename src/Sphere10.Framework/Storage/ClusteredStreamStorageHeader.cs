using System.IO;

namespace Sphere10.Framework {

	public class ClusteredStreamStorageHeader {
		public const int ByteLength = 256;

		private const int VersionOffset = 0;
		private const int ClusterSizeOffset = VersionOffset + sizeof(byte);
		private const int TotalClustersOffset = ClusterSizeOffset + sizeof(int);
		private const int ListingsOffset = TotalClustersOffset + sizeof(int);

		private Stream _headerStream;
		private EndianBinaryReader _reader;
		private EndianBinaryWriter _writer;

		private byte? _version;
		private int? _clusterSize;
		private int? _totalClusters;
		private int? _listings;

		public virtual void AttachTo(Stream rootStream, Endianness endianness) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.Argument(rootStream.Length >= ByteLength, nameof(rootStream), "Missing header");
			_headerStream = new BoundedStream(rootStream, 0, 255);
			_reader = new EndianBinaryReader(EndianBitConverter.For(endianness), _headerStream);
			_writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), _headerStream);
			_version = null;
			_clusterSize = null;
			_totalClusters = null;
			_listings = null;
			CheckHeaderIntegrity();
		}

		public virtual void CreateIn(byte version, Stream rootStream, int clusterSize, Endianness endianness) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.Argument(rootStream.Length == 0, nameof(rootStream), "Must be empty");
			rootStream.Seek(0, SeekOrigin.Begin);
			var writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), rootStream);
			writer.Write(version); // Version
			writer.Write(clusterSize); // ClusterSize
			writer.Write((int)0); // TotalClusters 
			writer.Write((int)0); // Listings
			writer.Write(Tools.Array.Gen<byte>(ByteLength - sizeof(byte) - sizeof(int) - sizeof(int) - sizeof(int), 0)); // header padding
			AttachTo(rootStream, endianness);
		}


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

		public int Listings {
			get {
				if (!_listings.HasValue) {
					_headerStream.Seek(ListingsOffset, SeekOrigin.Begin);
					_listings = _reader.ReadInt32();
				}
				return _listings.Value;
			}
			set {
				if (_listings == value)
					return;
				_listings = value;
				_headerStream.Seek(ListingsOffset, SeekOrigin.Begin);
				_writer.Write(_listings.Value);
			}
		}

		public override string ToString() => $"[BlobContainer] Version: {Version}, Cluster Size: {ClusterSize}, Total Clusters: {TotalClusters}, Listings: {Listings}";

		private void CheckHeaderIntegrity() {
			if (Version != 1)
				throw new CorruptDataException($"Corrupt header (Version field was {Version} bytes)");

			if (ClusterSize <= 0)
				throw new CorruptDataException($"Corrupt header (ClusterSize field was {ClusterSize} bytes)");

			if (TotalClusters < 0)
				throw new CorruptDataException($"Corrupt header (TotalClusters was {TotalClusters})");

			if (Listings < 0)
				throw new CorruptDataException($"Corrupt header (Listings field was {Listings})");
		}
	}
}
