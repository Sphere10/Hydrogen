using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen;

public class DictionaryEqualityComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>>
{
	private readonly IEqualityComparer<TKey> _keyComparer;
	private readonly IEqualityComparer<TValue> _valueComparer;

	public DictionaryEqualityComparer() 
		: this(EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default) {}

	public DictionaryEqualityComparer(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
	{
		_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
		_valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
	}

	public bool Equals(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
	{
		if (x == y) return true;
		if (x == null || y == null) return false;
		if (x.Count != y.Count) return false;

		foreach (var kvp in x)
		{
			if (!y.TryGetValue(kvp.Key, out TValue valueInY)) return false;
			if (!_valueComparer.Equals(kvp.Value, valueInY)) return false;
		}

		// Check if y has any keys that x doesn't have
		foreach (var kvp in y)
		{
			if (!x.ContainsKey(kvp.Key)) return false;
		}

		return true;
	}

	public int GetHashCode(Dictionary<TKey, TValue> obj)
	{
		if (obj == null) return 0;
		int hash = 0;

		foreach (var kvp in obj)
		{
			hash ^= _keyComparer.GetHashCode(kvp.Key);
			hash ^= _valueComparer.GetHashCode(kvp.Value);
		}

		return hash;
	}
}