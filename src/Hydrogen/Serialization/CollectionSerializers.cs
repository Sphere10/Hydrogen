using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hydrogen;

public class CollectionInterfaceSerializer<T> : ItemSerializerDecorator<ICollection<T>> {

	public CollectionInterfaceSerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<T>(valueSerializer, sizeDescriptorStrategy).AsProjection(x => (ICollection<T>)x.ToList(), x => x.ToArray())) {
	}

}

public class CollectionSerializer<T> : ItemSerializerDecorator<Collection<T>> {

	public CollectionSerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<T>(valueSerializer, sizeDescriptorStrategy).AsProjection(x => new Collection<T>(x), x => x.ToArray())) {
	}

}