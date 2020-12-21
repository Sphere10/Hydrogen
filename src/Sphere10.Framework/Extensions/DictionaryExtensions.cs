//-----------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Sphere10.Framework.Collections;

namespace Sphere10.Framework {
	public static class DictionaryExtensions {

	    public static IDictionary<K,V> AsReadOnly<K, V>(this IDictionary<K, V> dictionary) {
	        return new ReadOnlyDictionaryDecorator<K, V>(dictionary);
	    }

	    public static SynchronizedDictionary<K, V> AsSynchronized<K,V>(this IDictionary<K, V> dictionary) {
	        return new SynchronizedDictionary<K, V>(dictionary);
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
	                        break;
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
            params IDictionary<TKey, TValue>[] dicts ) {
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
}
