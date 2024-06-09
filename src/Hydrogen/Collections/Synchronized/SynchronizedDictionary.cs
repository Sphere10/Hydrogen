// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Hydrogen;

public class SynchronizedDictionary<TKey, TValue, TInner> : DictionaryDecorator<TKey, TValue>, ISynchronizedObject where TInner : IDictionary<TKey, TValue> {

	private readonly ISynchronizedObject _lock;

	public SynchronizedDictionary(TInner internalDictionary, ISynchronizedObject @lock = null)
		: base(internalDictionary) {
		_lock = @lock ?? new SynchronizedObject();
	}

	public override ICollection<TKey> Keys {
		get {
			using (EnterReadScope())
				return base.Keys;
		}
	}

	public override ICollection<TValue> Values {
		get {
			using (EnterReadScope())
				return base.Values;
		}
	}

	public ISynchronizedObject ParentSyncObject {
		get => _lock.ParentSyncObject;
		set => _lock.ParentSyncObject = value;
	}

	public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

	public override void Add(TKey key, TValue value) {
		using (EnterWriteScope())
			base.Add(key, value);
	}

	public override bool ContainsKey(TKey key) {
		using (EnterReadScope())
			return base.ContainsKey(key);
	}

	public override bool TryGetValue(TKey key, out TValue value) {
		using (EnterReadScope())
			return base.TryGetValue(key, out value);
	}

	public override TValue this[TKey key] {
		get {
			using (EnterReadScope())
				return base[key];
		}
		set {
			using (EnterWriteScope())
				base[key] = value;
		}
	}

	public override void Add(KeyValuePair<TKey, TValue> item) {
		using (EnterWriteScope())
			base.Add(item);
	}

	public override bool Contains(KeyValuePair<TKey, TValue> item) {
		using (EnterReadScope())
			return base.Contains(item);
	}

	public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
		using (EnterReadScope())
			base.CopyTo(array, arrayIndex);
	}

	public override bool Remove(KeyValuePair<TKey, TValue> item) {
		using (EnterWriteScope())
			return base.Remove(item);
	}

	public override bool Remove(TKey item) {
		using (EnterWriteScope())
			return base.Remove(item);
	}

	public override void Clear() {
		using (EnterWriteScope())
			base.Clear();
	}

	public override int Count {
		get {
			using (EnterReadScope())
				return base.Count;
		}
	}

	public override bool IsReadOnly {
		get {
			using (EnterReadScope())
				return base.IsReadOnly;
		}
	}
	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		var readScope = EnterReadScope();
		return base.GetEnumerator().OnDispose(readScope.Dispose);
	}

	public IDisposable EnterReadScope() {
		return _lock.EnterReadScope();
	}

	public IDisposable EnterWriteScope() {
		return _lock.EnterWriteScope();
	}
}

public class SynchronizedDictionary<TKey, TValue> : SynchronizedDictionary<TKey, TValue, IDictionary<TKey, TValue>> {

	public SynchronizedDictionary()
		: this(new Dictionary<TKey, TValue>()) {
	}

	public SynchronizedDictionary(IEqualityComparer<TKey> keyComparer, ISynchronizedObject @lock = null)
		: base(new Dictionary<TKey, TValue>(keyComparer), @lock) {
	}

	public SynchronizedDictionary(IDictionary<TKey, TValue> internalDictionary, ISynchronizedObject @lock = null)
		: base(internalDictionary, @lock) {
	}
}
