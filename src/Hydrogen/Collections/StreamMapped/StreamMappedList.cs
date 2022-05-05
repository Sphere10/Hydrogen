﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Hydrogen {

	/// <summary>
	/// A list whose items are persisted over a stream via an <see cref="IClusteredStorage"/>.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class StreamMappedList<TItem> : SingularListBase<TItem>, IStreamMappedList<TItem> {
		private int _version;

		public StreamMappedList(Stream rootStream, int clusterSize, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, int recordKeySize = 0, int reservedRecords = 0, Endianness endianness = Endianness.LittleEndian)
			: this(new ClusteredStorage(rootStream, clusterSize, policy, recordKeySize, reservedRecords, endianness), itemSerializer, itemComparer) {
		}

		public StreamMappedList(IClusteredStorage storage, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null) {
			Guard.ArgumentNotNull(storage, nameof(storage));
			Storage = storage;
			ItemSerializer = itemSerializer ?? ItemSerializer<TItem>.Default;
			ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
			_version = 0;
		}

		public override int Count => Storage.Count - Storage.Header.ReservedRecords;

		public IClusteredStorage Storage { get; }

		public IItemSerializer<TItem> ItemSerializer { get; }

		public IEqualityComparer<TItem> ItemComparer { get; }

		public override TItem Read(int index) {
			CheckIndex(index, true);
			return Storage.LoadItem(Storage.Header.ReservedRecords + index, ItemSerializer); // Index checking deferred to Storage
		}

		public override int IndexOf(TItem item) {
			// TODO: if _storage keeps checksums, use that to quickly filter
			var listRecords = Count;
			for (var i = 0; i < listRecords; i++) {
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
			CheckIndex(index, true);
			using var _ = EnterInsertScope(index, item);
		}

		public ClusteredStreamScope EnterInsertScope(int index, TItem item) {
			// Index checking deferred to Storage
			UpdateVersion();
			return Storage.EnterSaveItemScope(index + Storage.Header.ReservedRecords, item, ItemSerializer, ListOperationType.Insert);
		}

		public override void Update(int index, TItem item) {
			CheckIndex(index, false);
			using var _ = EnterUpdateScope(index, item);
		}

		public ClusteredStreamScope EnterUpdateScope(int index, TItem item) {
			// Index checking deferred to Storage
			UpdateVersion();
			return Storage.EnterSaveItemScope(index + Storage.Header.ReservedRecords, item, ItemSerializer, ListOperationType.Update);
		}

		public override bool Remove(TItem item) {
			var index = IndexOf(item);
			if (index >= 0) {
				UpdateVersion();
				Storage.Remove(index + Storage.Header.ReservedRecords);
				
				return true;
			}
			return false;
		}

		public override void RemoveAt(int index) {
			CheckIndex(index, false);
			Storage.Remove(Storage.Header.ReservedRecords + index);
		}

		public override void Clear() {
			Storage.Clear();
			UpdateVersion();
		}

		public override void CopyTo(TItem[] array, int arrayIndex) {
			Guard.ArgumentNotNull(array, nameof(array));
			Guard.ArgumentInRange(arrayIndex, 0, Math.Max(0, array.Length - 1), nameof(array));
			var itemsToCopy = Math.Min(Count, array.Length - arrayIndex);
			for (var i = 0; i < itemsToCopy; i++)
				array[i + arrayIndex] = Read(i);
		}

		public override IEnumerator<TItem> GetEnumerator() {
			var version = _version;
			var count = Count;
			for (var i = 0; i < count; i++) {
				if (_version != version)
					throw new InvalidOperationException("Collection was mutated during enumeration");
				yield return Read(i);
			}
		}
	

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateVersion() => Interlocked.Increment(ref _version);

	}

}

