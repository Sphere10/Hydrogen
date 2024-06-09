// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Tools;

namespace Hydrogen;

/// <summary>
/// A objectStream that stores objects (and metadata) in a stream and provides a List-like interface.
/// This class uses a <see cref="Streams"/> to store the objects and their metadata. This class is 
/// a primitive from which <see cref="StreamMappedList{TItem}"/>, <see cref="StreamMappedDictionary{TKey,TValue}"/> and
/// <see cref="StreamMappedHashSet{TItem}"/> build their functionality of. It it also what <see cref="ObjectSpace"/> are comprised of.
/// Conceptually, an <see cref="ObjectStream"/> like "database table" that stores objects and metadata suitable for querying and lookup.
/// 
/// </summary>
public class ObjectStream : SyncLoadableBase, ICriticalObject, IDisposable {
	public event EventHandlerEx<long, object, ObjectStreamOperationType> PreItemOperation;
	public event EventHandlerEx<long, object, ObjectStreamOperationType> PostItemOperation;
	public event EventHandlerEx Clearing { add => Streams.Clearing += value; remove => Streams.Clearing -= value; }
	public event EventHandlerEx Cleared { add => Streams.Cleared += value; remove => Streams.Cleared -= value; }

	private readonly bool _preAllocateOptimization;

	public ObjectStream(Type objectType, ClusteredStreams streams, IItemSerializer objectSerializer, bool preAllocateOptimization) {
		Guard.ArgumentNotNull(objectType, nameof(objectType));
		Guard.ArgumentNotNull(streams, nameof(streams));
		Guard.ArgumentNotNull(objectSerializer, nameof(objectSerializer));
		Streams = streams;
		ItemType = objectType;
		ItemSerializer = objectSerializer;
		_preAllocateOptimization = preAllocateOptimization;
		streams.Loading += _ => NotifyLoading();
		streams.Loaded += _ => NotifyLoaded();
	}

	public ICriticalObject ParentCriticalObject { get => Streams.ParentCriticalObject; set => Streams.ParentCriticalObject = value; }

	public object Lock => Streams.Lock;

	public bool IsLocked => Streams.IsLocked;

	public override bool RequiresLoad => Streams.RequiresLoad;

	public ClusteredStreams Streams { get; }

	public Type ItemType { get; }

	public long Count => Streams.Count - Streams.Header.ReservedStreams;

	public bool OwnsStreams { get; set; }

	public IItemSerializer ItemSerializer { get; }

	#region ILoadable & IDisposable
	
	protected override void LoadInternal() {
		if (Streams.RequiresLoad)
			Streams.Load();
	}

	public IDisposable EnterAccessScope() => Streams.EnterAccessScope();

	public void Dispose() {
		if (OwnsStreams)
			Streams.Dispose();
	}

	#endregion

	#region Item management

	public ClusteredStreamDescriptor GetItemDescriptor(long index) {
		using var _ = Streams.EnterAccessScope();
		return Streams.GetStreamDescriptor(index + Streams.Header.ReservedStreams);
	}

	public void SaveItem(long index, object item, ObjectStreamOperationType operationType) {
		Guard.Argument(operationType != ObjectStreamOperationType.Remove, nameof(operationType), "Remove operation not supported in this method");
		CheckItemType(item);
		using var _ = Streams.EnterAccessScope();
		using var stream = SaveItemAndReturnStream(index, item, operationType);
	}

	public object LoadItem(long index) {
		using var _ = Streams.EnterAccessScope();
		using var stream = LoadItemAndReturnStream(index, out var item);
		CheckItemType(item);
		return item;
	}

	public byte[] GetItemBytes(long index) {
		using var _ = Streams.EnterAccessScope();
		using var stream = Streams.OpenRead(index + Streams.Header.ReservedStreams);
		if (stream.IsNull || stream.IsReaped)
			return null;
		return stream.ToArray();
	}

	public void RemoveItem(long index) {
		using var _ = Streams.EnterAccessScope();
		NotifyPreItemOperation(index, null, ObjectStreamOperationType.Remove);
		Streams.Remove(index + Streams.Header.ReservedStreams);
		NotifyPostItemOperation(index, null, ObjectStreamOperationType.Remove);
	}

	public void ReapItem(long index) {
		using var _ = Streams.EnterAccessScope();
		NotifyPreItemOperation(index, null, ObjectStreamOperationType.Reap);
		using (var stream = Streams.OpenWrite(index + Streams.Header.ReservedStreams)) {
			stream.SetLength(0);
			stream.IsNull = false;
			stream.IsReaped = true;
		}
		NotifyPostItemOperation(index, null, ObjectStreamOperationType.Reap);
	}

	public bool IsReaped(long index) => GetItemDescriptor(index).Traits.HasFlag(ClusteredStreamTraits.Reaped);

	public void Clear() {
		using var _ = Streams.EnterAccessScope();
		Streams.Clear();
	}

	internal ClusteredStream SaveItemAndReturnStream(long index, object item, ObjectStreamOperationType operationType) {
		Guard.ArgumentGTE(index, 0, nameof(index));
		Guard.Argument(operationType != ObjectStreamOperationType.Remove, nameof(operationType), "Remove operation not supported in this method");
		CheckItemType(item);
	
		// Pre-notify before opening any streams
		NotifyPreItemOperation(index, item, operationType);
		
		var streamIndex = index + Streams.Header.ReservedStreams;
		// initialized and re-entrancy checks done by one of below called methods
		var stream = operationType switch {
			ObjectStreamOperationType.Add => Streams.Add(),
			ObjectStreamOperationType.Update => Streams.OpenWrite(streamIndex),
			ObjectStreamOperationType.Insert => Streams.Insert(streamIndex),
			_ => throw new ArgumentException($@"List operation type '{operationType}' not supported", nameof(operationType)),
		};
		try {
			stream.IsReaped = false;
			using var writer = new EndianBinaryWriter(EndianBitConverter.For(Streams.Endianness), stream);
			if (item != null) {
				stream.IsNull = false;
				if (_preAllocateOptimization) {
					// pre-setting the stream length before serialization improves performance since it avoids
					// re-allocating fragmented stream on individual properties of the serialized item
					var expectedSize = ItemSerializer.PackedCalculateSize(item);
					stream.SetLength(expectedSize);
					ItemSerializer.Serialize(item, writer);
				} else {
					var byteLength = ItemSerializer.PackedSerializeReturnSize(item, writer);
					stream.SetLength(byteLength);
				}

			} else {
				stream.IsNull = true;
				stream.SetLength(0); // open descriptor will save when closed
			}

			// Ensure post-operation fired when stream owner disposes 
			stream.AddFinalizer(() => NotifyPostItemOperation(index, item, operationType));
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

	internal ClusteredStream LoadItemAndReturnStream(long index, out object item) {
		var streamIndex = index + Streams.Header.ReservedStreams;
		// initialized and re-entrancy checks done by Open
		var stream = Streams.OpenRead(streamIndex);
		Guard.Ensure(!stream.IsReaped, $"Item at index {index} has been reaped");
		try {
			NotifyPreItemOperation(index, default, ObjectStreamOperationType.Read);
			if (!stream.IsNull) {
				using var reader = new EndianBinaryReader(EndianBitConverter.For(Streams.Endianness), stream);
				item = ItemSerializer.PackedDeserialize(reader);
				CheckItemType(item);
			} else {
				item = default;
			}
			var capturedItem = item;
			stream.AddFinalizer( () => NotifyPostItemOperation(index, capturedItem, ObjectStreamOperationType.Read));
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

	#endregion
	
	#region Event Notification

	private void NotifyPreItemOperation(long index, object item, ObjectStreamOperationType operationType) {
		CheckItemType(item);
		PreItemOperation?.Invoke(index, item, operationType);
	}

	private void NotifyPostItemOperation(long index, object item, ObjectStreamOperationType operationType) {
		CheckItemType(item);
		PostItemOperation?.Invoke(index, item, operationType);
	}


	#endregion

	private void CheckItemType(object item) {
		if (item != null)
			CheckType(item.GetType());
	}

	private void CheckType(Type type) {
		if (type != ItemType)
			throw new InvalidOperationException($@"This objectStream does not support type '{type}'");
	}
	
}