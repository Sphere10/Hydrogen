// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public sealed class BijectiveDictionary<U, V> : DictionaryDecorator<U, V>, IBijectiveDictionary<U, V> {
	private readonly BijectiveDictionary<V, U> _bijection;

	public BijectiveDictionary() : this(EqualityComparer<U>.Default, EqualityComparer<V>.Default) {
	}

	public BijectiveDictionary(IEqualityComparer<U> equalityComparerU, IEqualityComparer<V> equalityComparerV)
		: this(new Dictionary<U, V>(equalityComparerU), new Dictionary<V, U>(equalityComparerV)) {
	}

	public BijectiveDictionary(IDictionary<U, V> internalDictionary, IDictionary<V, U> internalBijectiveDictionary) 
		: base(internalDictionary) {
		MakeSame(internalDictionary, internalBijectiveDictionary);
		_bijection = new BijectiveDictionary<V, U>(internalBijectiveDictionary, this);
	}

	public BijectiveDictionary(IDictionary<U, V> internalDictionary, BijectiveDictionary<V, U> bijection) 
		: base(internalDictionary) {
		Guard.ArgumentNotNull(bijection, nameof(bijection));
		MakeSame(internalDictionary, bijection.InternalDictionary);
		_bijection = bijection;
	}

	public override bool IsReadOnly => base.IsReadOnly && _bijection.InternalDictionary.IsReadOnly;

	public IBijectiveDictionary<V, U> Bijection => _bijection;

	public IDictionary<U,V> UnderlyingDictionary => InternalDictionary;

	public override void Add(U key, V value) {
		base.Add(key, value);
		_bijection.InternalDictionary.Add(value, key);
	}

	public override V this[U key] {
		get => base[key];
		set {
			var isUpdate = base.TryGetValue(key, out var oldValue);
			base[key] = value;
			if (isUpdate)
				_bijection.InternalDictionary.Remove(oldValue);
			_bijection.InternalDictionary[value] = key;
		}
	}

	public override void Add(KeyValuePair<U, V> item) {
		base.Add(item);
		_bijection.InternalDictionary.Add(item.ToInverse());
	}

	public override bool Remove(KeyValuePair<U, V> item) => base.Remove(item) && _bijection.InternalDictionary.Remove(item.ToInverse());

	public override bool Remove(U key) {
		if (TryGetValue(key, out var val))
			if (base.Remove(key))
				if (!_bijection.InternalDictionary.Remove(val))
					throw new InvalidOperationException("Failed to remove from bijection");
		return false;
	}

	public bool ContainsValue(V value) => _bijection.InternalDictionary.ContainsKey(value);

	public bool TryGetKey(V value, out U key) => _bijection.InternalDictionary.TryGetValue(value, out key);

	public override void Clear() {
		base.Clear();
		_bijection.InternalDictionary.Clear();
	}

	private void MakeSame(IDictionary<U, V> internalDictionary, IDictionary<V, U> internalBijectiveDictionary) {
		foreach (var kvp in internalDictionary) {
			if (!internalBijectiveDictionary.ContainsKey(kvp.Value))
				internalBijectiveDictionary.Add(kvp.Value, kvp.Key);
		}
		foreach (var kvp in internalBijectiveDictionary) {
			if (!internalDictionary.ContainsKey(kvp.Value))
				internalDictionary.Add(kvp.Value, kvp.Key);
		}
	}
}
