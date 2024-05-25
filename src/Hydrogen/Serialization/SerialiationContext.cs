using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public sealed class SerializationContext : SyncScope {

	private enum SerializationStatus { Sizing, Sized, Serializing, Serialized, Deserializing, Deserialized }

	private readonly BijectiveDictionary<object, long> _processedObjects;
	private readonly IDictionary<long, SerializationStatus> _objectSerializationStatus;
	private readonly List<Action> _onRootContextEndActions;
	private long _currentlyDeserializingIndex;
	private SerializerFactory _serializerFactory;
	private long _lastNonEphemeralTypeCode;

	public SerializationContext() {
		_processedObjects = new BijectiveDictionary<object, long>(ReferenceEqualityComparer.Instance, EqualityComparer<long>.Default);
		_objectSerializationStatus = new Dictionary<long, SerializationStatus>();
		_onRootContextEndActions = new List<Action>();
		_currentlyDeserializingIndex = -1;
		_serializerFactory = null;
		_lastNonEphemeralTypeCode = 0;
	}

	public static SerializationContext New => new();

	public SerializerFactory EphemeralFactory {
		get  {
			Guard.Ensure(_serializerFactory is not null, "On-the-fly serializer construction is not permitted in this serialization context");
			return _serializerFactory;
		}
	}

	public bool HasEphemeralFactory => _serializerFactory is not null;

	public void SetEphemeralFactory(SerializerFactory factory) {
		Guard.Ensure(factory is not null && _serializerFactory is null, "An ephemeral factory has already been set in this serialization context");
		_serializerFactory = factory;
		_lastNonEphemeralTypeCode = factory.MaxGeneratedTypeCode;
	}

	public IEnumerable<SerializerFactory.Registration> GetEphemeralRegistrations() => _serializerFactory.GetRegistrationsAfterTypeCode(_lastNonEphemeralTypeCode);

	public void ClearEphemeralFactory() {
		Guard.Ensure(_serializerFactory is not null , "No ephemeral factory was set in this context");
		_serializerFactory = null;
		_lastNonEphemeralTypeCode = 0;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public bool IsSizingOrSerializingObject(object obj, out long index) {
		if (obj is null) {
			index = -1;
			return false;
		}

		return _processedObjects.TryGetValue(obj, out index) && _objectSerializationStatus[index].IsIn(SerializationStatus.Sizing, SerializationStatus.Serializing);
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public bool HasSizedOrSerializedObject(object obj, out long index) {
		if (obj is null) {
			index = -1;
			return false;
		}

		// Optimized form of:
		// return _processedObjects.TryGetValue(obj, out index) && _objectSerializationStatus[index].IsIn(SerializationStatus.Sizing, SerializationStatus.Sized, SerializationStatus.Serializing, SerializationStatus.Serialized);
		if (!_processedObjects.TryGetValue(obj, out index))
			return false;
		var status = _objectSerializationStatus[index];
		return (int)status is >= (int)SerializationStatus.Sizing and <= (int)SerializationStatus.Serialized;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public bool IsSerializingObject(object obj, out long index) {
		if (obj is null) {
			index = -1;
			return false;
		}

		return _processedObjects.TryGetValue(obj, out index) && _objectSerializationStatus[index] == SerializationStatus.Serializing;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public bool IsSerializingOrHasSerializedObject(object obj, out long index) {
		if (obj is null) {
			index = -1;
			return false;
		}

		// Optimized form of:
		// return _processedObjects.TryGetValue(obj, out index) && _objectSerializationStatus[index].IsIn(SerializationStatus.Serializing,  SerializationStatus.Serialized);
		if (!_processedObjects.TryGetValue(obj, out index))
			return false;
		var status = _objectSerializationStatus[index];
		return (int)status is >= (int)SerializationStatus.Serializing and <= (int)SerializationStatus.Serialized;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public object GetSizedOrSerializedObject(long index) {
		Debug.Assert(_objectSerializationStatus.TryGetValue(index, out var status) && status.IsIn(SerializationStatus.Sizing, SerializationStatus.Sized, SerializationStatus.Serializing, SerializationStatus.Serialized), $"No object was sized/serialized in this serialization context at index {index}");
		return _processedObjects.Bijection[index];
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public object GetSerializedObject(long index) {
		Debug.Assert(_objectSerializationStatus.TryGetValue(index, out var status) && status == SerializationStatus.Serialized, $"No object was serialized in this serialization context at index {index}");
		return _processedObjects.Bijection[index];
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public object GetDeserializedObject(long index) {
		if(!_objectSerializationStatus.TryGetValue(index, out var status))
			throw new InvalidOperationException($"No object at deserialization context index {index} exists");

		if (status != SerializationStatus.Deserialized)
			throw new InvalidOperationException($"Object at deserialization context index {index} was in status {status}");

		return _processedObjects.Bijection[index];
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public void NotifySizing(object obj, out long index) {
		obj ??= new NullPlaceHolder();
		index = _processedObjects.Count;
		_processedObjects[obj] = index;;
		_objectSerializationStatus[index] = SerializationStatus.Sizing;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public void NotifySized(long index) {
		_objectSerializationStatus[index] = SerializationStatus.Sized;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public void NotifySerializedObject(long index) {
		_objectSerializationStatus[index] = SerializationStatus.Serialized;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public void NotifyDeserializingObject(out long serializationContextIndex) {
		serializationContextIndex = _processedObjects.Count;
		_processedObjects.Add(new PlaceHolder(this, serializationContextIndex), serializationContextIndex); // we place a dummy object in the dictionary to reserve the index
		_objectSerializationStatus.Add(serializationContextIndex, SerializationStatus.Deserializing);
		_currentlyDeserializingIndex = serializationContextIndex;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public void SetDeserializingItem(object item) {
		if (_currentlyDeserializingIndex < 0)
			return;

		// this is a special case where we are deserializing an object that was not notified as being deserialized
		if (_currentlyDeserializingIndex == -1)
			NotifyDeserializingObject(out _);

		Debug.Assert(_processedObjects.Bijection[_currentlyDeserializingIndex] is PlaceHolder, "Expected PlaceHolder but was an instance. Logic error in serialization flow.");
		_processedObjects.Bijection[_currentlyDeserializingIndex] = item;
		_objectSerializationStatus[_currentlyDeserializingIndex] = SerializationStatus.Deserialized;
		_currentlyDeserializingIndex = -1;
	}
	
#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public void NotifyDeserializedObject(object obj, long serializationContextIndex) {
		obj ??= new NullPlaceHolder();
		
		if (_processedObjects.Bijection.TryGetValue(serializationContextIndex, out var item)) 
			Debug.Assert(item is PlaceHolder || ReferenceEquals(item, obj), "Cannot change a deserialized instance in context. Logic error in serialization flow.");
		
		_processedObjects.Bijection[serializationContextIndex] = obj; // setting it this way updates the dummy object put in NotifyDeserializingObject
		_objectSerializationStatus[serializationContextIndex] = SerializationStatus.Deserialized;
		_currentlyDeserializingIndex = -1;
	}

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public void RegisterFinalizationAction(Action action) => _onRootContextEndActions.Add(action);

#if !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	protected override void OnScopeEnd() {
		foreach (var action in _onRootContextEndActions)
			action();
		_processedObjects.Clear();

	}

	#region Inner Types

	internal sealed class PlaceHolder {
		private readonly SerializationContext _owner;
		private readonly long _serializationContextIndex;

		public PlaceHolder(SerializationContext owner, long serializationContextIndex) {
			_owner = owner;
			_serializationContextIndex = serializationContextIndex;
		}

#if !DEBUG
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
