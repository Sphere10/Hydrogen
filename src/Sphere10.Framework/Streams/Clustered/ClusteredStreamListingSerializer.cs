namespace Sphere10.Framework {
	public class ClusteredStreamListingSerializer : StaticSizeObjectSerializer<ClusteredStreamListing> {

		public ClusteredStreamListingSerializer()
			: base(sizeof(int) + sizeof(int)) {
		}

		public override bool TrySerialize(ClusteredStreamListing item, EndianBinaryWriter writer) {
			writer.Write(item.StartCluster);
			writer.Write(item.Size);
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out ClusteredStreamListing item) {
			item = new ClusteredStreamListing {
				StartCluster = reader.ReadInt32(),
				Size = reader.ReadInt32()
			};
			return true;
		}
	}
}
