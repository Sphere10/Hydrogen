using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class EnumerableSerializer<T> : ItemSerializerDecorator<IEnumerable<T>> {

	public EnumerableSerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<T>(valueSerializer, sizeDescriptorStrategy).AsProjection(x => (IEnumerable<T>)x, x => x.ToArray()).AsNullable()) {
	}

}
