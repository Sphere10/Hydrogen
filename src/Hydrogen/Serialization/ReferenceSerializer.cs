using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Serializer wrapper for handling reference types. This serializes a prefix byte that indicates whether the item is null, not null, or a reference to an object
/// that was previously serialized in the serialization context. It is used to support nullability and cyclic/repeating references in a serialization context.
/// </summary>
public sealed class ReferenceSerializer<TItem> : ItemSerializerDecorator<TItem> {
	private const string ErrMsg_NullValuesNotEnabled = "Null values are not enabled on this serializer";

	private readonly bool _supportsNull;
	private readonly bool _supportsContextReferences;
	private readonly bool _supportsExternalReferences;
	private readonly bool _supportsReferences; 

	public ReferenceSerializer(IItemSerializer<TItem> valueSerializer) 
		: this(valueSerializer, ReferenceSerializerMode.Default) {
	}


	public ReferenceSerializer(IItemSerializer<TItem> valueSerializer, ReferenceSerializerMode mode) 
		: base(valueSerializer) {
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
		var itemType = ClassifyItem(item, context, true, out var contextIndex);
		switch(itemType) {
			case PrefixTag.IsNull:
				context.NotifySerializingObject(item, true);
				return sizeof(byte);
			case PrefixTag.IsNotNull:
				context.NotifySerializingObject(item, true);
				return sizeof(byte) + Internal.CalculateSize(context, item);
			case PrefixTag.IsContextReference:
				return sizeof(byte) + CVarIntSerializer.Instance.CalculateSize(context, contextIndex);
			default:
				throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
		}
	}

	public override void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) {
		var itemType = ClassifyItem(item, context, false, out var contextIndex);
		PrimitiveSerializer<byte>.Instance.Serialize((byte)itemType, writer, context);
		switch (itemType) {
			case PrefixTag.IsNull:
				if (_supportsReferences)
					context.NotifySerializingObject(item, false);
				break;
			case PrefixTag.IsNotNull:
				if (_supportsReferences)
					context.NotifySerializingObject(item, false);
				Internal.Serialize(item, writer, context);
				break;
			case PrefixTag.IsContextReference:
				CVarIntSerializer.Instance.Serialize(contextIndex, writer, context);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
		}
	}

	public override TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var itemType = (PrefixTag)PrimitiveSerializer<byte>.Instance.Deserialize(reader, context);
		switch (itemType) {
			case PrefixTag.IsNull:
				Guard.Ensure(_supportsNull, ErrMsg_NullValuesNotEnabled);
				if (_supportsReferences)
					context.NotifyDeserializingObject(out _);
				return default;
			case PrefixTag.IsNotNull:
				long index = -1;
				if (_supportsReferences)
					context.NotifyDeserializingObject(out index);
				
				var item = Internal.Deserialize(reader, context);

				if (_supportsReferences)
					context.NotifyDeserializedObject(item, index);
				return item;
			case PrefixTag.IsContextReference:
				var contextIndex = CVarIntSerializer.Instance.Deserialize(reader, context);
				return (TItem)context.GetSerializedObject(contextIndex);
			default:
				throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
		}
	}

	private PrefixTag ClassifyItem(TItem item, SerializationContext context, bool sizeOnly, out long index) {
		index = -1;
		if (item == null) 
			return _supportsNull ? PrefixTag.IsNull : throw new InvalidOperationException(ErrMsg_NullValuesNotEnabled);

		if (_supportsContextReferences && context.HasSerializedObject(item, out index, sizeOnly))
			return PrefixTag.IsContextReference;
		
		return PrefixTag.IsNotNull;
	}
	public enum PrefixTag : byte {
		IsNull = 0,
		IsNotNull = 1,
		IsContextReference = 2,
		//IsExternalReference = 3,   // serializers a pointer to an external object and places external object in the context for serialization by user
	}
}