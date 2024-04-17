using System.Collections.Generic;

namespace Hydrogen;

public interface IReadOnlyDictionaryList<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyList<TValue> {
	int IndexOf(TKey key);
}
