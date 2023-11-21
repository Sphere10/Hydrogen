using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class HashSetSerializer<TItem> : ItemSerializerDecorator<HashSet<TItem>> {

	public HashSetSerializer(IItemSerializer<TItem> itemSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<TItem>(itemSerializer, sizeDescriptorStrategy).AsProjection(x => x.ToHashSet(), x => x.ToArray())) {
	}

}
