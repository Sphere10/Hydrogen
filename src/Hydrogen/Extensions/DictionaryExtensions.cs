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
using System.Diagnostics;
using Hydrogen.Collections;

namespace Hydrogen;

public static class IDictionaryExtensions {

	public static bool TrySearchKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, out TKey key, IEqualityComparer<TValue> comparer = null) {
		comparer ??= EqualityComparer<TValue>.Default;
		foreach (var kvp in dictionary) {
			if (comparer.Equals(kvp.Value, value)) {
				key = kvp.Key;
				return true;
			}
		}
		key = default;
		return false;
	}

	public static IReadOnlyDictionary<K, V> AsReadOnly<K, V>(this IDictionary<K, V> dictionary) {
		return new ReadOnlyDictionaryAdapter<K, V>(dictionary);
	}

	public static SynchronizedDictionary<K, V> AsSynchronized<K, V>(this IDictionary<K, V> dictionary) {
		return new(dictionary);
	}


	public static void AddRange<A, B>(this IDictionary<A, B> dictionary, A[] keys, B[] values) {
		Debug.Assert(keys != null);
		Debug.Assert(values != null);
		Debug.Assert(keys.Length == values.Length);
		for (int i = 0; i < keys.Length; i++) {
			dictionary.Add(keys[i], values[i]);
		}
	}

	public static void AddRange<A, B>(this IDictionary<A, B> dictionary, IEnumerable<KeyValuePair<A, B>> values, CollectionConflictPolicy conflictPolicy = CollectionConflictPolicy.Skip) {
		foreach (var value in values) {
			if (!dictionary.ContainsKey(value.Key)) {
				dictionary.Add(value);
			} else {
				switch (conflictPolicy) {
					case CollectionConflictPolicy.Override:
						dictionary[value.Key] = value.Value;
						break;
					case CollectionConflictPolicy.Skip:
						break;
					case CollectionConflictPolicy.Throw:
					default:
						throw new Exception(string.Format("Dictionary already contains a value with key '{0}'", value.Key));
				}
			}
		}
	}

	public static ILookup<TValue, TKey> Inverse<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
		return dictionary.ToLookup(kvp => kvp.Value, kvp => kvp.Key);
	}


	public static ILookup<TValue, TKey> InverseUsingValueReferenceAsKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TValue : class {
		return new ValueReferenceInverseDictionary<TKey, TValue>(dictionary);
	}


	public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
		this IDictionary<TKey, TValue> dict,
		Func<IGrouping<TKey, TValue>, TValue> resolveDuplicates = null,
		params IDictionary<TKey, TValue>[] dicts) {
		return dicts.Union(dict).Merge(resolveDuplicates);
	}

	public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<IDictionary<TKey, TValue>> dicts,
	                                                            Func<IGrouping<TKey, TValue>, TValue> resolveDuplicates = null) {
		if (resolveDuplicates == null)
			resolveDuplicates = group => @group.First();

		return dicts.SelectMany(dict => dict)
			.ToLookup(pair => pair.Key, pair => pair.Value)
			.ToDictionary(group => group.Key, group => resolveDuplicates(group));
	}
}
