using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Tests.Collections.StreamMapped;
internal class BrokenConstantLengthSerializer<T> : ItemSerializerDecorator<T> {
	public BrokenConstantLengthSerializer(IItemSerializer<T> internalSerializer) : base(internalSerializer) {
	}

	public override bool TrySerialize(T item, EndianBinaryWriter writer, out long bytesWritten) {
		if (base.TrySerialize(item, writer, out bytesWritten)) {
			writer.Write(0); // write an extra byte where there shouldn't be one
			return true;
		}
		return false;
	}
}
