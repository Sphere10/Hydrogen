// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class EnumSerializer<T> : ConstantSizeItemSerializerBase<T> where T : Enum {
	private readonly Action<SerializationContext, EndianBinaryWriter, T> _writePrimitive;
	private readonly Func<SerializationContext, EndianBinaryReader, T> _readPrimitive;

	public EnumSerializer()
		: base(GetPrimitiveCorrectSerializer(typeof(T), out var writer, out var reader, out _), false) {
		_writePrimitive = writer;
		_readPrimitive = reader;
	}

	public static EnumSerializer<T> Instance { get; } = new();

	public override void Serialize(T item, EndianBinaryWriter writer, SerializationContext context)
		=> _writePrimitive(context, writer, item);

	public override T Deserialize(EndianBinaryReader reader, SerializationContext context)
		=> _readPrimitive(context, reader);

	private static long GetPrimitiveCorrectSerializer(Type type, out Action<SerializationContext, EndianBinaryWriter, T> writer, out Func<SerializationContext, EndianBinaryReader, T> reader, out TypeCode enumTypeCode) {
		enumTypeCode = Type.GetTypeCode(Enum.GetUnderlyingType(type));
		switch (enumTypeCode) {
			case TypeCode.Byte:
				writer = (context, writer, item) => PrimitiveSerializer<byte>.Instance.Serialize((byte)(object)item, writer, context);
				reader = (context, reader) => (T)(object)PrimitiveSerializer<byte>.Instance.Deserialize(reader, context);
				return PrimitiveSerializer<byte>.Instance.ConstantSize;
			case TypeCode.SByte:
				writer = (context, writer, item) => PrimitiveSerializer<sbyte>.Instance.Serialize((sbyte)(object)item, writer, context);
				reader = (context, reader) => (T)(object)PrimitiveSerializer<sbyte>.Instance.Deserialize(reader, context);
				return PrimitiveSerializer<sbyte>.Instance.ConstantSize;
			case TypeCode.UInt16:
				writer = (context, writer, item) => PrimitiveSerializer<ushort>.Instance.Serialize((ushort)(object)item, writer, context);
				reader = (context, reader) => (T)(object)PrimitiveSerializer<ushort>.Instance.Deserialize(reader, context);
				return PrimitiveSerializer<ushort>.Instance.ConstantSize;
			case TypeCode.UInt32:
				writer = (context, writer, item) => PrimitiveSerializer<uint>.Instance.Serialize((uint)(object)item, writer, context);
				reader = (context, reader) => (T)(object)PrimitiveSerializer<uint>.Instance.Deserialize(reader, context);
				return PrimitiveSerializer<uint>.Instance.ConstantSize;
			case TypeCode.UInt64:
				writer = (context, writer, item) => PrimitiveSerializer<ulong>.Instance.Serialize((ulong)(object)item, writer, context);
				reader = (context, reader) => (T)(object)PrimitiveSerializer<ulong>.Instance.Deserialize(reader, context);
				return PrimitiveSerializer<ulong>.Instance.ConstantSize;
			case TypeCode.Int16:
				writer = (context, writer, item) => PrimitiveSerializer<short>.Instance.Serialize((short)(object)item, writer, context);
				reader = (context, reader) => (T)(object)PrimitiveSerializer<short>.Instance.Deserialize(reader, context);
				return PrimitiveSerializer<short>.Instance.ConstantSize;
			case TypeCode.Int32:
				writer = (context, writer, item) => PrimitiveSerializer<int>.Instance.Serialize((int)(object)item, writer, context);
				reader = (context, reader) => (T)(object)PrimitiveSerializer<int>.Instance.Deserialize(reader, context);
				return PrimitiveSerializer<int>.Instance.ConstantSize;
			case TypeCode.Int64:
				writer = (context, writer, item) => PrimitiveSerializer<long>.Instance.Serialize((long)(object)item, writer, context);
				reader = (context, reader) => (T)(object)PrimitiveSerializer<long>.Instance.Deserialize(reader, context);
				return PrimitiveSerializer<long>.Instance.ConstantSize;
			default:
				throw new NotSupportedException($"{nameof(enumTypeCode)}");
		}
	}
}
