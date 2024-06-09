// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class EnumerableKeyDictionary<K, V> : IDictionary<IEnumerable<K>, V> {

	#region Fields

	private readonly TreeHeader _tree;
	private volatile uint _surrogateKey;
	private bool _hasValue;
	private IDictionary<K, EnumerableKeyDictionary<K, V>> _childNodes;

	#endregion

	#region Constructors

	public EnumerableKeyDictionary() : this(EqualityComparer<K>.Default) {
	}

	public EnumerableKeyDictionary(IEqualityComparer<K> keyComparer) {
		_tree = new TreeHeader(this, keyComparer);
		Initialize();
	}

	private EnumerableKeyDictionary(EnumerableKeyDictionary<K, V> parent) {
		_tree = parent._tree;
		Initialize();
	}

	#endregion

	#region Convenience Methods

	public V this[params K[] key] {
		get => this[key.AsEnumerable()];
		set => this[key.AsEnumerable()] = value;
	}

	public void Add(V value, params K[] key) {
		Add(key, value);
	}

	public bool ContainsKey(params K[] key) {
		return ContainsKey(key.AsEnumerable());
	}

	public bool TryGetValue(out V value, params K[] key) {
		return TryGetValue(key.AsEnumerable(), out value);
	}

	#endregion

	#region IDictionary implementation

	public virtual void Add(IEnumerable<K> key, V value) {
		SetInternal(key, value);
	}

	public virtual bool ContainsKey(IEnumerable<K> key) {
		V v = default(V);
		return GetInternal(key, ref v);
	}

	public virtual ICollection<IEnumerable<K>> Keys => _tree.KeyMap.Values;

	public virtual bool Remove(IEnumerable<K> key) {
		return Del(key);
	}

	public virtual bool TryGetValue(IEnumerable<K> key, out V value) {
		value = default(V);
		return GetInternal(key, ref value);
	}

	public virtual ICollection<V> Values => _tree.ValueMap.Values;

	public virtual V this[IEnumerable<K> key] {
		get {
			if (!TryGetValue(key, out var val))
				throw new ArgumentOutOfRangeException(nameof(key));
			return val;
		}
		set => Add(key, value);
	}

	public virtual void Add(KeyValuePair<IEnumerable<K>, V> item) {
		Add(item.Key, item.Value);
	}

	public void Clear() {
		foreach (var child in _childNodes.Values)
			child.Clear();
		_tree.KeyMap.Remove(_surrogateKey);
		_tree.ValueMap.Remove(_surrogateKey);
		_childNodes.Clear();
	}

	public virtual bool Contains(KeyValuePair<IEnumerable<K>, V> item) {
		var val = default(V);
		return GetInternal(item.Key, ref val);
	}

	public virtual void CopyTo(KeyValuePair<IEnumerable<K>, V>[] array, int arrayIndex) {
		if (array == null)
			throw new ArgumentNullException(nameof(array));

		if (arrayIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));

		var itemsToCopy = GetKeyValuePairs();

		if (array.Length - arrayIndex < itemsToCopy.Count())
			throw new ArgumentException("The number of elements is greater than available space from index to end of destination array", "array");

		foreach (var item in itemsToCopy)
			array[++arrayIndex] = item;

	}

	public virtual int Count => _tree.ValueMap.Count;

	public bool IsReadOnly => false;

	public virtual bool Remove(KeyValuePair<IEnumerable<K>, V> item) {
		return Del(item.Key);
	}

	public virtual IEnumerator<KeyValuePair<IEnumerable<K>, V>> GetEnumerator() {
		return GetKeyValuePairs().GetEnumerator();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
		return GetKeyValuePairs().GetEnumerator();
	}

	#endregion

	#region Auxillary

	private void Initialize() {
		_childNodes = new Dictionary<K, EnumerableKeyDictionary<K, V>>(_tree.KeyComparer);
		_hasValue = false;
		_surrogateKey = _tree.NextSurrogateID();
	}

	private bool GetInternal(IEnumerable<K> keys, ref V val) {
		if (!keys.Any()) {
			if (_hasValue) {
				val = _tree.ValueMap[_surrogateKey];
				return true;
			}
			return false;
		}

		var head = keys.Take(1).Single();
		if (typeof(K) == typeof(object)) {
			head = (K)Tools.Object.SanitizeObject(head);
		}
		var tail = keys.Skip(1);
		return _childNodes.ContainsKey(head) && _childNodes[head].GetInternal(tail, ref val);
	}

	private void SetInternal(IEnumerable<K> keys, V val, Queue<K> traversalList = null) {
		if (!keys.Any())
			throw new ArgumentException("Keys was empty", nameof(keys));

		if (traversalList == null)
			traversalList = new Queue<K>();

		var head = keys.Take(1).Single();
		if (typeof(K) == typeof(object)) {
			head = (K)Tools.Object.SanitizeObject(head);
		}
		var tail = keys.Skip(1);

		EnumerableKeyDictionary<K, V> childNode;
		if (_childNodes.ContainsKey(head)) {
			childNode = _childNodes[head];
		} else {
			childNode = new EnumerableKeyDictionary<K, V>(this);
			_childNodes.Add(head, childNode);
		}
		traversalList.Enqueue(head);
		if (tail.Any()) {
			childNode.SetInternal(tail, val, traversalList);
		} else {
			childNode._hasValue = true;
			_tree.ValueMap[childNode._surrogateKey] = val;
			_tree.KeyMap[childNode._surrogateKey] = (from x in traversalList select x).ToArray();
		}
		traversalList.Dequeue();
	}


	private bool Del(IEnumerable<K> keys) {
		if (!keys.Any())
			throw new ArgumentException("Keys was empty", "keys");

		var head = keys.Take(1).Single();
		var tail = keys.Skip(1);
		if (_childNodes.ContainsKey(head)) {
			var childNode = _childNodes[head];
			if (tail.Any()) {
				return childNode.Del(tail);
			}
			if (childNode._hasValue) {
				childNode._hasValue = false;
				_tree.KeyMap.Remove(childNode._surrogateKey);
				_tree.ValueMap.Remove(childNode._surrogateKey);
				return true;
			}
		}
		return false;
	}

	private IEnumerable<KeyValuePair<IEnumerable<K>, V>> GetKeyValuePairs() {
		return
			from surrogateKey in _tree.KeyMap.Keys
			let key = _tree.KeyMap[surrogateKey]
			let val = _tree.ValueMap[surrogateKey]
			select new KeyValuePair<IEnumerable<K>, V>(key, val);
	}

	#endregion

	#region TreeHeader inner class

	private class TreeHeader {
		public readonly EnumerableKeyDictionary<K, V> RootNode;
		public readonly IEqualityComparer<K> KeyComparer;
		public readonly IDictionary<uint, V> ValueMap;
		public readonly IDictionary<uint, IEnumerable<K>> KeyMap;
		private volatile uint _identitySeed;
		private readonly object _threadLock;


		public TreeHeader(EnumerableKeyDictionary<K, V> rootNode, IEqualityComparer<K> keyComparer) {
			RootNode = rootNode;
			KeyComparer = keyComparer ?? EqualityComparer<K>.Default;
			ValueMap = new Dictionary<uint, V>();
			KeyMap = new Dictionary<uint, IEnumerable<K>>();
			_threadLock = new object();
			_identitySeed = 0;
		}

		public uint NextSurrogateID() {
			lock (_threadLock) {
				return _identitySeed++;
			}
		}
	}

	#endregion

}
