using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hydrogen;

public class SerializationContext : SyncScope {

	private enum SerializationStatus { Sizing, Sized, Serializing, Serialized, Deserializing, Deserialized }

	private BijectiveDictionary<object, long> _processedObjects;
	private IDictionary<long, SerializationStatus> _objectSerializationStatus;
	private List<Action> _onRootContextEndActions;
	

	public SerializationContext() {
		_processedObjects = new BijectiveDictionary<object, long>(ReferenceEqualityComparer.Instance, EqualityComparer<long>.Default);
		_objectSerializationStatus = new Dictionary<long, SerializationStatus>();
		_onRootContextEndActions = new List<Action>();
	}

	public static SerializationContext New => new();

	public bool IsSizingOrSerializingObject(object obj, out long index) {
		if (obj is null) {
			index = -1;
			return false;
		}

		return _processedObjects.TryGetValue(obj, out index) && _objectSerializationStatus[index].IsIn(SerializationStatus.Sizing, SerializationStatus.Serializing);
	}

	public bool HasSizedOrSerializedObject(object obj, out long index) {
		if (obj is null) {
			index = -1;
			return false;
		}

		return _processedObjects.TryGetValue(obj, out index) && _objectSerializationStatus[index].IsIn(SerializationStatus.Sizing, SerializationStatus.Sized, SerializationStatus.Serializing, SerializationStatus.Serialized);
	}

	public bool IsSerializingObject(object obj, out long index) {
		if (obj is null) {
			index = -1;
			return false;
		}

		return _processedObjects.TryGetValue(obj, out index) && _objectSerializationStatus[index] == SerializationStatus.Serializing;
	}

	public bool IsSerializingOrHasSerializedObject(object obj, out long index) {
		if (obj is null) {
			index = -1;
			return false;
		}

		return _processedObjects.TryGetValue(obj, out index) && _objectSerializationStatus[index].IsIn(SerializationStatus.Serializing,  SerializationStatus.Serialized);
	}

	public object GetSizedOrSerializedObject(long index) {
		Guard.Ensure(_objectSerializationStatus.TryGetValue(index, out var status) && status.IsIn(SerializationStatus.Sizing, SerializationStatus.Sized, SerializationStatus.Serializing, SerializationStatus.Serialized), $"No object was sized/serialized in this serialization context at index {index}");
		return _processedObjects.Bijection[index];
	}

	public object GetSerializedObject(long index) {
		Guard.Ensure(_objectSerializationStatus.TryGetValue(index, out var status) && status == SerializationStatus.Serialized, $"No object was serialized in this serialization context at index {index}");
		return _processedObjects.Bijection[index];
	}

	public object GetDeserializedObject(long index) {
		Guard.Ensure(_objectSerializationStatus.TryGetValue(index, out var status) && status == SerializationStatus.Deserialized, $"No object was serialized in this serialization context at index {index}");
		return _processedObjects.Bijection[index];
	}

	public void NotifySizing(object obj, out long index) {
		obj ??= new NullPlaceHolder();
		index = _processedObjects.Count;
		_processedObjects[obj] = index;;
		_objectSerializationStatus[index] = SerializationStatus.Sized;
	}

	public void NotifySized(long index) {
		_objectSerializationStatus[index] = SerializationStatus.Sized;
	}

	public void NotifySerializingObject(object obj, out long index) {
		obj ??= new NullPlaceHolder();

		if (_processedObjects.TryGetValue(obj, out index)) {
			// Some serializers may size objects before serializing them, and sizing may notify an object of serializtion for sizing purposes only
			// so we need to make sure we re-use the index established during sizing, and unmark this object as "sizing only"
			//if (_objectSerializationStatus[index] != SerializationStatus.Sized)
				//throw new InvalidOperationException("Object was already serialized in this serialization context");
			_objectSerializationStatus[index] = SerializationStatus.Serializing;
			return;
		}

		index = _processedObjects.Count;
		_processedObjects[obj] = index;
		_objectSerializationStatus[index] = SerializationStatus.Serializing;
	}

	public void NotifySerializedObject(long index) {
		_objectSerializationStatus[index] = SerializationStatus.Serialized;
	}

	long _currentlyDeserializingIndex = -1;

	public void NotifyDeserializingObject(out long serializationContextIndex) {
		serializationContextIndex = _processedObjects.Count;
		_processedObjects.Add(new PlaceHolder(this, serializationContextIndex), serializationContextIndex); // we place a dummy object in the dictionary to reserve the index
		_objectSerializationStatus.Add(serializationContextIndex, SerializationStatus.Deserializing);
		_currentlyDeserializingIndex = serializationContextIndex;
	}

	public void SetDeserializingItem(object item) {
		if (_currentlyDeserializingIndex < 0)
			return;

		// this is a special case where we are deserializing an object that was not notified as being deserialized
		if (_currentlyDeserializingIndex == -1)
			NotifyDeserializingObject(out _);

		Guard.Ensure(_processedObjects.Bijection[_currentlyDeserializingIndex] is PlaceHolder, "Expected PlaceHolder but was an instance. Logic error in serialization flow.");
		_processedObjects.Bijection[_currentlyDeserializingIndex] = item;
		_objectSerializationStatus[_currentlyDeserializingIndex] = SerializationStatus.Deserialized;
		_currentlyDeserializingIndex = -1;
	}

	public void NotifyDeserializedObject(object obj, long serializationContextIndex) {
		obj ??= new NullPlaceHolder();
		
		if (_processedObjects.Bijection.TryGetValue(serializationContextIndex, out var item)) 
			Guard.Ensure(item is PlaceHolder || ReferenceEquals(item, obj), "Cannot change a deserialized instance in context. Logic error in serialization flow.");
		
		_processedObjects.Bijection[serializationContextIndex] = obj; // setting it this way updates the dummy object put in NotifyDeserializingObject
		_objectSerializationStatus[serializationContextIndex] = SerializationStatus.Deserialized;
		_currentlyDeserializingIndex = -1;
	}

	public void RegisterFinalizationAction(Action action) => _onRootContextEndActions.Add(action);

	protected override void OnScopeEnd() {
		foreach (var action in _onRootContextEndActions)
			action();
		_processedObjects.Clear();

	}

	#region Inner Types

	internal class PlaceHolder {
		private readonly SerializationContext _owner;
		private readonly long _serializationContextIndex;

		public PlaceHolder(SerializationContext owner, long serializationContextIndex) {
			_owner = owner;
			_serializationContextIndex = serializationContextIndex;
		}

		public object GetValue() {
			var value = _owner.GetDeserializedObject(_serializationContextIndex);
			if (value is NullPlaceHolder)
				value = null;
			return value;
		}
	}

	internal class NullPlaceHolder {
		public override string ToString() => "NULL PLACEHOLDER";
	}

	#endregion

}
