using System;

namespace Sphere10.Framework {
	
	/// <summary>
	/// Serializer of decimal type for use with generic item serializer. Decimal is a value type not a primitive, and it cannot be
	/// serialized via reflection. Requires reading parts via method.
	/// </summary>
	public class DecimalSerializer : StaticSizeItemSerializer<decimal> {
		public DecimalSerializer() : base(sizeof(decimal)) {
		}

		public override bool TrySerialize(decimal item, EndianBinaryWriter writer) {
			writer.Write(item);
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out decimal item) {
			item = reader.ReadDecimal();
			return true;
		}
	}
}
