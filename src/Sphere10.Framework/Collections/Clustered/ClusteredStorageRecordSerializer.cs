namespace Sphere10.Framework {
	public class ClusteredStorageRecordSerializer : StaticSizeItemSerializerBase<ClusteredStorageRecord> {
		private readonly ClusteredStoragePolicy _policy;
		private readonly int _keySize;

		public ClusteredStorageRecordSerializer(ClusteredStoragePolicy policy, int keySize)
			: base(DetermineSizeBasedOnPolicy(policy, keySize)) {
			_policy = policy;
			_keySize = keySize;
		}

		public override bool TrySerialize(ClusteredStorageRecord item, EndianBinaryWriter writer) {
			writer.Write((byte)item.Traits);
			writer.Write(item.StartCluster);
			writer.Write(item.Size);
			if (_policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
				writer.Write(item.KeyChecksum);

			if (_policy.HasFlag(ClusteredStoragePolicy.TrackKey)) {
				if (item.Key?.Length != _keySize)
					return false;
				writer.Write(item.Key);
			}

			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out ClusteredStorageRecord item) {
			item = new ClusteredStorageRecord {
				Traits = (ClusteredStorageRecordTraits) reader.ReadByte(),
				StartCluster = reader.ReadInt32(),
				Size = reader.ReadInt32()
			};

			if (_policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
				item.KeyChecksum = reader.ReadInt32();

			if (_policy.HasFlag(ClusteredStoragePolicy.TrackKey))
				item.Key = reader.ReadBytes(_keySize);

			return true;
		}


		static int DetermineSizeBasedOnPolicy(ClusteredStoragePolicy policy, int keySize) {
			var size = sizeof(byte) + sizeof(int) + sizeof(int); // Traits + Size + StartCluster

			if (policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
				size += sizeof(int);

			if (policy.HasFlag(ClusteredStoragePolicy.TrackKey)) {
				size += keySize;
			}
			return size;
		}
	}
}
