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

/// <summary>
/// A chain-of-responsibility pattern implementation of <see cref="IDictionary{TKey,TValue}"/>.
/// </summary>
public class DictionaryChain<TKey, TValue> : DictionaryDecorator<TKey, TValue> {

	public DictionaryChain(IDictionary<TKey, TValue> head)
		: this([head]) {
	}

	public DictionaryChain(IEnumerable<IDictionary<TKey, TValue>> chain)
		: this(chain as IDictionary<TKey, TValue>[] ?? chain.ToArray()) {
	}

	public DictionaryChain(params IDictionary<TKey, TValue>[] chain)
		: this(chain.Length > 0 ? Tools.Array.PopHead(ref chain) : throw new ArgumentException("No dictionaries supplied", nameof(chain)), chain) {
	}
	private DictionaryChain(IDictionary<TKey, TValue> link, IDictionary<TKey, TValue>[] chain)
		: this(link, chain.Length > 0 ? new DictionaryChain<TKey, TValue>(chain) : null) {
	}

	public DictionaryChain(IDictionary<TKey, TValue> head, DictionaryChain<TKey, TValue> tail)
		: base(head) {
		Next = tail;
		if (Next != null)
			Next.Previous = this;
	}

	public override int Count => base.Count + (Next?.Count ?? 0);

	public override bool IsReadOnly => base.IsReadOnly || (Next?.IsReadOnly ?? false);

	public DictionaryChain<TKey, TValue> Next { get; private set; } = null;

	public DictionaryChain<TKey, TValue> Previous { get; private set; } = null;

	public IDictionary<TKey,TValue> UnderlyingDictionary => InternalDictionary;

	public override TValue this[TKey key] {
		get {
			if (TryGetValue(key, out var value))
				return value;
			throw new KeyNotFoundException($"The key '{key}' was not found");
		}
		set => Add(key, value);
	}

	public override ICollection<TKey> Keys =>  Next is null ? base.Keys : new ConcatenatedCollection<TKey>(base.Keys, Next.Keys);

	public override ICollection<TValue> Values => Next is null ? base.Values : new ConcatenatedCollection<TValue>(base.Values, Next.Values);

	public DictionaryChain<TKey, TValue> AttachHead(IDictionary<TKey, TValue> link)
		=> AttachHead(new DictionaryChain<TKey, TValue>(link));

	public DictionaryChain<TKey, TValue> AttachHead(DictionaryChain<TKey, TValue> head) {
		this.Previous = head;
		head.Next = this;
		return head;
	}

	public DictionaryChain<TKey, TValue> DetachHead() {
		Guard.Against(Next == null, "Terminal link encountered (last link)");
		var nextHead = Next;
		Next = null;
		nextHead.Previous = null;
		return nextHead;
	}

	public override bool ContainsKey(TKey key) => base.ContainsKey(key) || (Next?.ContainsKey(key) ?? false);

	public override bool Contains(KeyValuePair<TKey, TValue> item) => base.Contains(item) || (Next?.Contains(item) ?? false);

	public override bool TryGetValue(TKey key, out TValue value) => base.TryGetValue(key, out value) || (Next?.TryGetValue(key, out value) ?? false);

	public override void Add(TKey key, TValue value) => AddInternal(key, value);

	public override void Add(KeyValuePair<TKey, TValue> item) => AddInternal(item.Key, item.Value);

	public override bool Remove(TKey item) => base.Remove(item) || (Next?.Remove(item) ?? false);

	public override bool Remove(KeyValuePair<TKey, TValue> item) => base.Remove(item) || (Next?.Remove(item) ?? false);

	public override void Clear() {
		base.Clear();
		Next?.Clear();
	}

	public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
		base.CopyTo(array, arrayIndex);
		Next?.CopyTo(array, arrayIndex + base.Count);
	}

	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
		base
			.GetEnumerator()
			.AsEnumerable()
			.Concat(
				Next?
					.GetEnumerator()
					.AsEnumerable() ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>()
			).GetEnumerator();

	private bool TryLocate(TKey key, out IDictionary<TKey, TValue> link, out TValue value) {
		link = this;
		return base.TryGetValue(key, out value) || (Next?.TryLocate(key, out link, out value) ?? false);

	}

	private void AddInternal(TKey key, TValue value) {
		if (TryLocate(key, out var link, out _))
			link[key] = value;
		else
			base.Add(key, value);
	}
}
