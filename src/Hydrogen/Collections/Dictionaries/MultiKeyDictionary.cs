// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Hydrogen;

public class MultiKeyDictionary<K1, K2, V> : Dictionary<K1, Dictionary<K2, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer ?? EqualityComparer<K2>.Default;
	}

	public V this[K1 key1, K2 key2] {
		get {
			if (!ContainsKey(key1) || !this[key1].ContainsKey(key2))
				throw new ArgumentOutOfRangeException();
			return base[key1][key2];
		}
		set {
			if (!ContainsKey(key1))
				this[key1] = new Dictionary<K2, V>(_key2Comparer);
			this[key1][key2] = value;
		}
	}

	public void Add(K1 key1, K2 key2, V value) {
		if (!ContainsKey(key1))
			this[key1] = new Dictionary<K2, V>(_key2Comparer);
		this[key1][key2] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2);
	}

	public bool TryGetValue(K1 key1, K2 key2, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, out value);
	}

	public new IEnumerable<(K1, K2)> Keys => 
		from k1 in base.Keys
		from k2 in this[k1].Keys
		select (k1, k2);

	public new IEnumerable<V> Values 
		=> base.Values.SelectMany(x => x.Values);
}


public class MultiKeyDictionary<K1, K2, K3, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer ?? EqualityComparer<K2>.Default;
		_key3Comparer = key3Comparer ?? EqualityComparer<K3>.Default;
	}

	public V this[K1 key1, K2 key2, K3 key3] {
		get => ContainsKey(key1) ? this[key1][key2, key3] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, V>(_key2Comparer, _key3Comparer);
			this[key1][key2, key3] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, V>(_key2Comparer, _key3Comparer);
		this[key1][key2][key3] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, out value);
	}

	public new IEnumerable<(K1, K2, K3)> Keys => 
		from k1 in base.Keys
		from tail in this[k1].Keys
		select (k1, tail.Item1, tail.Item2);

	public new IEnumerable<V> Values 
		=> base.Values.SelectMany(x => x.Values);
}


public class MultiKeyDictionary<K1, K2, K3, K4, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, K4, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;
	private readonly IEqualityComparer<K4> _key4Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer ?? EqualityComparer<K2>.Default;
		_key3Comparer = key3Comparer ?? EqualityComparer<K3>.Default;
		_key4Comparer = key4Comparer ?? EqualityComparer<K4>.Default;
	}

	public V this[K1 key1, K2 key2, K3 key3, K4 key4] {
		get => ContainsKey(key1) ? this[key1][key2, key3, key4] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, K4, V>(_key2Comparer, _key3Comparer, _key4Comparer);
			this[key1][key2, key3, key4] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, K4 key4, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, K4, V>(_key2Comparer, _key3Comparer, _key4Comparer);
		this[key1][key2][key3][key4] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3, key4);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, key4, out value);
	}

	public new IEnumerable<(K1, K2, K3, K4)> Keys => 
		from k1 in base.Keys
		from tail in this[k1].Keys
		select (k1, tail.Item1, tail.Item2, tail.Item3);

	public new IEnumerable<V> Values 
		=> base.Values.SelectMany(x => x.Values);
}


public class MultiKeyDictionary<K1, K2, K3, K4, K5, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, K4, K5, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;
	private readonly IEqualityComparer<K4> _key4Comparer;
	private readonly IEqualityComparer<K5> _key5Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null,
	                          IEqualityComparer<K5> key5Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer ?? EqualityComparer<K2>.Default;
		_key3Comparer = key3Comparer ?? EqualityComparer<K3>.Default;
		_key4Comparer = key4Comparer ?? EqualityComparer<K4>.Default;
		_key5Comparer = key5Comparer ?? EqualityComparer<K5>.Default;
	}

	public V this[K1 key1, K2 key2, K3 key3, K4 key4, K5 key5] {
		get => ContainsKey(key1) ? this[key1][key2, key3, key4, key5] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer);
			this[key1][key2, key3, key4, key5] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer);
		this[key1][key2][key3][key4][key5] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3, key4, key5);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, key4, key5, out value);
	}
}


public class MultiKeyDictionary<K1, K2, K3, K4, K5, K6, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, K4, K5, K6, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;
	private readonly IEqualityComparer<K4> _key4Comparer;
	private readonly IEqualityComparer<K5> _key5Comparer;
	private readonly IEqualityComparer<K6> _key6Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null, IEqualityComparer<K5> key5Comparer = null,
	                          IEqualityComparer<K6> key6Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer ?? EqualityComparer<K2>.Default;
		_key3Comparer = key3Comparer ?? EqualityComparer<K3>.Default;
		_key4Comparer = key4Comparer ?? EqualityComparer<K4>.Default;
		_key5Comparer = key5Comparer ?? EqualityComparer<K5>.Default;
		_key6Comparer = key6Comparer ?? EqualityComparer<K6>.Default;
	}

	public V this[K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6] {
		get => ContainsKey(key1) ? this[key1][key2, key3, key4, key5, key6] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer);
			this[key1][key2, key3, key4, key5, key6] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer);
		this[key1][key2][key3][key4][key5][key6] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3, key4, key5, key6);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, key4, key5, key6, out value);
	}
}


public class MultiKeyDictionary<K1, K2, K3, K4, K5, K6, K7, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, K4, K5, K6, K7, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;
	private readonly IEqualityComparer<K4> _key4Comparer;
	private readonly IEqualityComparer<K5> _key5Comparer;
	private readonly IEqualityComparer<K6> _key6Comparer;
	private readonly IEqualityComparer<K7> _key7Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null, IEqualityComparer<K5> key5Comparer = null,
	                          IEqualityComparer<K6> key6Comparer = null, IEqualityComparer<K7> key7Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer;
		_key3Comparer = key3Comparer;
		_key4Comparer = key4Comparer;
		_key5Comparer = key5Comparer;
		_key6Comparer = key6Comparer;
		_key7Comparer = key7Comparer;
	}

	public V this[K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7] {
		get => ContainsKey(key1) ? this[key1][key2, key3, key4, key5, key6, key7] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer);
			this[key1][key2, key3, key4, key5, key6, key7] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer);
		this[key1][key2][key3][key4][key5][key6][key7] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3, key4, key5, key6, key7);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, key4, key5, key6, key7, out value);
	}
}


public class MultiKeyDictionary<K1, K2, K3, K4, K5, K6, K7, K8, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;
	private readonly IEqualityComparer<K4> _key4Comparer;
	private readonly IEqualityComparer<K5> _key5Comparer;
	private readonly IEqualityComparer<K6> _key6Comparer;
	private readonly IEqualityComparer<K7> _key7Comparer;
	private readonly IEqualityComparer<K8> _key8Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null, IEqualityComparer<K5> key5Comparer = null,
	                          IEqualityComparer<K6> key6Comparer = null, IEqualityComparer<K7> key7Comparer = null, IEqualityComparer<K8> key8Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer;
		_key3Comparer = key3Comparer;
		_key4Comparer = key4Comparer;
		_key5Comparer = key5Comparer;
		_key6Comparer = key6Comparer;
		_key7Comparer = key7Comparer;
		_key8Comparer = key8Comparer;
	}

	public V this[K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8] {
		get => ContainsKey(key1) ? this[key1][key2, key3, key4, key5, key6, key7, key8] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer, _key8Comparer);
			this[key1][key2, key3, key4, key5, key6, key7, key8] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer, _key8Comparer);
		this[key1][key2][key3][key4][key5][key6][key7][key8] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3, key4, key5, key6, key7, key8);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, key4, key5, key6, key7, key8, out value);
	}
}


public class MultiKeyDictionary<K1, K2, K3, K4, K5, K6, K7, K8, K9, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;
	private readonly IEqualityComparer<K4> _key4Comparer;
	private readonly IEqualityComparer<K5> _key5Comparer;
	private readonly IEqualityComparer<K6> _key6Comparer;
	private readonly IEqualityComparer<K7> _key7Comparer;
	private readonly IEqualityComparer<K8> _key8Comparer;
	private readonly IEqualityComparer<K9> _key9Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null, IEqualityComparer<K5> key5Comparer = null,
	                          IEqualityComparer<K6> key6Comparer = null, IEqualityComparer<K7> key7Comparer = null, IEqualityComparer<K8> key8Comparer = null, IEqualityComparer<K9> key9Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer;
		_key3Comparer = key3Comparer;
		_key4Comparer = key4Comparer;
		_key5Comparer = key5Comparer;
		_key6Comparer = key6Comparer;
		_key7Comparer = key7Comparer;
		_key8Comparer = key8Comparer;
		_key9Comparer = key9Comparer;
	}

	public V this[K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9] {
		get => ContainsKey(key1) ? this[key1][key2, key3, key4, key5, key6, key7, key8, key9] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer, _key8Comparer, _key9Comparer);
			this[key1][key2, key3, key4, key5, key6, key7, key8, key9] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer, _key8Comparer, _key9Comparer);
		this[key1][key2][key3][key4][key5][key6][key7][key8][key9] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3, key4, key5, key6, key7, key8, key9);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, key4, key5, key6, key7, key8, key9, out value);
	}
}


public class MultiKeyDictionary<K1, K2, K3, K4, K5, K6, K7, K8, K9, K10, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, K10, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;
	private readonly IEqualityComparer<K4> _key4Comparer;
	private readonly IEqualityComparer<K5> _key5Comparer;
	private readonly IEqualityComparer<K6> _key6Comparer;
	private readonly IEqualityComparer<K7> _key7Comparer;
	private readonly IEqualityComparer<K8> _key8Comparer;
	private readonly IEqualityComparer<K9> _key9Comparer;
	private readonly IEqualityComparer<K10> _key10Comparer;


	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null, IEqualityComparer<K5> key5Comparer = null,
	                          IEqualityComparer<K6> key6Comparer = null, IEqualityComparer<K7> key7Comparer = null, IEqualityComparer<K8> key8Comparer = null, IEqualityComparer<K9> key9Comparer = null,
	                          IEqualityComparer<K10> key10Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer;
		_key3Comparer = key3Comparer;
		_key4Comparer = key4Comparer;
		_key5Comparer = key5Comparer;
		_key6Comparer = key6Comparer;
		_key7Comparer = key7Comparer;
		_key8Comparer = key8Comparer;
		_key9Comparer = key9Comparer;
		_key10Comparer = key10Comparer;
	}

	public V this[K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, K10 key10] {
		get => ContainsKey(key1) ? this[key1][key2, key3, key4, key5, key6, key7, key8, key9, key10] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, K10, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer, _key8Comparer, _key9Comparer, _key10Comparer);
			this[key1][key2, key3, key4, key5, key6, key7, key8, key9, key10] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, K10 key10, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, K10, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer, _key8Comparer, _key9Comparer, _key10Comparer);
		this[key1][key2][key3][key4][key5][key6][key7][key8][key9][key10] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, K10 key10) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3, key4, key5, key6, key7, key8, key9, key10);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, K10 key10, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, key4, key5, key6, key7, key8, key9, key10, out value);
	}
}


public class MultiKeyDictionary<K1, K2, K3, K4, K5, K6, K7, K8, K9, K10, K11, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, K10, K11, V>> {
	private readonly IEqualityComparer<K2> _key2Comparer;
	private readonly IEqualityComparer<K3> _key3Comparer;
	private readonly IEqualityComparer<K4> _key4Comparer;
	private readonly IEqualityComparer<K5> _key5Comparer;
	private readonly IEqualityComparer<K6> _key6Comparer;
	private readonly IEqualityComparer<K7> _key7Comparer;
	private readonly IEqualityComparer<K8> _key8Comparer;
	private readonly IEqualityComparer<K9> _key9Comparer;
	private readonly IEqualityComparer<K10> _key10Comparer;
	private readonly IEqualityComparer<K11> _key11Comparer;

	public MultiKeyDictionary(IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null, IEqualityComparer<K5> key5Comparer = null,
	                          IEqualityComparer<K6> key6Comparer = null, IEqualityComparer<K7> key7Comparer = null, IEqualityComparer<K8> key8Comparer = null, IEqualityComparer<K9> key9Comparer = null, IEqualityComparer<K10> key10Comparer = null,
	                          IEqualityComparer<K11> key11Comparer = null) : base(key1Comparer) {
		_key2Comparer = key2Comparer;
		_key3Comparer = key3Comparer;
		_key4Comparer = key4Comparer;
		_key5Comparer = key5Comparer;
		_key6Comparer = key6Comparer;
		_key7Comparer = key7Comparer;
		_key8Comparer = key8Comparer;
		_key9Comparer = key9Comparer;
		_key10Comparer = key10Comparer;
		_key11Comparer = key11Comparer;
	}

	public V this[K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, K10 key10, K11 key11] {
		get => ContainsKey(key1) ? this[key1][key2, key3, key4, key5, key6, key7, key8, key9, key10, key11] : default(V);
		set {
			if (!ContainsKey(key1))
				this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, K10, K11, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer, _key8Comparer, _key9Comparer, _key10Comparer, _key11Comparer);
			this[key1][key2, key3, key4, key5, key6, key7, key8, key9, key10, key11] = value;
		}
	}

	public void Add(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, K10 key10, K11 key11, V value) {
		if (!ContainsKey(key1))
			this[key1] = new MultiKeyDictionary<K2, K3, K4, K5, K6, K7, K8, K9, K10, K11, V>(_key2Comparer, _key3Comparer, _key4Comparer, _key5Comparer, _key6Comparer, _key7Comparer, _key8Comparer, _key9Comparer, _key10Comparer, _key11Comparer);
		this[key1][key2][key3][key4][key5][key6][key7][key8][key9][key10][key11] = value;
	}

	public bool ContainsKey(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, K10 key10, K11 key11) {
		return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3, key4, key5, key6, key7, key8, key9, key10, key11);
	}

	public bool TryGetValue(K1 key1, K2 key2, K3 key3, K4 key4, K5 key5, K6 key6, K7 key7, K8 key8, K9 key9, K10 key10, K11 key11, out V value) {
		value = default;
		return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, key3, key4, key5, key6, key7, key8, key9, key10, key11, out value);
	}
}
