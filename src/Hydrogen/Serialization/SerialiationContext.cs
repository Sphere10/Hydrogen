using System;
using System.Collections.Generic;

namespace Hydrogen;

public class SerializationContext : SyncScope {
	private BijectiveDictionary<object, long> _serializedObjects;
	private HashSet<long> _sizedOnlyObjects;  // this is needed since some serializers may size their objects before serializing them, and we need to know if they were already serialized
	private List<Action> _onRootContextEndActions;

	//public EndianBinaryWriter Writer { get; }

	//public EndianBinaryReader Reader { get; }


	public static SerializationContext New => new();

	public void SetDeserializingItem<TItem>(TItem item) {
		throw new NotImplementedException();
	}

	public bool HasSerializedObject(object obj, out long serializationContextIndex) {
		if (obj is null) {
			serializationContextIndex = -1;
			return false;
		}

		return _serializedObjects.TryGetValue(obj, out serializationContextIndex) && !_sizedOnlyObjects.Contains(serializationContextIndex);
	}

	public object GetSerializedObject(long serializationContextIndex)
		=> _serializedObjects.Bijection[serializationContextIndex];

	public void NotifySerializingObject(object obj, bool sizeOnly) {
		obj ??= new NullPlaceHolder();

		if (_serializedObjects.TryGetValue(obj, out var index)) {
			// Some serializers may size objects before serializing them, and sizing may notify an object of serializtion for sizing purposes only
			// so we need to make sure we re-use the index established during sizing, and unmark this object as "sizing only"
			if (!_sizedOnlyObjects.Contains(index))
				throw new InvalidOperationException("Object was already serialized in this serialization context");
			_sizedOnlyObjects.Remove(index);
			return;
		}

		index = _serializedObjects.Count;

		_serializedObjects.Add(obj, index);
		if (sizeOnly)
			_sizedOnlyObjects.Add(index);
	}

	public void NotifyDeserializingObject(out long serializationContextIndex) {
		serializationContextIndex = _serializedObjects.Count;
		_serializedObjects.Add(new PlaceHolder(this, serializationContextIndex), serializationContextIndex); // we place a dummy object in the dictionary to reserve the index
	}

	public void NotifyDeserializedObject(object obj, long serializationContextIndex) {
		if (obj is not null)
			Guard.Argument(!_serializedObjects.ContainsKey(obj), nameof(obj), "Object was already serialized in this serialization context");
		obj ??= new NullPlaceHolder();
		_serializedObjects.Bijection[serializationContextIndex] = obj; // setting it this way updates the dummy object put in NotifyDeserializingObject
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

	#endregion



}
