using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class ProjectedReadOnlyDictionary<TKey, TValue, TProjectedKey, TProjectedValue> : IReadOnlyDictionary<TProjectedKey, TProjectedValue> {
	private readonly IReadOnlyDictionary<TKey, TValue> _source;
	private readonly Func<TKey, TProjectedKey> _keyProjection;
	private readonly Func<TProjectedKey, TKey> _inverseKeyProjection;
	private readonly Func<TValue, TProjectedValue> _valueProjection;

	public ProjectedReadOnlyDictionary(IReadOnlyDictionary<TKey, TValue> source, Func<TKey, TProjectedKey> keyProjection, Func<TProjectedKey, TKey> inverseKeyProjection, Func<TValue, TProjectedValue> valueProjection) {
		_source = source;
		_keyProjection = keyProjection;
		_inverseKeyProjection = inverseKeyProjection;
		_valueProjection = valueProjection;
	}

	public int Count => _source.Count;

	public IEnumerable<TProjectedKey> Keys => _source.Keys.Select(_keyProjection);

	public IEnumerable<TProjectedValue> Values => _source.Values.Select(_valueProjection);

	public bool ContainsKey(TProjectedKey key) => _source.ContainsKey(_inverseKeyProjection(key));

	public bool TryGetValue(TProjectedKey key, out TProjectedValue value) {
		if (!_source.TryGetValue(_inverseKeyProjection(key), out var sourceValue)) {
			value = default!;
			return false;
		}
		value = _valueProjection(sourceValue);
		return true;
	}

	public IEnumerator<KeyValuePair<TProjectedKey, TProjectedValue>> GetEnumerator() {
		foreach (var kvp in _source) {
			yield return kvp.AsProjection(_keyProjection, _valueProjection);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public TProjectedValue this[TProjectedKey key] => _valueProjection(_source[_inverseKeyProjection(key)]);

}
