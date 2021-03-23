using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.Collections.StreamMapped {

	public class ItemListingSerializer : IObjectSerializer<ItemListing> {
		public bool IsFixedSize { get; } = true;
		public int FixedSize { get; } = sizeof(int) + sizeof(int);
		public int CalculateTotalSize(IEnumerable<ItemListing> items, bool calculateIndividualItems, out int[] itemSizes) {
			var listings = items as ItemListing[] ?? items.ToArray();
			int sum = listings.Length * FixedSize;
			itemSizes = Enumerable.Repeat(FixedSize, listings.Length).ToArray();

			return sum;
		}

		public int CalculateSize(ItemListing item) => FixedSize;

		public int Serialize(ItemListing @object, EndianBinaryWriter writer) {
			writer.Write(@object.Size);
			writer.Write(@object.ClusterStartIndex);

			return sizeof(int) + sizeof(int);
		}

		public ItemListing Deserialize(int size, EndianBinaryReader reader) {
			int itemSize = reader.ReadInt32();
			int startIndex = reader.ReadInt32();

			return new ItemListing {
				ClusterStartIndex = startIndex,
				Size = itemSize
			};
		}
	}

}
