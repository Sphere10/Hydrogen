using System.Collections.ObjectModel;
using System.Linq;

namespace Hydrogen;

public class CollectionSerializer<T> : ItemSerializerDecorator<Collection<T>> {

	public CollectionSerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<T>(valueSerializer, sizeDescriptorStrategy).AsProjection(x => new Collection<T>(x), x => x.ToArray())) {
	}

}