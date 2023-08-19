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
using Tools;

namespace Hydrogen;

/// <summary>
/// A list whose items are persisted over a stream via an <see cref="IClusteredStorage"/>.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class StreamMappedList<TItem> : SingularListBase<TItem>, IStreamMappedList<TItem> {

	public event EventHandlerEx<object> Loading { add => Streams.Loading += value; remove => Streams.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => Streams.Loaded += value; remove => Streams.Loaded -= value; }

	private int _version;
	private readonly bool _preAllocateOptimization;

	public StreamMappedList(Stream rootStream, int clusterSize, IItemSerializer<TItem> itemSerializer = null,
							IEqualityComparer<TItem> itemComparer = null, StreamContainerPolicy policy = StreamContainerPolicy.Default, long recordKeySize = 0,
							long reservedRecords = 0, Endianness endianness = Endianness.LittleEndian, bool autoLoad = false)
		: this(new StreamContainer(rootStream, clusterSize, policy, recordKeySize, reservedRecords, endianness, autoLoad), itemSerializer, itemComparer) {
	}

	public StreamMappedList(StreamContainer streams, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null) {
		Guard.ArgumentNotNull(streams, nameof(streams));
		Streams = streams;
		ItemSerializer = itemSerializer ?? ItemSerializer<TItem>.Default;
		ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
		_preAllocateOptimization = Streams.Policy.HasFlag(StreamContainerPolicy.FastAllocate);
		_version = 0;
	}

	public override long Count => Streams.Count - Streams.Header.ReservedStreams;

	public StreamContainer Streams { get; }

	public IItemSerializer<TItem> ItemSerializer { get; }

	public IEqualityComparer<TItem> ItemComparer { get; }

	public virtual bool RequiresLoad => Streams.RequiresLoad;

	public virtual void Load() => Streams.Load();

	public virtual Task LoadAsync() => Streams.LoadAsync();

	public override TItem Read(long index) {
		CheckIndex(index, true);
		return Streams.LoadItem(Streams.Header.ReservedStreams + index, ItemSerializer, _preAllocateOptimization); // Index checking deferred to Streams
	}

	public override long IndexOfL(TItem item) {
		// TODO: if _streams keeps checksums, use that to quickly filter
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

	public ClusteredStream EnterAddScope(TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return Streams.SaveItemAndReturnStream(Streams.Count, item, ItemSerializer, ListOperationType.Add, _preAllocateOptimization);
	}

	public override void Insert(long index, TItem item) {
		CheckIndex(index, true);
		using var _ = EnterInsertScope(index, item);
	}

	public ClusteredStream EnterInsertScope(long index, TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return Streams.SaveItemAndReturnStream(index + Streams.Header.ReservedStreams, item, ItemSerializer, ListOperationType.Insert, _preAllocateOptimization);
	}

	public override void Update(long index, TItem item) {
		CheckIndex(index, false);
		using var _ = EnterUpdateScope(index, item);
	}

	public ClusteredStream EnterUpdateScope(long index, TItem item) {
		// Index checking deferred to Streams
		UpdateVersion();
		return Streams.SaveItemAndReturnStream(index + Streams.Header.ReservedStreams, item, ItemSerializer, ListOperationType.Update, _preAllocateOptimization);
	}

	public override bool Remove(TItem item) {
		var index = IndexOf(item);
		if (index >= 0) {
			UpdateVersion();
			Streams.Remove(index + Streams.Header.ReservedStreams);

			return true;
		}
		return false;
	}

	public override void RemoveAt(long index) {
		CheckIndex(index, false);
		UpdateVersion();
		Streams.Remove(Streams.Header.ReservedStreams + index);
	}

	public override void Clear() {
		UpdateVersion();
		Streams.Clear();
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
