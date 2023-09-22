// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class EnumSerializer<T> : ConstantSizeItemSerializerBase<T> where T : Enum {
	private readonly Action<EndianBinaryWriter, T> _writePrimitive;
	private readonly Func<EndianBinaryReader, T> _readPrimitive;

	public EnumSerializer()
		: base(GetPrimitiveCorrectSerializer(typeof(T), out var writer, out var reader, out _), false) {
		_writePrimitive = writer;
		_readPrimitive = reader;
	}

	public static EnumSerializer<T> Instance { get; } = new();

	public override void SerializeInternal(T item, EndianBinaryWriter writer)
		=> _writePrimitive(writer, item);

	public override T DeserializeInternal(EndianBinaryReader reader)
		=> _readPrimitive(reader);

	private static long GetPrimitiveCorrectSerializer(Type type, out Action<EndianBinaryWriter, T> writer, out Func<EndianBinaryReader, T> reader, out TypeCode enumTypeCode) {
		enumTypeCode = Type.GetTypeCode(Enum.GetUnderlyingType(type));
		switch (enumTypeCode) {
			case TypeCode.Byte:
				writer = (writer, item) => PrimitiveSerializer<byte>.Instance.Serialize((byte)(object)item, writer);
				reader = reader => (T)(object)PrimitiveSerializer<byte>.Instance.Deserialize(reader);
				return PrimitiveSerializer<byte>.Instance.ConstantSize;
			case TypeCode.SByte:
				writer = (writer, item) => PrimitiveSerializer<sbyte>.Instance.Serialize((sbyte)(object)item, writer);
				reader = reader => (T)(object)PrimitiveSerializer<sbyte>.Instance.Deserialize(reader);
				return PrimitiveSerializer<sbyte>.Instance.ConstantSize;
			case TypeCode.UInt16:
				writer = (writer, item) => PrimitiveSerializer<ushort>.Instance.Serialize((ushort)(object)item, writer);
				reader = reader => (T)(object)PrimitiveSerializer<ushort>.Instance.Deserialize(reader);
				return PrimitiveSerializer<ushort>.Instance.ConstantSize;
			case TypeCode.UInt32:
				writer = (writer, item) => PrimitiveSerializer<uint>.Instance.Serialize((uint)(object)item, writer);
				reader = reader => (T)(object)PrimitiveSerializer<uint>.Instance.Deserialize(reader);
				return PrimitiveSerializer<uint>.Instance.ConstantSize;
			case TypeCode.UInt64:
				writer = (writer, item) => PrimitiveSerializer<ulong>.Instance.Serialize((ulong)(object)item, writer);
				reader = reader => (T)(object)PrimitiveSerializer<ulong>.Instance.Deserialize(reader);
				return PrimitiveSerializer<ulong>.Instance.ConstantSize;
			case TypeCode.Int16:
				writer = (writer, item) => PrimitiveSerializer<short>.Instance.Serialize((short)(object)item, writer);
				reader = reader => (T)(object)PrimitiveSerializer<short>.Instance.Deserialize(reader);
				return PrimitiveSerializer<short>.Instance.ConstantSize;
			case TypeCode.Int32:
				writer = (writer, item) => PrimitiveSerializer<int>.Instance.Serialize((int)(object)item, writer);
				reader = reader => (T)(object)PrimitiveSerializer<int>.Instance.Deserialize(reader);
				return PrimitiveSerializer<int>.Instance.ConstantSize;
			case TypeCode.Int64:
				writer = (writer, item) => PrimitiveSerializer<long>.Instance.Serialize((long)(object)item, writer);
				reader = reader => (T)(object)PrimitiveSerializer<long>.Instance.Deserialize(reader);
				return PrimitiveSerializer<long>.Instance.ConstantSize;
			default:
				throw new NotSupportedException($"{nameof(enumTypeCode)}");
		}
	}
}
