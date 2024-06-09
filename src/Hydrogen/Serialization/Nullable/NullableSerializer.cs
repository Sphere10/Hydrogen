// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System;

namespace Hydrogen;

public class NullableSerializer<T> : ItemSerializerBase<T?> where T : struct  {
	private readonly byte[] _padding;

	public NullableSerializer(IItemSerializer<T> valueSerializer, bool preserveConstantSize = false) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.Argument(!valueSerializer.SupportsNull, nameof(valueSerializer), "Serializer already supports null.");
		ValueSerializer = valueSerializer;
		var isConstantSize = ValueSerializer.IsConstantSize && preserveConstantSize;
		IsConstantSize =  isConstantSize;
		ConstantSize = isConstantSize ? sizeof(bool) + ValueSerializer.ConstantSize : -1;
		_padding = isConstantSize ? new byte[ValueSerializer.ConstantSize] : Array.Empty<byte>();
	}

	public static NullableSerializer<T> Instance { get; } = typeof(T) switch {
		{ IsPrimitive: true } => new(PrimitiveSerializer<T>.Instance),
		{ IsEnum: true } t => new NullableSerializer<T>(((IItemSerializer<T>) TypeActivator.Activate(typeof(EnumSerializer<>).MakeGenericType(t)))),
		{ } t when t == typeof(decimal) => new((IItemSerializer<T>) PrimitiveSerializer<decimal>.Instance),
		{ } t when t == typeof(DateTime) => new((IItemSerializer<T>) DateTimeSerializer.Instance),
		{ } t when t == typeof(TimeSpan) => new((IItemSerializer<T>) TimeSpanSerializer.Instance),
		{ } t when t == typeof(DateTimeOffset) => new((IItemSerializer<T>) DateTimeOffsetSerializer.Instance),
		{ } t when t == typeof(Guid) => new((IItemSerializer<T>) GuidSerializer.Instance),
		{ } t when t == typeof(CVarInt) => new((IItemSerializer<T>) (object)CVarIntSerializer.Instance),
		{ } t when t == typeof(VarInt) => new((IItemSerializer<T>) (object)VarIntSerializer.Instance),
		 _ => throw new InvalidOperationException($"Unable to determine value serializer for {typeof(T).ToStringCS()}, construct this serializer via constructor and specify the value serializer")
	};

	public override bool SupportsNull => true; 

	public override bool IsConstantSize { get; }

	public override long ConstantSize { get; } 

	public IItemSerializer<T> ValueSerializer { get; }

	public override long CalculateSize(SerializationContext context, T? item) {
		if (IsConstantSize)
			return ConstantSize;
		
		long size = sizeof(bool);
		if (item.HasValue)
			size += ValueSerializer.CalculateSize(context, item.Value);

		return size;
	}

	public override void Serialize(T? item, EndianBinaryWriter writer, SerializationContext context) {
		if (item.HasValue) {
			writer.Write(true);
			ValueSerializer.Serialize(item.Value, writer, context);
		} else {
			writer.Write(false);
			if (IsConstantSize)
				writer.Write(_padding);
		}
	}

	public override T? Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var hasValue = reader.ReadBoolean();
		var result = hasValue ? new T?(ValueSerializer.Deserialize(reader, context)) : null;
		if (IsConstantSize)
			reader.ReadBytes(ValueSerializer.ConstantSize);
		return result;
	}
}
