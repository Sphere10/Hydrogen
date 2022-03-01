namespace Sphere10.Framework {
	public class ClusteredStorageRecordSerializer : StaticSizeItemSerializer<ClusteredStorageRecord> {
		private readonly ClusteredStoragePolicy _policy;

		public ClusteredStorageRecordSerializer(ClusteredStoragePolicy policy)
			: base(DetermineSizeBasedOnPolicy(policy)) {
			_policy = policy;
		}


		public override bool TrySerialize(ClusteredStorageRecord item, EndianBinaryWriter writer) {
			writer.Write((byte)item.Traits);
			writer.Write(item.StartCluster);
			writer.Write(item.Size);
			if (_policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
				writer.Write(item.KeyChecksum);

			if (_policy.HasFlag(ClusteredStoragePolicy.TrackDigests)) {
				if (item.ValueDigest?.Length != 32)
					return false;
				writer.Write(item.ValueDigest);
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

			if (_policy.HasFlag(ClusteredStoragePolicy.TrackDigests))
				item.ValueDigest = reader.ReadBytes(32);

			return true;
		}


		static int DetermineSizeBasedOnPolicy(ClusteredStoragePolicy policy) {
			var size = sizeof(byte) + sizeof(int) + sizeof(int); // Traits + Size + StartCluster

			if (policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
				size += sizeof(int);

			if (policy.HasFlag(ClusteredStoragePolicy.TrackDigests))
				size += 32;

			return size;
		}
	}
}
