using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ClusteredListTests_Bug1 {

		[Test]
		public void BugFoundInClusteredDictionary1([Values]ClusteringType clusteringType) {
			using var stream = new MemoryStream();
			ClusteredListImplBase<KeyValuePair<string, byte[]>, ItemListing> clusteredList =
				clusteringType switch {
					ClusteringType.Static => new StaticClusteredList<KeyValuePair<string, byte[]>, ItemListing>(32, 100, 30000, stream, new KeyValuePairSerializer<string, byte[]>(new StringSerializer(Encoding.UTF8), new ByteArraySerializer()), new ItemListingSerializer()),
					ClusteringType.Dynamic => new DynamicClusteredList<KeyValuePair<string, byte[]>, ItemListing>(32, stream, new KeyValuePairSerializer<string, byte[]>(new StringSerializer(Encoding.UTF8), new ByteArraySerializer()), new ItemListingSerializer()),
					_ => throw new ArgumentOutOfRangeException(nameof(clusteringType), clusteringType, null)
				};
			clusteredList.ListingActivator = NewListingInstance;
			var item = KeyValuePair.Create("alpha", Tools.Array.Gen<byte>(102, default));
			Assert.That(() => clusteredList.Add(item), Throws.Nothing);
		}

		private ItemListing NewListingInstance(object source, KeyValuePair<string, byte[]> item, int itemSizeBytes, int clusterStartIndex)
			=> new() {
				Size = itemSizeBytes,
				ClusterStartIndex = clusterStartIndex,
				Traits = ItemListingTraits.Used,
				KeyChecksum = item.Key.GetHashCode(),
			};


		[StructLayout(LayoutKind.Sequential)]
		private struct ItemListing : IClusteredItemListing {
			public int ClusterStartIndex { get; set; }

			public int Size { get; set; }

			public int KeyChecksum { get; set; }

			public ItemListingTraits Traits { get; set; }

		}

		[Flags]
		private enum ItemListingTraits : byte {
			Used = 1 << 0
		}

		private class ItemListingSerializer : StaticSizeObjectSerializer<ItemListing> {

			public ItemListingSerializer()
				: base(sizeof(int) + sizeof(int) + sizeof(int) + sizeof(byte)) {
			}

			public override bool TrySerialize(ItemListing item, EndianBinaryWriter writer) {
				writer.Write(item.ClusterStartIndex);
				writer.Write(item.Size);
				writer.Write(item.KeyChecksum);
				writer.Write((byte)item.Traits);
				return true;
			}

			public override bool TryDeserialize(EndianBinaryReader reader, out ItemListing item) {
				item = new ItemListing {
					ClusterStartIndex = reader.ReadInt32(),
					Size = reader.ReadInt32(),
					KeyChecksum = reader.ReadInt32(),
					Traits = (ItemListingTraits)reader.ReadByte()
				};
				return true;
			}
		}

	}
}
