//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;

//namespace Sphere10.Framework {

//	public class StreamedList<TItem, TListing> : SingularListBase<TItem>, ILoadable where TListing : IBlobListing {
//		public event EventHandlerEx<object> Loading;
//		public event EventHandlerEx<object> Loaded;

//		private readonly BlobContainer<TListing> _blobContainer;
//		private readonly IItemSerializer<TItem> _itemSerializer;
//		private IEqualityComparer<TItem> _itemComparer;
//		private readonly Endianness _endianness;

//		public StreamedList(Stream stream, int clusterSize, IItemSerializer<TItem> itemSerializer, ListingActivator<TItem, TListing> listingActivator, IItemSerializer<TListing> listingSerializer, IEqualityComparer<TItem> itemComparer = null, Endianness endianness = Endianness.LittleEndian) 
//		: this(stream, clusterSize, itemSerializer, (item, bytes, index) => listingActivator(itemSerializer.Deserialize(item, endianness), bytes, index), listingSerializer, itemComparer, endianness) {
//		}

//		public StreamedList(Stream stream, int clusterSize, IItemSerializer<TItem> itemSerializer, ListingActivator<byte[], TListing> listingActivator, IItemSerializer<TListing> listingSerializer, IEqualityComparer<TItem> itemComparer = null, Endianness endianness = Endianness.LittleEndian) {
//			_blobContainer = new BlobContainer<TListing>(stream, clusterSize, listingActivator, listingSerializer, endianness);
//			_itemSerializer = itemSerializer;
//			_itemComparer = itemComparer;
//			_endianness = endianness;
//		}

//		public override int Count => _blobContainer.Count;

//		public bool RequiresLoad => _blobContainer.RequiresLoad;

//		public void Load() => _blobContainer.Load();

//		public override void Insert(int index, TItem item) => _blobContainer.Insert(0, _itemSerializer.Serialize(item, _endianness));

//		public override void RemoveAt(int index) => _blobContainer.RemoveAt(index);

//		public override TItem Read(int index) => _itemSerializer.Deserialize(_blobContainer.ReadAll(index), _endianness);

//		public override int IndexOf(TItem item) {
//			// use listings hashcode to filter potential candidates
//			throw new System.NotImplementedException();
//		}

//		public override bool Contains(TItem item) {
//			// use listings hashcode to filter potential candidates
//			throw new System.NotImplementedException();
//		}

//		public override void Add(TItem item) => _blobContainer.Add(_itemSerializer.Serialize(item, _endianness));

//		public override void Update(int index, TItem item) => _blobContainer.Update(index, _itemSerializer.Serialize(item, _endianness));

//		public override bool Remove(TItem item) {
//			// use hashcode to filter potential candidates
//			throw new System.NotImplementedException();
//		}

//		public override void Clear() => _blobContainer.Clear();

//		public override void CopyTo(TItem[] array, int arrayIndex) {
//			Guard.ArgumentNotNull(array, nameof(array));
//			Guard.ArgumentInRange(arrayIndex, 0, Math.Max(0, array.Length - 1), nameof(array));
//			var itemsToCopy = Math.Min(_blobContainer.Count, array.Length - arrayIndex);
//			for (var i = 0; i < itemsToCopy; i++)
//				array[i + arrayIndex] = Read(i);
//		}

//		public override IEnumerator<TItem> GetEnumerator() {
//			throw new NotImplementedException();
//		}

//	}

//}
