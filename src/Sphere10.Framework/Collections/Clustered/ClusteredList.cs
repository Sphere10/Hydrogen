using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sphere10.Framework {

	/// <summary>
	/// A list whose items are persisted via an <see cref="IClusteredStorage"/>.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class ClusteredList<TItem> : SingularListBase<TItem>, IClusteredList<TItem> {
		private int _version;

		public ClusteredList(Stream rootStream, int clusterSize, IItemSerializer<TItem> itemSerializer, IEqualityComparer<TItem> itemComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, int recordKeySize = 0,  Endianness endianness = Endianness.LittleEndian)
			: this(new ClusteredStorage(rootStream, clusterSize, policy, recordKeySize, endianness), itemSerializer, itemComparer) {
		}

		public ClusteredList(IClusteredStorage storage, IItemSerializer<TItem> itemSerializer, IEqualityComparer<TItem> itemComparer = null) {
			Guard.ArgumentNotNull(storage, nameof(storage));
			Guard.ArgumentNotNull(itemSerializer, nameof(itemSerializer));
			Storage = storage;
			ItemSerializer = itemSerializer;
			ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
			_version = 0;
		}

		public override int Count => Storage.Count;

		public IClusteredStorage Storage { get; }

		public IItemSerializer<TItem> ItemSerializer { get; }

		public IEqualityComparer<TItem> ItemComparer { get; }

		public override TItem Read(int index) 
			=> Storage.LoadItem(index, ItemSerializer); // Index checking deferred to Storage

		public override int IndexOf(TItem item) {
			// TODO: if _storage keeps checksums, use that to quickly filter
			for (var i = 0; i < Storage.Count; i++) {
				if (ItemComparer.Equals(item, Read(i)))
					return i;
			}
			return -1;
		}

		public override bool Contains(TItem item) => IndexOf(item) != -1;

		public override void Add(TItem item) {
			using var _ = EnterAddScope(item);
		}

		public ClusteredStreamScope EnterAddScope(TItem item) {
			// Index checking deferred to Storage
			UpdateVersion();
			return Storage.EnterSaveItemScope(Storage.Count, item, ItemSerializer, ListOperationType.Add);
		}
		
		public override void Insert(int index, TItem item) {
			using var _ = EnterInsertScope(index, item);
		}

		public ClusteredStreamScope EnterInsertScope(int index, TItem item) {
			// Index checking deferred to Storage
			UpdateVersion();
			return Storage.EnterSaveItemScope(index, item, ItemSerializer, ListOperationType.Insert);
		}

		public override void Update(int index, TItem item) {
			using var _ = EnterUpdateScope(index, item);
		}

		public ClusteredStreamScope EnterUpdateScope(int index, TItem item) {
			// Index checking deferred to Storage
			UpdateVersion();
			return Storage.EnterSaveItemScope(index, item, ItemSerializer, ListOperationType.Update);
		}

		public override bool Remove(TItem item) {
			var index = IndexOf(item);
			if (index >= 0) {
				UpdateVersion();
				Storage.Remove(index);
				
				return true;
			}
			return false;
		}

		public override void RemoveAt(int index) 
			=> Storage.Remove(index);

		public override void Clear() {
			Storage.Clear();
			UpdateVersion();
		}

		public override void CopyTo(TItem[] array, int arrayIndex) {
			Guard.ArgumentNotNull(array, nameof(array));
			Guard.ArgumentInRange(arrayIndex, 0, Math.Max(0, array.Length - 1), nameof(array));
			var itemsToCopy = Math.Min(Storage.Count, array.Length - arrayIndex);
			for (var i = 0; i < itemsToCopy; i++)
				array[i + arrayIndex] = Read(i);
		}

		public override IEnumerator<TItem> GetEnumerator() {
			var version = _version;
			for (var i = 0; i < Storage.Count; i++) {
				if (_version != version)
					throw new InvalidOperationException("Collection was mutated during enumeration");
				yield return Read(i);
			}
		}
	

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateVersion() => Interlocked.Increment(ref _version);

	}

}

