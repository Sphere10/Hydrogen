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
	public event EventHandlerEx<object> Loading { add => ObjectContainer.Loading += value; remove => ObjectContainer.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => ObjectContainer.Loaded += value; remove => ObjectContainer.Loaded -= value; }

	private int _version;
	private readonly ObjectContainerIndex<TItem, int> _checksumIndex;  // null checksum means item at index

	public StreamMappedList(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long reservedStreams = 0,
		long checksumIndexStreamIndex = 0,
		Endianness endianness = Endianness.LittleEndian, 
		bool autoLoad = false
	) : this(
		new StreamContainer(
			rootStream, 
			clusterSize,
			policy,
			reservedStreams,
			endianness, 
			false
		),
		itemSerializer, 
		itemComparer, 
		itemChecksummer,
		checksumIndexStreamIndex,
		autoLoad
	)  {
		if (itemChecksummer != null)
			Guard.Ensure(reservedStreams > checksumIndexStreamIndex, $"No reserved stream available for checksum index {checksumIndexStreamIndex}");

		ObjectContainer.OwnsStreamContainer = true;
	}

	public StreamMappedList(
		StreamContainer streamContainer,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		long checksumIndexStreamIndex = 0, 
		bool autoLoad = false
	) : this(
		new ObjectContainer<TItem>(
			streamContainer, 
			itemSerializer, 
			streamContainer.Policy.HasFlag(StreamContainerPolicy.FastAllocate)
		), 
		itemSerializer, 
		itemComparer, 
		itemChecksummer, 
		checksumIndexStreamIndex,
		autoLoad
	) {
		OwnsContainer = true;
	}

	public StreamMappedList(
		ObjectContainer<TItem> objectContainer,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		long checksumIndexStreamIndex = 0,
		bool autoLoad = false
	) {
		Guard.ArgumentNotNull(objectContainer, nameof(objectContainer));
		ObjectContainer = objectContainer;
		ItemSerializer = itemSerializer ?? ItemSerializer<TItem>.Default;
		ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
		if (itemChecksummer != null) {
			ObjectContainer.StreamContainer.RegisterInitAction( () => Guard.Ensure(objectContainer.StreamContainer.Header.ReservedStreams > checksumIndexStreamIndex, $"No reserved stream {checksumIndexStreamIndex} available for checksum index"));
			Guard.EnsureNotThrows(()=> itemChecksummer.CalculateChecksum(default), $"ItemChecksummer cannot calculate checksum for default values of {typeof(TItem).Name}");
			// ensure that default integer values are substituted with 1, since 0 is reserved for "no checksum"
			ItemChecksummer = itemChecksummer.WithSubstitution(default, 1); 
		}
		_version = 0;

		_checksumIndex =
			itemChecksummer != null ?
			new ObjectContainerIndex<TItem, int>(
				ObjectContainer,
				checksumIndexStreamIndex,
				ItemChecksummer.CalculateChecksum,
				EqualityComparer<int>.Default,
				PrimitiveSerializer<int>.Instance) : 
			null;

		if (autoLoad && RequiresLoad)
			Load();
	}

	public override long Count => ObjectContainer.Count;

	public ObjectContainer<TItem> ObjectContainer { get; }

	public IItemSerializer<TItem> ItemSerializer { get; }

	public IEqualityComparer<TItem> ItemComparer { get; }

	public IItemChecksummer<TItem> ItemChecksummer { get; }

	public bool OwnsContainer { get; set; }

	public virtual bool RequiresLoad => ObjectContainer.RequiresLoad;

	public virtual void Load() => ObjectContainer.Load();

	public virtual Task LoadAsync() => ObjectContainer.LoadAsync();

	public virtual void Dispose() {
		_checksumIndex?.Dispose();
		if (OwnsContainer)
			ObjectContainer.Dispose();
	}

	public override TItem Read(long index) {
		CheckIndex(index, true);
		return ObjectContainer.LoadItem(index);
	}

	public override long IndexOfL(TItem item) {
		var indicesToCheck =
			_checksumIndex != null ?
			_checksumIndex.Lookup[ItemChecksummer.CalculateChecksum(item)] :
			Tools.Collection.RangeL(0L, Count);

		foreach (var index in indicesToCheck) {
			if (ItemComparer.Equals(item, Read(index)))
				return index;
		}
		return -1L;
	}

	public override bool Contains(TItem item) => IndexOf(item) != -1;

	public override void Add(TItem item) {
		using var _ = EnterAddScope(item);
	}

	public override void Insert(long index, TItem item) {
		CheckIndex(index, true);
		using var _ = EnterInsertScope(index, item);
	}

	public override void Update(long index, TItem item) {
		CheckIndex(index, false);
		using var _ = EnterUpdateScope(index, item);
	}

	public override bool Remove(TItem item) {
		var index = IndexOf(item);
		if (index >= 0) {
			UpdateVersion();
			ObjectContainer.RemoveItem(index);
			return true;
		}
		return false;
	}

	public override void RemoveAt(long index) {
		CheckIndex(index, false);
		UpdateVersion();
		ObjectContainer.RemoveItem(index);
	}

	public override void Clear() {
		UpdateVersion();
		ObjectContainer.Clear();
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

	protected ClusteredStream EnterAddScope(TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return ObjectContainer.SaveItemAndReturnStream(ObjectContainer.Count, item, ObjectContainerOperationType.Add);
	}

	protected ClusteredStream EnterInsertScope(long index, TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return ObjectContainer.SaveItemAndReturnStream(index, item, ObjectContainerOperationType.Insert);
	}

	protected ClusteredStream EnterUpdateScope(long index, TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return ObjectContainer.SaveItemAndReturnStream(index, item, ObjectContainerOperationType.Update);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateVersion() => Interlocked.Increment(ref _version);

}
