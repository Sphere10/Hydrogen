using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class SetInterfaceSerializer<TItem> : ItemSerializerDecorator<ISet<TItem>> {

	public SetInterfaceSerializer(IItemSerializer<TItem> itemSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<TItem>(itemSerializer, sizeDescriptorStrategy).AsProjection(x => (ISet<TItem>)x.ToHashSet(), x => x.ToArray())) {
	}

}