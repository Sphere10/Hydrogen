using System.Collections.Generic;

namespace Hydrogen;

public interface IMerkleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IMerkleCollection<KeyValuePair<TKey, TValue>> {
}
