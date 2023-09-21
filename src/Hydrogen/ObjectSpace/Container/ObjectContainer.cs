// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
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

namespace Hydrogen;

/// <summary>
/// A container that stores objects (and metadata) in a stream and provides a List-like interface.
/// This class uses a <see cref="StreamContainer"/> to store the objects and their metadata. This class is 
/// a primitive from which <see cref="StreamMappedList{TItem}"/>, <see cref="StreamMappedDictionary{TKey,TValue}"/> and
/// <see cref="StreamMappedHashSet{TItem}"/> build their functionality of. It it also what <see cref="ObjectSpace"/> are comprised of.
/// Conceptially, an <see cref="ObjectContainer"/> like "database table" that stores objects and metadata suitable for querying and lookup.
/// 
/// </summary>
public class ObjectContainer : ICriticalObject, ILoadable, IDisposable {
	public event EventHandlerEx<object> Loading { add => StreamContainer.Loading += value; remove => StreamContainer.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => StreamContainer.Loaded += value; remove => StreamContainer.Loaded -= value; }
	public event EventHandlerEx<object, long, object, ObjectContainerOperationType> PreItemOperation;
	public event EventHandlerEx<object, long, object, ObjectContainerOperationType> PostItemOperation;
	public event EventHandlerEx<object> Clearing;
	public event EventHandlerEx<object> Cleared;


	private readonly bool _preAllocateOptimization;
	private readonly Type _objectType;
	private readonly IDictionary<long, IObjectContainerMetaDataProvider> _metaDataProviders;
	public IItemSerializer ItemSerializer { get; }

	public ObjectContainer(Type objectType, StreamContainer streamContainer, IItemSerializer packedPackedSerializer, bool preAllocateOptimization) {
		Guard.ArgumentNotNull(objectType, nameof(objectType));
		Guard.ArgumentNotNull(streamContainer, nameof(streamContainer));
		Guard.ArgumentNotNull(packedPackedSerializer, nameof(packedPackedSerializer));
		StreamContainer = streamContainer;
		_objectType = objectType;
		ItemSerializer = packedPackedSerializer;
		_preAllocateOptimization = preAllocateOptimization;
		_metaDataProviders = new Dictionary<long, IObjectContainerMetaDataProvider>();
	}

	public ICriticalObject ParentCriticalObject { get => StreamContainer.ParentCriticalObject; set => StreamContainer.ParentCriticalObject = value; }

	public object Lock => StreamContainer.Lock;

	public bool IsLocked => StreamContainer.IsLocked;

	public bool Initialized => StreamContainer.Initialized;

	public bool RequiresLoad => StreamContainer.RequiresLoad;

	public StreamContainer StreamContainer { get; }

	IEnumerable<IObjectContainerMetaDataProvider> MetaDataProviders => _metaDataProviders.Values;

	public long Count => StreamContainer.Count - StreamContainer.Header.ReservedStreams;

	public bool OwnsStreamContainer { get; set; }

	#region ILoadable & IDisposable
	
	public void Load() => StreamContainer.Load();

	public Task LoadAsync() => StreamContainer.LoadAsync();

	public IDisposable EnterAccessScope() => StreamContainer.EnterAccessScope();

	public void Dispose() {
		foreach(var metaDataProvider in _metaDataProviders) {
			metaDataProvider.Value.Dispose();
		}
		_metaDataProviders.Clear();

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
		NotifyClearing();
		StreamContainer.Clear();
		NotifyCleared();
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
					var byteLength = ItemSerializer.Serialize(item, writer);
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
		var stream = StreamContainer.OpenWrite(streamIndex);
		try {
			NotifyPreItemOperation(index, default, ObjectContainerOperationType.Read);
			if (!stream.IsNull) {
				using var reader = new EndianBinaryReader(EndianBitConverter.For(StreamContainer.Endianness), stream);
				item = ItemSerializer.Deserialize(stream.Length, reader);
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

	#region Meta-Data Management
	
	internal void RegisterMetaDataProvider(IObjectContainerMetaDataProvider provider) {
		Guard.ArgumentNotNull(provider, nameof(provider));
		Guard.Against(StreamContainer.Initialized, "Cannot register meta-data provider after container has been initialized");
		Guard.Against(_metaDataProviders.ContainsKey(provider.ReservedStreamIndex), $"Meta-data provider for reserved stream {provider.ReservedStreamIndex} already registered");
		StreamContainer.RegisterInitAction(() => Guard.Ensure(StreamContainer.Header.ReservedStreams > provider.ReservedStreamIndex, $"No reserved stream {provider.ReservedStreamIndex} available for index"));
		_metaDataProviders.Add(provider.ReservedStreamIndex, provider);
	}

	internal T GetMetaDataProvider<T>(long reservedStream) where T : IObjectContainerMetaDataProvider {
		return (T)_metaDataProviders[reservedStream];
	}


	#endregion



	#region Event Notification

	private void NotifyPreItemOperation(long index, object item, ObjectContainerOperationType operationType) {
		CheckItemType(item);
		PreItemOperation?.Invoke(this, index, item, operationType);
	}

	private void NotifyPostItemOperation(long index, object item, ObjectContainerOperationType operationType) {
		CheckItemType(item);
		PostItemOperation?.Invoke(this, index, item, operationType);
	}

	private void NotifyClearing() {
		Clearing?.Invoke(this);
	}

	private void NotifyCleared() {
		Cleared?.Invoke(this);
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

