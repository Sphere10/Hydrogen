using System.Collections.Generic;

namespace Sphere10.Framework;

public interface IMerkleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IMerkleCollection<KeyValuePair<TKey, TValue>> {
}
