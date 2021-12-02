using System;

namespace Sphere10.Framework {
	
	/// <summary>
	/// Serializer of decimal type for use with generic item serializer. Decimal is a value type not a primitive, and it cannot be
	/// serialized via reflection. Requires reading parts via method.
	/// </summary>
	public class DecimalSerializer : StaticSizeObjectSerializer<decimal> {
		public DecimalSerializer() : base(sizeof(decimal)) {
		}

		public override bool TrySerialize(decimal item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				writer.Write(item);
				bytesWritten = sizeof(decimal);
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out decimal item) {
			try {
				item = reader.ReadDecimal();
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}
}
