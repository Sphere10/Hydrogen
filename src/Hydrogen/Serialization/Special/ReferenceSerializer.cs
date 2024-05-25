using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Serializer wrapper for handling reference types. This serializes a prefix byte that indicates whether the item is null, not null, or a reference to an object
/// that was previously serialized in the serialization context. It is used to support nullability and cyclic/repeating references in a serialization context.
/// </summary>
public sealed class ReferenceSerializer<TItem> : ItemSerializerDecorator<TItem> {
	private static string ErrMsg_NullValuesNotEnabled = $"Null value for '{typeof(TItem).ToStringCS()}' is not permitted";

	private readonly bool _supportsNull;
	private readonly bool _supportsContextReferences;
	private readonly bool _supportsExternalReferences;
	private readonly bool _supportsReferences; 

	public ReferenceSerializer(IItemSerializer<TItem> valueSerializer) 
		: this(valueSerializer, ReferenceSerializerMode.Default) {
	}

	public ReferenceSerializer(IItemSerializer<TItem> valueSerializer, ReferenceSerializerMode mode) 
		: base(valueSerializer) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.Argument(!valueSerializer.GetType().IsSubtypeOfGenericType(typeof(ReferenceSerializer<>), out _), nameof(valueSerializer), "Value serializer cannot be a reference serializer");
		Guard.Ensure(!typeof(TItem).IsValueType, $"{nameof(TItem)} can only be used with reference types");
		_supportsNull = mode.HasFlag(ReferenceSerializerMode.SupportNull);
		_supportsContextReferences = mode.HasFlag(ReferenceSerializerMode.SupportContextReferences);
		_supportsExternalReferences = mode.HasFlag(ReferenceSerializerMode.SupportExternalReferences);
		_supportsReferences = _supportsContextReferences || _supportsExternalReferences;
	}

	public override bool SupportsNull => true;

	public override long CalculateTotalSize(SerializationContext context, IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		// We need to calculate the size of each item individually, since some may be references and some may not be.
		var sizes = items.Select(item => CalculateSize(context, item)).ToArray();
		itemSizes = calculateIndividualItems ? sizes.ToArray() : null;
		return sizes.Sum();
	}
		

	public override long CalculateSize(SerializationContext context, TItem item) {
		var referenceType = ClassifyReferenceType(item, context, true, out var contextIndex);
		switch(referenceType) {
			case ReferenceType.IsNull:
				context.NotifySizing(item, out contextIndex);
				long size = sizeof(byte);
				context.NotifySized(contextIndex);
				return size;
			case ReferenceType.IsNotNull:
				if (!_supportsReferences)
					if (context.IsSizingOrSerializingObject(item, out _))
						throw new InvalidOperationException($"Cyclic-reference was encountered when sizing item  '{item}'. Please ensure context references are enabled sizing cyclic-referencing object graphs or ensure no cyclic references exist.");
				context.NotifySizing(item, out contextIndex);
				size = sizeof(byte) + Internal.CalculateSize(context, item);
				context.NotifySized(contextIndex);
				return size;
			case ReferenceType.IsContextReference:
				return sizeof(byte) + CVarIntSerializer.Instance.CalculateSize(context, unchecked((ulong)contextIndex));
			default:
				throw new ArgumentOutOfRangeException(nameof(referenceType), referenceType, null);
		}
	}

	public override void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) {
		var referenceType = ClassifyReferenceType(item, context, false, out var contextIndex);
		PrimitiveSerializer<byte>.Instance.Serialize((byte)referenceType, writer, context);
		switch (referenceType) {
			case ReferenceType.IsNull:
				context.NotifySerializingObject(item, out contextIndex);
				context.NotifySerializedObject(contextIndex);
				break;
			case ReferenceType.IsNotNull:
				if (!_supportsReferences)
					if (context.IsSerializingObject(item, out _))
						throw new InvalidOperationException($"Cyclic-reference was encountered when serializing item '{item}'. Please ensure context references are enabled serializing cyclic-referencing object graphs or ensure no cyclic references exist.");
				context.NotifySerializingObject(item, out contextIndex);
				Internal.Serialize(item, writer, context);
				context.NotifySerializedObject(contextIndex);
				break;
			case ReferenceType.IsContextReference:
				CVarIntSerializer.Instance.Serialize(unchecked((ulong)contextIndex), writer, context);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(referenceType), referenceType, null);
		}
	}

	public override TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var referenceType = (ReferenceType)PrimitiveSerializer<byte>.Instance.Deserialize(reader, context);
		switch (referenceType) {
			case ReferenceType.IsNull:
				Guard.Ensure(_supportsNull, ErrMsg_NullValuesNotEnabled);
				context.NotifyDeserializingObject(out _);
				return default;
			case ReferenceType.IsNotNull:
				context.NotifyDeserializingObject(out var index);
				var item = Internal.Deserialize(reader, context);
				context.NotifyDeserializedObject(item, index);
				return item;
			case ReferenceType.IsContextReference:
				var contextIndex = CVarIntSerializer.Instance.Deserialize(reader, context);
				return (TItem)context.GetDeserializedObject(unchecked((long)(ulong)contextIndex));
			default:
				throw new ArgumentOutOfRangeException(nameof(referenceType), referenceType, null);
		}
	}

	private ReferenceType ClassifyReferenceType(TItem item, SerializationContext context, bool sizeOnly, out long index) {
		index = -1;
		if (item == null) 
			return _supportsNull ? ReferenceType.IsNull : throw new InvalidOperationException(ErrMsg_NullValuesNotEnabled);

		if (_supportsContextReferences && (sizeOnly ? context.HasSizedOrSerializedObject(item, out index) : context.IsSerializingOrHasSerializedObject(item, out index)))
			return ReferenceType.IsContextReference;
		
		return ReferenceType.IsNotNull;
	}
	public enum ReferenceType : byte {
		IsNull = 0,
		IsNotNull = 1,
		IsContextReference = 2,
		//IsExternalReference = 3,   // serializers a pointer to an external object and places external object in the context for serialization by user
	}
}