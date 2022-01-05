//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.InteropServices;
//using System.Runtime.Serialization;

//namespace Sphere10.Framework {
//	/// <summary>
//	/// A list whose items are stored in a linked-list of clusters serialized over a stream similar in principle to a file storage system.
//	/// </summary>
//	/// <typeparam name="T">The type of item being stored in the list</typeparam>
//	public class ClusteredList<T> : ExtendedListDecorator<T>, ILoadable {
//		public event EventHandlerEx<object> Loading { add => InternalExtendedList.Loading += value; remove => InternalExtendedList.Loading -= value; }
//		public event EventHandlerEx<object> Loaded { add => InternalExtendedList.Loaded += value; remove => InternalExtendedList.Loaded -= value; }

//		/// <summary>
//		/// Constructs a ClusteredList of unbounded length. 
//		/// </summary>
//		/// <param name="clusterDataSize"></param>
//		/// <param name="stream"></param>
//		/// <param name="itemSerializer"></param>
//		/// <param name="itemComparer"></param>
//		/// <remarks>Will use an <see cref="DynamicClusteredList{T,TListing}"/> under the hood</remarks>
//		public ClusteredList(int clusterDataSize, Stream stream, IItemSerializer<T> itemSerializer, IEqualityComparer<T> itemComparer = null)
//			: base(
//				/*(new DynamicClusteredList<T, ItemListing>(
//					clusterDataSize,
//					stream,
//					itemSerializer,
//					new ItemListingSerializer(), 
//					itemComparer
//				)*/
//				new ExtendedList<T>()
//			) {
//			InternalExtendedList.ListingActivator = NewListingInstance;
//		}

//		/// <summary>
//		/// Constructs a ClusteredList of fixed length with slightly better performance. 
//		/// </summary>
//		/// <param name="clusterDataSize"></param>
//		/// <param name="maxItems"></param>
//		/// <param name="maxStorageBytes"></param>
//		/// <param name="stream"></param>
//		/// <param name="itemSerializer"></param>
//		/// <param name="itemComparer"></param>
//		/// <remarks>Will use an <see cref="StaticClusteredList{T,TListing}"/> under the hood</remarks>
//		public ClusteredList(int clusterDataSize, int maxItems, long maxStorageBytes, Stream stream, IItemSerializer<T> itemSerializer, IEqualityComparer<T> itemComparer = null)
//			: base(
//				/*new StaticClusteredList<T, ItemListing>(
//					clusterDataSize,
//					maxItems,
//					maxStorageBytes,
//					stream,
//					itemSerializer,
//					new ItemListingSerializer(),
//					itemComparer
//				)*/
//				new ExtendedList<T>()
//			) {
//			InternalExtendedList.ListingActivator = NewListingInstance;
//		}

//		//private ClusteredList(ClusteredListImplBase<T, ItemListing> listImpl) 
//		//	: base(listImpl) {
//		//}

//		public bool RequiresLoad => InternalExtendedList.RequiresLoad;

//		public void Load() => InternalExtendedList.Load();

//		internal new ClusteredListImplBase<T, ItemListing> InternalExtendedList => (ClusteredListImplBase<T, ItemListing>)base.InternalExtendedList;

//		private ItemListing NewListingInstance(object source, T item, int itemSizeBytes, int clusterStartIndex) 
//			=> new() {
//			Size = itemSizeBytes,
//			ClusterStartIndex = clusterStartIndex
//		};

//		[StructLayout(LayoutKind.Sequential)]
//		public struct ItemListing : IBlobListing {
//			public int ClusterStartIndex { get; set; }
//			public int Size { get; set; }
//		}

//		public class ItemListingSerializer : StaticSizeObjectSerializer<ItemListing> {

//			public ItemListingSerializer() 
//				: base(sizeof(int) + sizeof(int)) {
//			}

//			public override bool TrySerialize(ItemListing item, EndianBinaryWriter writer) {
//				writer.Write(item.ClusterStartIndex);
//				writer.Write(item.Size);
//				return true;
//			}

//			public override bool TryDeserialize(EndianBinaryReader reader, out ItemListing item) {
//				item = new ItemListing {
//					ClusterStartIndex = reader.ReadInt32(),
//					Size = reader.ReadInt32()
//				};
//				return true;
//			}
//		}
//	}

//}
