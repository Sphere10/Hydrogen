using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class ExtendedListInterfaceSerializer<T> : ItemSerializerDecorator<IExtendedList<T>> {

	public ExtendedListInterfaceSerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<T>(valueSerializer, sizeDescriptorStrategy).AsProjection(x => x.ToExtendedList(), x => x.ToArray())) {
	}
}
