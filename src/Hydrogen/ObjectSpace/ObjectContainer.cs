using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Hydrogen;


/// <summary>
/// A container that stores objects in a stream using a <see cref="StreamContainer"/>. This can also maintain
/// object metadata such as indexes, timestamps, merkle-trees, etc. This class is used by collections which
/// store their items in a stream.
/// </summary>
public class ObjectContainer : ILoadable {
	public event EventHandlerEx<object> Loading;
	public event EventHandlerEx<object> Loaded;
	public event EventHandlerEx<object, long, object, ListOperationType> PreItemOperation;
	public event EventHandlerEx<object, long, object, ListOperationType> PostItemOperation;
	public event EventHandlerEx<object> Clearing;
	public event EventHandlerEx<object> Cleared;

	private bool _loaded;
	private readonly bool _preAllocateOptimization;
	private readonly IItemSerializer<object> _packedSerializer;
	private readonly Type _objectType;

	public ObjectContainer(Type objectType, StreamContainer streamContainer, IItemSerializer<object> packedPackedSerializer, bool preAllocateOptimization, bool autoLoad = false) {
		Guard.ArgumentNotNull(objectType, nameof(objectType));
		Guard.ArgumentNotNull(streamContainer, nameof(streamContainer));
		Guard.ArgumentNotNull(packedPackedSerializer, nameof(packedPackedSerializer));
		StreamContainer = streamContainer;
		_objectType = objectType;
		_packedSerializer = packedPackedSerializer;
		_preAllocateOptimization = preAllocateOptimization;
		_loaded = false;
//		StreamContainer.RegisterInitAction(() => Guard.Ensure(StreamContainer.Header.ReservedStreams == expectedReservedStream, "The stream container does not have the expected number of reserved streams."));

		if (autoLoad)
			Load();
	}

	public bool RequiresLoad => !_loaded || StreamContainer.RequiresLoad;

	public StreamContainer StreamContainer { get; }

	public long Count => StreamContainer.Count - StreamContainer.Header.ReservedStreams;

	public void Load() {
		if (StreamContainer.RequiresLoad)
			StreamContainer.Load();

		NotifyLoading();
		_loaded = true;
		NotifyLoaded();
	}

	public Task LoadAsync() => StreamContainer.LoadAsync();

	public void SaveItem(long index, object item, ListOperationType operationType) {
		Guard.Argument(operationType != ListOperationType.Remove, nameof(operationType), "Remove operation not supported in this method");
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

	public void RemoveItem(long index) {
		NotifyPreItemOperation(index, null, ListOperationType.Remove);
		using var _ = StreamContainer.EnterAccessScope();
		StreamContainer.Remove(index + StreamContainer.Header.ReservedStreams);
		NotifyPostItemOperation(index, null, ListOperationType.Remove);
	}

	public void Clear() {
		NotifyClearing();
		StreamContainer.Clear();
		NotifyCleared();
	}

	internal ClusteredStream SaveItemAndReturnStream(long index, object item, ListOperationType operationType) {
		Guard.Argument(operationType != ListOperationType.Remove, nameof(operationType), "Remove operation not supported in this method");
		CheckItemType(item);

		var streamIndex = index + StreamContainer.Header.ReservedStreams;
		// initialized and re-entrancy checks done by one of below called methods
		var stream = operationType switch {
			ListOperationType.Add => StreamContainer.Add(),
			ListOperationType.Update => StreamContainer.OpenWrite(streamIndex),
			ListOperationType.Insert => StreamContainer.Insert(streamIndex),
			_ => throw new ArgumentException($@"List operation type '{operationType}' not supported", nameof(operationType)),
		};
		try {
			using var writer = new EndianBinaryWriter(EndianBitConverter.For(StreamContainer.Endianness), stream);
			if (item != null) {
				NotifyPreItemOperation(index, item, operationType);
				stream.IsNull = false;
				if (_preAllocateOptimization) {
					// pre-setting the stream length before serialization improves performance since it avoids
					// re-allocating fragmented stream on individual properties of the serialized item
					var expectedSize = _packedSerializer.CalculateSize(item);
					stream.SetLength(expectedSize);
					_packedSerializer.Serialize(item, writer);
				} else {
					var byteLength = _packedSerializer.Serialize(item, writer);
					stream.SetLength(byteLength);
				}

			} else {
				stream.IsNull = true;
				stream.SetLength(0); // open descriptor will save when closed
			}
			NotifyPostItemOperation(index, item, operationType);
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
			NotifyPreItemOperation(index, default, ListOperationType.Read);
			if (!stream.IsNull) {
				using var reader = new EndianBinaryReader(EndianBitConverter.For(StreamContainer.Endianness), stream);
				item = _packedSerializer.Deserialize(stream.Length, reader);
				CheckItemType(item);
			} else item = default;
			NotifyPostItemOperation(index, item, ListOperationType.Read);
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

	private void NotifyLoading() {
		Loading?.Invoke(this);
	}

	private void NotifyLoaded() {
		Loaded?.Invoke(this);
	}

	private void NotifyPreItemOperation(long index, object item, ListOperationType operationType) {
		CheckItemType(item);
		PreItemOperation?.Invoke(this, index, item, operationType);
	}

	private void NotifyPostItemOperation(long index, object item, ListOperationType operationType) {
		CheckItemType(item);
		PostItemOperation?.Invoke(this, index, item, operationType);
	}

	private void NotifyClearing() {
		Clearing?.Invoke(this);
	}

	private void NotifyCleared() {
		Cleared?.Invoke(this);
	}

	private void CheckItemType(object item) {
		if (item != null)
			CheckType(item.GetType());
	}

	private void CheckType(Type type) {
		if (type != _objectType)
			throw new InvalidOperationException($@"This container does not support type '{type}'");
	}
}

