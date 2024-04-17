using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class ProjectedDictionary<TKey, TValue, TProjectedKey, TProjectedValue> : IDictionary<TProjectedKey, TProjectedValue> {
	private readonly IDictionary<TKey, TValue> _source;
	private readonly Func<TKey, TProjectedKey> _keyProjection;
	private readonly Func<TProjectedKey, TKey> _inverseKeyProjection;
	private readonly Func<TValue, TProjectedValue> _valueProjection;
	private readonly Func<TProjectedValue, TValue> _inverseValueProjection;

	public ProjectedDictionary(IDictionary<TKey, TValue> source, Func<TKey, TProjectedKey> keyProjection, Func<TProjectedKey, TKey> inverseKeyProjection,
	                           Func<TValue, TProjectedValue> valueProjection, Func<TProjectedValue, TValue> inverseValueProjection) {
		_source = source;
		_keyProjection = keyProjection;
		_inverseKeyProjection = inverseKeyProjection;
		_valueProjection = valueProjection;
		_inverseValueProjection = inverseValueProjection;
	}

	public int Count => _source.Count;

	public bool IsReadOnly => _source.IsReadOnly;

	public ICollection<TProjectedKey> Keys => _source.Keys.AsProjection(_keyProjection, _inverseKeyProjection);

	public ICollection<TProjectedValue> Values => _source.Values.AsProjection(_valueProjection, _inverseValueProjection);

	public void Add(TProjectedKey key, TProjectedValue value) => _source.Add(_inverseKeyProjection(key), _inverseValueProjection(value));

	public bool ContainsKey(TProjectedKey key) => _source.ContainsKey(_inverseKeyProjection(key));

	public bool TryGetValue(TProjectedKey key, out TProjectedValue value) {
		if (!_source.TryGetValue(_inverseKeyProjection(key), out var sourceValue)) {
			value = default!;
			return false;
		}
		value = _valueProjection(sourceValue);
		return true;
	}

	public void Add(KeyValuePair<TProjectedKey, TProjectedValue> item) => _source.Add(item.AsProjection(_inverseKeyProjection, _inverseValueProjection));

	public bool Contains(KeyValuePair<TProjectedKey, TProjectedValue> item) => _source.Contains(item.AsProjection(_inverseKeyProjection, _inverseValueProjection));

	public void CopyTo(KeyValuePair<TProjectedKey, TProjectedValue>[] array, int arrayIndex) => _source.Select(kvp => kvp.AsProjection(_keyProjection, _valueProjection)).ToArray().CopyTo(array, arrayIndex);

	public bool Remove(KeyValuePair<TProjectedKey, TProjectedValue> item) => _source.Remove(item.AsProjection(_inverseKeyProjection, _inverseValueProjection));

	public bool Remove(TProjectedKey item) => _source.Remove(_inverseKeyProjection(item));

	public void Clear() => _source.Clear();

	public IEnumerator<KeyValuePair<TProjectedKey, TProjectedValue>> GetEnumerator() {
		foreach (var kvp in _source) {
			yield return kvp.AsProjection(_keyProjection, _valueProjection);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public TProjectedValue this[TProjectedKey key] {
		get => _valueProjection(_source[_inverseKeyProjection(key)]);
		set => _source[_inverseKeyProjection(key)] = _inverseValueProjection(value);
	}

}
