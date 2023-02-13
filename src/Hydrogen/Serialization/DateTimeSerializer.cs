using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hydrogen.FastReflection;

namespace Hydrogen;
public class DateTimeSerializer : StaticSizeItemSerializerBase<DateTime> {
	private readonly PrimitiveSerializer<long> _longSerializer = new();

	public DateTimeSerializer() : base(8) {
	}

	public override bool TrySerialize(DateTime item, EndianBinaryWriter writer) 
		=> _longSerializer.TrySerialize(item.ToBinary(), writer);

	public override bool TryDeserialize(EndianBinaryReader reader, out DateTime item) {
		if (_longSerializer.TryDeserialize(reader, out var binVal)) {
			item = default;
			return false;
		}
		item = DateTime.FromBinary(binVal);
		return true;
	}
}
