// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class ActionDictionaryDecorator<TKey, TValue, TConcrete> : DictionaryDecorator<TKey, TValue> where TConcrete : IDictionary<TKey, TValue> {

	private readonly Action _enterAction;
	private readonly Action _exitAction;

	public ActionDictionaryDecorator(TConcrete internalDictionary, Action enterAction, Action exitAction)
		: base(internalDictionary) {
		_enterAction = enterAction;
		_exitAction = exitAction;
	}

	public override ICollection<TKey> Keys {
		get {
			using (EnterScope())
				return base.Keys;
		}
	}

	public override ICollection<TValue> Values {
		get {
			using (EnterScope())
				return base.Values;
		}
	}

	public override void Add(TKey key, TValue value) {
		using (EnterScope())
			base.Add(key, value);
	}

	public override bool ContainsKey(TKey key) {
		using (EnterScope())
			return base.ContainsKey(key);
	}


	public override bool TryGetValue(TKey key, out TValue value) {
		using (EnterScope())
			return base.TryGetValue(key, out value);
	}

	public override TValue this[TKey key] {
		get {
			using (EnterScope())
				return base[key];
		}
		set {
			using (EnterScope())
				base[key] = value;
		}
	}

	public override void Add(KeyValuePair<TKey, TValue> item) {
		using (EnterScope())
			base.Add(item);
	}

	public override bool Contains(KeyValuePair<TKey, TValue> item) {
		using (EnterScope())
			return base.Contains(item);
	}

	public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
		using (EnterScope())
			base.CopyTo(array, arrayIndex);
	}

	public override bool Remove(KeyValuePair<TKey, TValue> item) {
		using (EnterScope())
			return base.Remove(item);
	}

	public override bool Remove(TKey item) {
		using (EnterScope())
			return base.Remove(item);
	}

	public override void Clear() {
		using (EnterScope())
			base.Clear();
	}

	public override int Count {
		get {
			using (EnterScope())
				return base.Count;
		}
	}

	public override bool IsReadOnly {
		get {
			using (EnterScope())
				return base.IsReadOnly;
		}
	}

	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		var scope = EnterScope();
		return base.GetEnumerator().OnDispose(scope.Dispose);
	}

	private IDisposable EnterScope() {
		_enterAction?.Invoke();
		return new ActionScope(_exitAction);
	}

}
