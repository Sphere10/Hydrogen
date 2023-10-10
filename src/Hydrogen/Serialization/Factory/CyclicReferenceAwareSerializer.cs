//using System;
//using System.Collections.Generic;

//namespace Hydrogen;

//	// if sub-serializer is composite seializer, you need to listen to the on create event
//	// basically extract out the cyclic reference logic in composite ser
//	// you can now localize the scope classes, etc
//	// hooray, clean again!
//internal class CyclicReferenceAwareSerializer<T> : ItemSerializerDecorator<T> {
//	public const byte DefaultCyclicReferenceMarker = 0;
//	private int _cyclicReferenceMarkerSizeBytes;
//	private Func<EndianBinaryReader, bool> _peekForCyclicReferenceMarker;
//	private Action<EndianBinaryWriter> _writeCyclicReferenceMarker;

//	public CyclicReferenceAwareSerializer(SerializerFactory factory, byte cyclicReferenceMarker = DefaultCyclicReferenceMarker)
//		: this(new FactorySerializer<T>(factory), cyclicReferenceMarker) {
//	}

//	// DANGEROUS since it will not work if the cyclic reference marker is a valid value for the type being serialized
//	// Kept private for now
//	private CyclicReferenceAwareSerializer(FactorySerializer<T> internalSerializer, byte cyclicReferenceMarker = DefaultCyclicReferenceMarker) 
//		: this(
//			internalSerializer,
//			CVarInt.SizeOf(cyclicReferenceMarker),
//			reader => reader.BaseStream.PeekNextByte() == cyclicReferenceMarker,
//			writer => CVarInt.Write(cyclicReferenceMarker, writer.BaseStream)
//		) {
//	}

//	public CyclicReferenceAwareSerializer(FactorySerializer<T> internalSerializer, int cyclicReferenceMarkerSizeBytes, Func<EndianBinaryReader, bool> peekForCyclicReferenceMarker, Action<EndianBinaryWriter> writeCyclicReferenceMarker) 
//		: base(internalSerializer) {
//		_cyclicReferenceMarkerSizeBytes = cyclicReferenceMarkerSizeBytes;
//		_peekForCyclicReferenceMarker = peekForCyclicReferenceMarker;
//		_writeCyclicReferenceMarker = writeCyclicReferenceMarker;
//	}

//	// Used by SerializerFactory to late-bind the internal serializer
//	internal CyclicReferenceAwareSerializer() {
//		_cyclicReferenceMarkerSizeBytes = CVarInt.SizeOf(DefaultCyclicReferenceMarker);
//		_peekForCyclicReferenceMarker = reader => reader.BaseStream.PeekNextByte() == DefaultCyclicReferenceMarker;
//		_writeCyclicReferenceMarker = writer => CVarInt.Write(DefaultCyclicReferenceMarker, writer.BaseStream);
//	}

//	internal void SetInternalSerializer(IItemSerializer<T> internalSerializer) {
//		Guard.ArgumentNotNull(internalSerializer, nameof(internalSerializer));
//		Internal = internalSerializer;
//	}

//	public override bool IsConstantSize => false;

//	public override long CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out long[] itemSizes) {
//		var itemSizesL = new List<long>();
//		var totalSize = 0L;
//		foreach (var item in items) {
//			using var context = SerializationContext.New;
//			var itemSize = CalculateSize(context, item);
//			if (calculateIndividualItems)
//				itemSizesL.Add(itemSize);
//			totalSize += itemSize;
//		}
//		itemSizes = itemSizesL.ToArray();
//		return totalSize;
//	}

//	public override long CalculateSize(SerializationContext context, T item) {
//		using var scope = new SerializationScope();
//		if (scope.HasSerializedObject(item, out var serializationContextIndex)) {
//			return _cyclicReferenceMarkerSizeBytes + CyclicReferenceSerializer.Instance.CalculateSize(context, new CyclicReference(serializationContextIndex));
//		}
//		scope.NotifySerializingObject(item, true);
//		return base.CalculateSize(context, item);

//	}

//	public override void Serialize(T item, EndianBinaryWriter writer, SerializationContext context) {
//		using var scope = new SerializationScope();
//		if (scope.HasSerializedObject(item, out var serializationContextIndex)) {
//			_writeCyclicReferenceMarker(writer);
//			CyclicReferenceSerializer.Instance.Serialize(new CyclicReference(serializationContextIndex), writer, context);
//		} else {
//			scope.NotifySerializingObject(item, false);
//			base.Serialize(item, writer, context);
//		}
//	}

//	public override T Deserialize(EndianBinaryReader reader, SerializationContext context) {
//		using var scope = new SerializationScope();
		
//		T item;
//		if (_peekForCyclicReferenceMarker(reader)) {
//			// deserializing a cyclic reference
//			reader.ReadBytes(_cyclicReferenceMarkerSizeBytes); // skip cyclic reference marker
//			var cyclicReference = CyclicReferenceSerializer.Instance.Deserialize(reader, context);
//			item = (T)scope.GetSerializedObject(cyclicReference.Index);
//		} else {
//			// deserializing an item, track it for potential future cyclic reference
//			scope.NotifyDeserializingObject(out var serializationContextIndex);
//			item = base.Deserialize(reader, context);
//			scope.NotifyDeserializedObject(item, serializationContextIndex);
//		}
//		return item;
//	}

//}

//internal record struct CyclicReference(long Index);

//internal class CyclicReferenceSerializer : ItemSerializer<CyclicReference> {

//	public static CyclicReferenceSerializer Instance { get; } = new();
//	public override long CalculateSize(SerializationContext context, CyclicReference item)
//		=> CVarInt.SizeOf(unchecked((ulong)item.Index));

//	public override void Serialize(CyclicReference item, EndianBinaryWriter writer, SerializationContext context)
//		=> CVarInt.Write(unchecked((ulong)item.Index), writer.BaseStream);

//	public override CyclicReference Deserialize(EndianBinaryReader reader, SerializationContext context)
//		=> new(unchecked((long)CVarInt.Read(reader.BaseStream)));
//}

//internal class SerializationScope : SyncContextScope {
//	private const string ContextID = nameof(SerializationScope);

//	private BijectiveDictionary<object, long> _serializedObjects;
//	private HashSet<long> _sizedOnlyObjects;  // this is needed since some serializers may size their objects before serializing them, and we need to know if they were already serialized
//	private List<Action> _onRootContextEndActions;

//	public SerializationScope() : base(ContextScopePolicy.None, ContextID) {
//	}

//	public static SerializationScope Current => (SerializationScope)CallContext.LogicalGetData(ContextID);

//	protected override void OnContextStart() {
//		_serializedObjects = new BijectiveDictionary<object, long>(
//		   new ReferenceDictionary<object, long>(),
//		   new Dictionary<long, object>()
//	   );
//		_sizedOnlyObjects = new HashSet<long>();
//		_onRootContextEndActions = new List<Action>();
//	}

//	protected override void OnContextResume() {
//		_serializedObjects = ((SerializationScope)RootScope)._serializedObjects;
//		_onRootContextEndActions = ((SerializationScope)RootScope)._onRootContextEndActions;
//		_sizedOnlyObjects = ((SerializationScope)RootScope)._sizedOnlyObjects;
//	}

//	protected override void OnContextEnd() {
//		if (IsRootScope) {
//			foreach (var action in _onRootContextEndActions)
//				action();
//			_serializedObjects.Clear();
//		}
//	}

//	public bool HasSerializedObject(object obj, out long serializationContextIndex) {
//		if (obj is null) {
//			serializationContextIndex = -1;
//			return false;
//		}

//		return _serializedObjects.TryGetValue(obj, out serializationContextIndex) && !_sizedOnlyObjects.Contains(serializationContextIndex);
//	}

//	public object GetSerializedObject(long serializationContextIndex)
//		=> _serializedObjects.Bijection[serializationContextIndex];

//	public void NotifySerializingObject(object obj, bool sizeOnly) {
//		obj ??= new NullPlaceHolder();

//		if (_serializedObjects.TryGetValue(obj, out var index)) {
//			// Some serializers may size objects before serializing them, and sizing may notify an object of serializtion for sizing purposes only
//			// so we need to make sure we re-use the index established during sizing, and unmark this object as "sizing only"
//			if (!_sizedOnlyObjects.Contains(index))
//				throw new InvalidOperationException("Object was already serialized in this serialization context");
//			_sizedOnlyObjects.Remove(index);
//			return;
//		}

//		index = _serializedObjects.Count;
		
//		_serializedObjects.Add(obj, index);
//		if (sizeOnly)
//			_sizedOnlyObjects.Add(index);
//	}

//	public void NotifyDeserializingObject(out long serializationContextIndex) {
//		serializationContextIndex = _serializedObjects.Count;
//		_serializedObjects.Add(new PlaceHolder(this, serializationContextIndex), serializationContextIndex); // we place a dummy object in the dictionary to reserve the index
//	}

//	public void NotifyDeserializedObject(object obj, long serializationContextIndex) {
//		if (obj is not null)
//			Guard.Argument(!_serializedObjects.ContainsKey(obj), nameof(obj), "Object was already serialized in this serialization context");
//		obj ??= new NullPlaceHolder();
//		_serializedObjects.Bijection[serializationContextIndex] = obj; // setting it this way updates the dummy object put in NotifyDeserializingObject
//	}

//	public void RegisterFinalizationAction(Action action) => _onRootContextEndActions.Add(action);

//	internal class PlaceHolder {
//		private readonly SerializationScope _ownerScope;
//		private readonly long _serializationContextIndex;

//		public PlaceHolder(SerializationScope ownerScope, long serializationContextIndex) {
//			_ownerScope = ownerScope;
//			_serializationContextIndex = serializationContextIndex;
//		}

//		public object GetValue() {
//			var value = _ownerScope.GetSerializedObject(_serializationContextIndex);
//			if (value is NullPlaceHolder)
//				value = null;
//			return value;
//		}
//	}

//	internal class NullPlaceHolder {
//		public override string ToString() => "NULL PLACEHOLDER";
//	}
//}