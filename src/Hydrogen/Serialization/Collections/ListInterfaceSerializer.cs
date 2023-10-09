using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class ListInterfaceSerializer<T> : ItemSerializerDecorator<IList<T>> {

	public ListInterfaceSerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<T>(valueSerializer, sizeDescriptorStrategy).AsProjection(x => (IList<T>)x.ToList(), x => x.ToArray())) {
	}

}
