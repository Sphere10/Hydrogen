using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class DictionaryInterfaceSerializer<TKey, TValue> : ItemSerializerDecorator<IDictionary<TKey, TValue>> {

	public DictionaryInterfaceSerializer(IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt)
		: this(new KeyValuePairSerializer<TKey, TValue>(keySerializer, valueSerializer), sizeDescriptorStrategy) {
	}

	public DictionaryInterfaceSerializer(KeyValuePairSerializer<TKey, TValue> kvpSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<KeyValuePair<TKey, TValue>>(kvpSerializer, sizeDescriptorStrategy).AsProjection(x => (IDictionary<TKey, TValue>)x.ToDictionary(), x => x.ToArray())) {
	}

}
