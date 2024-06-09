// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class PrimitiveSerializer<T> : ConstantSizeItemSerializerBase<T> where T : struct {
	private readonly Action<EndianBinaryWriter, T> _writePrimitive;
	private readonly Func<EndianBinaryReader, T> _readPrimitive;

	public static PrimitiveSerializer<T> Instance { get; } = new();

	public PrimitiveSerializer()
		: base(Tools.Memory.SizeOfPrimitive(typeof(T)), false) {
		Guard.Argument(Tools.Memory.IsSerializationPrimitive(typeof(T)), nameof(T), $"{typeof(T)} is not a primitive type");
		var typeCode = Type.GetTypeCode(typeof(T));
		_writePrimitive = GetPrimitiveWriter(typeCode);
		_readPrimitive = GetPrimitiveReader(typeCode);
	}

	public override void Serialize(T item, EndianBinaryWriter writer, SerializationContext context)
		=> _writePrimitive(writer, item);

	public override T Deserialize(EndianBinaryReader reader, SerializationContext context)
		=> _readPrimitive(reader);

	public static Action<EndianBinaryWriter, T> GetPrimitiveWriter(TypeCode typeCode) => typeCode switch {
		TypeCode.Boolean => (writer, item) => writer.Write((bool)(object)item),
		TypeCode.Byte => (writer, item) => writer.Write((byte)(object)item),
		TypeCode.Char => (writer, item) => writer.Write((char)(object)item),
		TypeCode.Decimal => (writer, item) => writer.Write((decimal)(object)item),
		TypeCode.Double => (writer, item) => writer.Write((double)(object)item),
		TypeCode.Int16 => (writer, item) => writer.Write((short)(object)item),
		TypeCode.Int32 => (writer, item) => writer.Write((int)(object)item),
		TypeCode.Int64 => (writer, item) => writer.Write((long)(object)item),
		TypeCode.SByte => (writer, item) => writer.Write((sbyte)(object)item),
		TypeCode.Single => (writer, item) => writer.Write((float)(object)item),
		TypeCode.UInt16 => (writer, item) => writer.Write((ushort)(object)item),
		TypeCode.UInt32 => (writer, item) => writer.Write((uint)(object)item),
		TypeCode.UInt64 => (writer, item) => writer.Write((ulong)(object)item),
		_ => throw new NotSupportedException($"{nameof(typeCode)}")
	};

	public static Func<EndianBinaryReader, T> GetPrimitiveReader(TypeCode typeCode) => typeCode switch {
		TypeCode.Boolean => reader => (T)(object)reader.ReadBoolean(),
		TypeCode.Byte => reader => (T)(object)reader.ReadByte(),
		TypeCode.Char => reader => (T)(object)reader.Read(),
		TypeCode.Decimal => reader => (T)(object)reader.ReadDecimal(),
		TypeCode.Double => reader => (T)(object)reader.ReadDouble(),
		TypeCode.Int16 => reader => (T)(object)reader.ReadInt16(),
		TypeCode.Int32 => reader => (T)(object)reader.ReadInt32(),
		TypeCode.Int64 => reader => (T)(object)reader.ReadInt64(),
		TypeCode.SByte => reader => (T)(object)reader.ReadSByte(),
		TypeCode.Single => reader => (T)(object)reader.ReadSingle(),
		TypeCode.UInt16 => reader => (T)(object)reader.ReadUInt16(),
		TypeCode.UInt32 => reader => (T)(object)reader.ReadUInt32(),
		TypeCode.UInt64 => reader => (T)(object)reader.ReadUInt64(),
		_ => throw new NotSupportedException($"{nameof(typeCode)}")
	};

}