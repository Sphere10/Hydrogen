using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class DictionaryInterfaceSerializer<TKey, TValue> : ItemSerializerDecorator<IDictionary<TKey, TValue>> {

	public DictionaryInterfaceSerializer(KeyValuePairSerializer<TKey, TValue> kvpSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<KeyValuePair<TKey, TValue>>(kvpSerializer, sizeDescriptorStrategy).AsProjection(x => (IDictionary<TKey, TValue>)x.ToDictionary(), x => x.ToArray())) {
	}

}

public class DictionarySerializer<TKey, TValue> : ItemSerializerDecorator<Dictionary<TKey, TValue>> {

	public DictionarySerializer(KeyValuePairSerializer<TKey, TValue> kvpSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(new ArraySerializer<KeyValuePair<TKey, TValue>>(kvpSerializer, sizeDescriptorStrategy).AsProjection(x => x.ToDictionary(), x => x.ToArray())) {
	}

}