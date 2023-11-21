using System;
using System.Collections.Generic;

namespace Hydrogen;

public class SerializationContext : SyncScope {
	private BijectiveDictionary<object, long> _serializedObjects;
	private HashSet<long> _onlySizedButNotSerialized;  // this is needed since some serializers may size their objects before serializing them, and we need to know if they were already serialized
	private List<Action> _onRootContextEndActions;
	private long _currentlyDeserializingIndex;

	public SerializationContext() {
		_serializedObjects = new BijectiveDictionary<object, long>();
		_onlySizedButNotSerialized = new HashSet<long>();
		_onRootContextEndActions = new List<Action>();
		_currentlyDeserializingIndex = -1;
	}

	public static SerializationContext New => new();

	public bool HasSerializedObject(object obj, out long serializationContextIndex, bool sizeOnly) {
		if (obj is null) {
			serializationContextIndex = -1;
			return false;
		}

		var hasSerialized = _serializedObjects.TryGetValue(obj, out serializationContextIndex);
		
		if (hasSerialized && !sizeOnly) {
			// if we're not just sizing (ie. we're serializing), we need to make sure that
			// the object is not in _onlySizedButNotSerialized set because if it is it means
			// it hasn't been serialized yet. 
			hasSerialized &= !_onlySizedButNotSerialized.Contains(serializationContextIndex);
		}

		return hasSerialized;
	}

	public object GetSerializedObject(long serializationContextIndex)
		=> _serializedObjects.Bijection[serializationContextIndex];

	public void NotifySerializingObject(object obj, bool sizeOnly) {
		obj ??= new NullPlaceHolder();

		if (_serializedObjects.TryGetValue(obj, out var index)) {
			// Some serializers may size objects before serializing them, and sizing may notify an object of serializtion for sizing purposes only
			// so we need to make sure we re-use the index established during sizing, and unmark this object as "sizing only"
			if (!_onlySizedButNotSerialized.Contains(index))
				throw new InvalidOperationException("Object was already serialized in this serialization context");
			_onlySizedButNotSerialized.Remove(index);
			return;
		}

		index = _serializedObjects.Count;

		_serializedObjects.Add(obj, index);
		if (sizeOnly)
			_onlySizedButNotSerialized.Add(index);
	}

	public void NotifyDeserializingObject(out long serializationContextIndex) {
		//Guard.Ensure(_currentlyDeserializingIndex == -1, "Parent item in deserialization context needs to be set before setting it");
		serializationContextIndex = _serializedObjects.Count;
		_currentlyDeserializingIndex = serializationContextIndex;
		_serializedObjects.Add(new PlaceHolder(this, serializationContextIndex), serializationContextIndex); // we place a dummy object in the dictionary to reserve the index
		_currentlyDeserializingIndex = serializationContextIndex;
	}

	public void SetDeserializingItem<TItem>(TItem item) {
		//Guard.Ensure(_currentlyDeserializingIndex >= 0, "Context was not notified a deserialization");

		if (_currentlyDeserializingIndex < 0)
			return;

		if (_currentlyDeserializingIndex == -1)
			NotifyDeserializingObject(out _);

		Guard.Ensure(_serializedObjects.Bijection[_currentlyDeserializingIndex] is PlaceHolder, "Expected PlaceHolder but was an instance. Logic error in serialization flow.");
		_serializedObjects.Bijection[_currentlyDeserializingIndex] = item;
		_currentlyDeserializingIndex = -1;
	}

	public void NotifyDeserializedObject(object obj, long serializationContextIndex) {
		obj ??= new NullPlaceHolder();
		
		if (_serializedObjects.Bijection.TryGetValue(serializationContextIndex, out var item)) 
			Guard.Ensure(item is PlaceHolder || ReferenceEquals(item, obj), "Cannot change a deserialized instance in context. Logic error in serialization flow.");
		
		_serializedObjects.Bijection[serializationContextIndex] = obj; // setting it this way updates the dummy object put in NotifyDeserializingObject
		_currentlyDeserializingIndex = -1;
	}

	public void RegisterFinalizationAction(Action action) => _onRootContextEndActions.Add(action);

	protected override void OnScopeEnd() {
		foreach (var action in _onRootContextEndActions)
			action();
		_serializedObjects.Clear();

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
			var value = _owner.GetSerializedObject(_serializationContextIndex);
			if (value is NullPlaceHolder)
				value = null;
			return value;
		}
	}

	internal class NullPlaceHolder {
		public override string ToString() => "NULL PLACEHOLDER";
	}

	internal enum SerializationContextType {
		Sizing,
		Serializing,
		Deserializing
	}

	#endregion



}
