using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	public class ItemListingSerializer : IItemSerializer<ItemListing> {
		public bool IsFixedSize { get; } = true;
		public int FixedSize { get; } = sizeof(int) + sizeof(int);
		public int CalculateTotalSize(IEnumerable<ItemListing> items, bool calculateIndividualItems, out int[] itemSizes) {
			var listings = items as ItemListing[] ?? items.ToArray();
			int sum = listings.Length * FixedSize;
			itemSizes = Enumerable.Repeat(FixedSize, listings.Length).ToArray();

			return sum;
		}

		public int CalculateSize(ItemListing item) => FixedSize;

		public bool TrySerialize(ItemListing item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				writer.Write(item.Size);
				writer.Write(item.ClusterStartIndex);

				bytesWritten = sizeof(int) + sizeof(int);

				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out ItemListing item) {
			try {
				var itemSize = reader.ReadInt32();
				var startIndex = reader.ReadInt32();

				item = new ItemListing {
					ClusterStartIndex = startIndex,
					Size = itemSize
				};

				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}
}
