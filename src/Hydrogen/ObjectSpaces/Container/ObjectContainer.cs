﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// A container that stores objects (and metadata) in a stream and provides a List-like interface.
/// This class uses a <see cref="StreamContainer"/> to store the objects and their metadata. This class is 
/// a primitive from which <see cref="StreamMappedList{TItem}"/>, <see cref="StreamMappedDictionary{TKey,TValue}"/> and
/// <see cref="StreamMappedHashSet{TItem}"/> build their functionality of. It it also what <see cref="ObjectSpace"/> are comprised of.
/// Conceptually, an <see cref="ObjectContainer"/> like "database table" that stores objects and metadata suitable for querying and lookup.
/// 
/// </summary>
public class ObjectContainer : SyncLoadableBase, ICriticalObject, IDisposable {
	public event EventHandlerEx<long, object, ObjectContainerOperationType> PreItemOperation;
	public event EventHandlerEx<long, object, ObjectContainerOperationType> PostItemOperation;
	public event EventHandlerEx Clearing { add => StreamContainer.Clearing += value; remove => StreamContainer.Clearing -= value; }
	public event EventHandlerEx Cleared { add => StreamContainer.Cleared += value; remove => StreamContainer.Cleared -= value; }


	private readonly bool _preAllocateOptimization;
	private readonly Type _objectType;
	private readonly IDictionary<long, IObjectContainerAttachment> _attachments;
	

	public ObjectContainer(Type objectType, StreamContainer streamContainer, IItemSerializer packedPackedSerializer, bool preAllocateOptimization) {
		Guard.ArgumentNotNull(objectType, nameof(objectType));
		Guard.ArgumentNotNull(streamContainer, nameof(streamContainer));
		Guard.ArgumentNotNull(packedPackedSerializer, nameof(packedPackedSerializer));
		StreamContainer = streamContainer;
		_objectType = objectType;
		ItemSerializer = packedPackedSerializer;
		_preAllocateOptimization = preAllocateOptimization;
		_attachments = new Dictionary<long, IObjectContainerAttachment>();
		streamContainer.Loading += _ => NotifyLoading();
		streamContainer.Loaded += _ => NotifyLoaded();
	}

	public ICriticalObject ParentCriticalObject { get => StreamContainer.ParentCriticalObject; set => StreamContainer.ParentCriticalObject = value; }

	public object Lock => StreamContainer.Lock;

	public bool IsLocked => StreamContainer.IsLocked;

	public override bool RequiresLoad => StreamContainer.RequiresLoad;

	public StreamContainer StreamContainer { get; }

	IEnumerable<IObjectContainerAttachment> Attachments => _attachments.Values;

	public long Count => StreamContainer.Count - StreamContainer.Header.ReservedStreams;

	public bool OwnsStreamContainer { get; set; }

	public IItemSerializer ItemSerializer { get; }

	#region ILoadable & IDisposable
	
	protected override void LoadInternal() {
		if (StreamContainer.RequiresLoad)
			StreamContainer.Load();

		foreach(var attachment in Attachments)
			attachment.Attach();
	}

	public IDisposable EnterAccessScope() => StreamContainer.EnterAccessScope();

	public void Dispose() {
		foreach(var attachment in _attachments.Values)
			attachment.Detach();
		_attachments.Clear();

		if (OwnsStreamContainer)
			StreamContainer.Dispose();
	}

	#endregion

	#region Item management

	public ClusteredStreamDescriptor GetItemDescriptor(long index) {
		using var _ = StreamContainer.EnterAccessScope();
		return StreamContainer.GetStreamDescriptor(index + StreamContainer.Header.ReservedStreams);
	}

	public void SaveItem(long index, object item, ObjectContainerOperationType operationType) {
		Guard.Argument(operationType != ObjectContainerOperationType.Remove, nameof(operationType), "Remove operation not supported in this method");
		CheckItemType(item);
		using var _ = StreamContainer.EnterAccessScope();
		using var stream = SaveItemAndReturnStream(index, item, operationType);
	}

	public object LoadItem(long index) {
		using var _ = StreamContainer.EnterAccessScope();
		using var stream = LoadItemAndReturnStream(index, out var item);
		CheckItemType(item);
		return item;
	}

	public byte[] GetItemBytes(long index) {
		using var _ = StreamContainer.EnterAccessScope();
		using var stream = StreamContainer.OpenRead(index + StreamContainer.Header.ReservedStreams);
		if (stream.IsNull || stream.IsReaped)
			return null;
		return stream.ToArray();
	}

	public void RemoveItem(long index) {
		using var _ = StreamContainer.EnterAccessScope();
		NotifyPreItemOperation(index, null, ObjectContainerOperationType.Remove);
		StreamContainer.Remove(index + StreamContainer.Header.ReservedStreams);
		NotifyPostItemOperation(index, null, ObjectContainerOperationType.Remove);
	}

	public void ReapItem(long index) {
		using var _ = StreamContainer.EnterAccessScope();
		NotifyPreItemOperation(index, null, ObjectContainerOperationType.Reap);
		using (var stream = StreamContainer.OpenWrite(index + StreamContainer.Header.ReservedStreams)) {
			stream.SetLength(0);
			stream.IsNull = false;
			stream.IsReaped = true;
		}
		NotifyPostItemOperation(index, null, ObjectContainerOperationType.Reap);
	}

	public void Clear() {
		_attachments.Values.ForEach(x => x.Detach());
		StreamContainer.Clear();
		_attachments.Values.ForEach(x => x.Attach());
	}

	internal ClusteredStream SaveItemAndReturnStream(long index, object item, ObjectContainerOperationType operationType) {
		Guard.ArgumentGTE(index, 0, nameof(index));
		Guard.Argument(operationType != ObjectContainerOperationType.Remove, nameof(operationType), "Remove operation not supported in this method");
		CheckItemType(item);

		var streamIndex = index + StreamContainer.Header.ReservedStreams;
		// initialized and re-entrancy checks done by one of below called methods
		var stream = operationType switch {
			ObjectContainerOperationType.Add => StreamContainer.Add(),
			ObjectContainerOperationType.Update => StreamContainer.OpenWrite(streamIndex),
			ObjectContainerOperationType.Insert => StreamContainer.Insert(streamIndex),
			_ => throw new ArgumentException($@"List operation type '{operationType}' not supported", nameof(operationType)),
		};
		try {
			stream.IsReaped = false;
			using var writer = new EndianBinaryWriter(EndianBitConverter.For(StreamContainer.Endianness), stream);
			if (item != null) {
				NotifyPreItemOperation(index, item, operationType);
				stream.IsNull = false;
				if (_preAllocateOptimization) {
					// pre-setting the stream length before serialization improves performance since it avoids
					// re-allocating fragmented stream on individual properties of the serialized item
					var expectedSize = ItemSerializer.CalculateSize(item);
					stream.SetLength(expectedSize);
					ItemSerializer.Serialize(item, writer);
				} else {
					var byteLength = ItemSerializer.SerializeReturnSize(item, writer);
					stream.SetLength(byteLength);
				}

			} else {
				stream.IsNull = true;
				stream.SetLength(0); // open descriptor will save when closed
			}
			stream.AddFinalizer(() => NotifyPostItemOperation(index, item, operationType));
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

	internal ClusteredStream LoadItemAndReturnStream(long index, out object item) {
		var streamIndex = index + StreamContainer.Header.ReservedStreams;
		// initialized and re-entrancy checks done by Open
		var stream = StreamContainer.OpenRead(streamIndex);
		Guard.Ensure(!stream.IsReaped, $"Item at index {index} has been reaped");
		try {
			NotifyPreItemOperation(index, default, ObjectContainerOperationType.Read);
			if (!stream.IsNull) {
				using var reader = new EndianBinaryReader(EndianBitConverter.For(StreamContainer.Endianness), stream);
				item = ItemSerializer.Deserialize(reader);
				CheckItemType(item);
			} else {
				item = default;
			}
			var capturedItem = item;
			stream.AddFinalizer( () => NotifyPostItemOperation(index, capturedItem, ObjectContainerOperationType.Read));
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

	#endregion

	#region Attachment Management
	
	internal void RegisterAttachment(IObjectContainerAttachment attachment) {
		Guard.ArgumentNotNull(attachment, nameof(attachment));
		//Guard.Against(StreamContainer.Initialized, "Cannot register meta-data provider after container has been initialized");
		Guard.Against(_attachments.ContainsKey(attachment.ReservedStreamIndex), $"Meta-data provider for reserved stream {attachment.ReservedStreamIndex} already registered");
		_attachments.Add(attachment.ReservedStreamIndex, attachment);

		// If container is already loaded, then attach now
		if (!RequiresLoad)
			attachment.Attach();
	}

	internal T GetAttachment<T>(long reservedStream) where T : IObjectContainerAttachment {
		return (T)_attachments[reservedStream];
	}

	#endregion

	#region Event Notification

	private void NotifyPreItemOperation(long index, object item, ObjectContainerOperationType operationType) {
		CheckItemType(item);
		PreItemOperation?.Invoke(index, item, operationType);
	}

	private void NotifyPostItemOperation(long index, object item, ObjectContainerOperationType operationType) {
		CheckItemType(item);
		PostItemOperation?.Invoke(index, item, operationType);
	}


	#endregion

	private void CheckItemType(object item) {
		if (item != null)
			CheckType(item.GetType());
	}

	private void CheckType(Type type) {
		if (type != _objectType)
			throw new InvalidOperationException($@"This container does not support type '{type}'");
	}


	
}