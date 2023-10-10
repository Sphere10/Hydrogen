using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Tests.Collections.StreamMapped;
internal class BrokenConstantLengthSerializer<T> : ItemSerializerDecorator<T> {
	public BrokenConstantLengthSerializer(IItemSerializer<T> internalSerializer) : base(internalSerializer) {
	}

	public override void Serialize(T item, EndianBinaryWriter writer, SerializationContext context) {
		base.Serialize(item, writer, context);
		writer.Write(0); // write an extra byte where there shouldn't be one
	}
}
