// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A list whose items are persisted over a stream via an <see cref="IClusteredStorage"/>.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class StreamMappedList<TItem> : SingularListBase<TItem>, IStreamMappedList<TItem> {

	public event EventHandlerEx<object> Loading { add => Storage.Loading += value; remove => Storage.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => Storage.Loaded += value; remove => Storage.Loaded -= value; }

	private int _version;

	public StreamMappedList(Stream rootStream, int clusterSize, IItemSerializer<TItem> itemSerializer = null,
							IEqualityComparer<TItem> itemComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, long recordKeySize = 0,
							long reservedRecords = 0, Endianness endianness = Endianness.LittleEndian, bool autoLoad = false)
		: this(new ClusteredStorage(rootStream, clusterSize, policy, recordKeySize, reservedRecords, endianness, autoLoad), itemSerializer, itemComparer) {
	}

	public StreamMappedList(ClusteredStorage storage, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null) {
		Guard.ArgumentNotNull(storage, nameof(storage));
		Storage = storage;
		ItemSerializer = itemSerializer ?? ItemSerializer<TItem>.Default;
		ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
		_version = 0;
	}

	public override long Count => Storage.Count - Storage.Header.ReservedRecords;

	public ClusteredStorage Storage { get; }

	public IItemSerializer<TItem> ItemSerializer { get; }

	public IEqualityComparer<TItem> ItemComparer { get; }

	public virtual bool RequiresLoad => Storage.RequiresLoad;

	public virtual void Load() => Storage.Load();

	public virtual Task LoadAsync() => Storage.LoadAsync();

	public override TItem Read(long index) {
		CheckIndex(index, true);
		return Storage.LoadItem(Storage.Header.ReservedRecords + index, ItemSerializer); // Index checking deferred to Storage
	}

	public override long IndexOfL(TItem item) {
		// TODO: if _storage keeps checksums, use that to quickly filter
		var listRecords = Count;
		for (var i = 0; i < listRecords; i++) {
			if (ItemComparer.Equals(item, Read(i)))
				return i;
		}
		return -1L;
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

	public override void Insert(long index, TItem item) {
		CheckIndex(index, true);
		using var _ = EnterInsertScope(index, item);
	}

	public ClusteredStreamScope EnterInsertScope(long index, TItem item) {
		// Index checking deferred to Storage
		UpdateVersion();
		return Storage.EnterSaveItemScope(index + Storage.Header.ReservedRecords, item, ItemSerializer, ListOperationType.Insert);
	}

	public override void Update(long index, TItem item) {
		CheckIndex(index, false);
		using var _ = EnterUpdateScope(index, item);
	}

	public ClusteredStreamScope EnterUpdateScope(long index, TItem item) {
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

	public override void RemoveAt(long index) {
		CheckIndex(index, false);
		UpdateVersion();
		Storage.Remove(Storage.Header.ReservedRecords + index);
	}

	public override void Clear() {
		UpdateVersion();
		Storage.Clear();
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
