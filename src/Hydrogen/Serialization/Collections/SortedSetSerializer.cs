using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class SortedSetSerializer<TItem> : ItemSerializerDecorator<SortedSet<TItem>> {

	public SortedSetSerializer(IItemSerializer<TItem> itemSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<TItem>(itemSerializer, sizeDescriptorStrategy).AsProjection(x => x.ToSortedSet(), x => x.ToArray())) {
	}

}
