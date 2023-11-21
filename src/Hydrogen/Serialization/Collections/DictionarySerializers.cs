using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class DictionarySerializer<TKey, TValue> : ItemSerializerDecorator<Dictionary<TKey, TValue>> {

	public DictionarySerializer(IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt)
		: this(new KeyValuePairSerializer<TKey, TValue>(keySerializer, valueSerializer), sizeDescriptorStrategy) {
	}

	public DictionarySerializer(KeyValuePairSerializer<TKey, TValue> kvpSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<KeyValuePair<TKey, TValue>>(kvpSerializer, sizeDescriptorStrategy).AsProjection(x => x.ToDictionary(), x => x.ToArray())) {
	}

}