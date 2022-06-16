//-----------------------------------------------------------------------
// <copyright file="IEnumerableExtensions.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;


namespace Hydrogen {


	public static class IEnumerableExtensions {

		public static IEnumerable<TResult> SelectWithExceptionHandler<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> func, Action<T, Exception> handler)
			=> enumerable
				.Select(x => {
					try {
						return (true, func(x));
					} catch (Exception error) {
						handler(x, error);
					}
					return default;
				})
				.Where(x => x.Item1)
				.Select(x => x.Item2);


		/// <summary>
		/// Enumerates as pairs  [0,1,2] becomes => [(0,1), (1,2)]
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<(T, T)> AsPairwise<T>(this IEnumerable<T> source) {
			var previous = default(T);
			using var it = source.GetEnumerator();

			if (it.MoveNext())
				previous = it.Current;

			while (it.MoveNext())
				yield return (previous, previous = it.Current);

		}

		public static T[,] ToArray2D<T>(this IEnumerable<IEnumerable<T>> enumerable) {
			var rows = new List<T[]>();
			var hlen = 0;
			var index = -1;
			foreach (var row in enumerable) {
				index++;
				var rowArr = row.ToArray();
				if (index == 0) {
					hlen = rowArr.Length;
				} else if (rowArr.Length != hlen) {
					throw new InvalidOperationException("Inconsistently sized row");
				}
				rows.Add(rowArr);
			}

			var result = new T[hlen, rows.Count];
			for (var i = 0; i < rows.Count; i++)
				result.SetRow(i, rows[i]);

			return result;
		}

		public static IEnumerable<T> OnMoveNext<T>(this IEnumerable<T> enumerable, Action action) {
			var @enum = enumerable.GetEnumerator().OnMoveNext(action);
			while (@enum.MoveNext()) {
				yield return @enum.Current;
			}
		}

		public static IEnumerable<T> OnDispose<T>(this IEnumerable<T> enumerable, Action action) {
			var @enum = enumerable.GetEnumerator().OnDispose(action);
			while (@enum.MoveNext()) {
				yield return @enum.Current;
			}
		}

		public static IEnumerable<T> WithMemory<T>(this IEnumerable<T> enumerable) {
			var @enum = enumerable.GetEnumerator().WithMemory();
			while (@enum.MoveNext()) {
				yield return @enum.Current;
			}
		}

		public static IEnumerable<IEnumerable<T>> Duplicate<T>(this IEnumerable<T> enumerable) {
			while (true)
				yield return enumerable;
		}

		public static IEnumerable<T> Loop<T>(this IEnumerable<T> enumerable) {
			return enumerable.Duplicate().SelectMany(x => x);
		}



		public static IEnumerable<T> TakeUntil<T>(
			this IEnumerable<T> elements,
			Func<T, bool> predicate
		) {
			return
				elements
				.Select((x, i) => new { Item = x, Index = i })
				.TakeUntil((x, i) => predicate(x.Item))
				.Select(x => x.Item);
		}

		public static IEnumerable<T> TakeUntil<T>(
			this IEnumerable<T> elements,
			Func<T, int, bool> predicate
		) {
			int i = 0;
			foreach (T element in elements) {
				yield return element;
				if (predicate(element, i)) {
					yield break;
				}
				i++;
			}
		}

		public static IEnumerable<IEnumerable<T>> OrderByAll<T>(this IEnumerable<IEnumerable<T>> table) {
			var tableList = table as List<IEnumerable<T>> ?? table.ToList();
			var numCols = tableList[0].Count();
			if (numCols == 0)
				return tableList;

			var result = tableList.OrderBy(r => r.ElementAt(0));
			for (var i = 1; i < numCols; i++) {
				var i1 = i;
				result = result.ThenBy(r => r.ElementAt(i1));
			}

			return result;
		}

		public static IEnumerable<T> WithoutClones<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<IEnumerable<T>, T> duplicateResolver) {
			return source.WithoutClones((x) => x, (x, resolution) => resolution, keySelector, duplicateResolver);
		}

		public static IEnumerable<T> WithoutClones<T, TMember, TKey>(this IEnumerable<T> source, Func<T, TMember> memberGetter, Func<T, TMember, T> memberCreator, Func<TMember, TKey> keySelector, Func<IEnumerable<TMember>, TMember> duplicateResolver) {
			var sourceArray = source.ToArray();
			var membersByKey = sourceArray.Select(memberGetter).ToLookup(keySelector).ToDictionary(g => g.Key, g => duplicateResolver(g));
			return sourceArray.Select(x => memberCreator(x, membersByKey[keySelector(memberGetter(x))]));
		}

		public static ObservableList<T> ToObservableList<T>(this IEnumerable<T> source) {
			return new ObservableList<T>(new List<T>(source));
		}

		/*public static ObseravableCloneableList<T> ToCloneableListWithEvents<T>(this IEnumerable<T> source) where T : ICloneable {
            return new ObseravableCloneableList<T>(source);
        }*/

		public static IEnumerable<T> InLinkedListOrder<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> linkSelector, Func<K, bool> isEndKey = null, bool includeEndKey = true) {
			K startKey;
			#region Find the start key
			// Quickly determine which nodes are linked to
			var existingNodes = new HashSet<K>();
			var linkedNodes = new HashSet<K>();
			var sourceArr = source as T[] ?? source.ToArray();
			foreach (var node in sourceArr) {
				var key = keySelector(node);
				var linkedNodeKey = linkSelector(node);
				existingNodes.Add(key);
				linkedNodes.Add(linkedNodeKey);
			}

			var unlinkedNodes = existingNodes.Except(linkedNodes).ToArray();
			var unlinkedNodesCount = unlinkedNodes.Count();
			if (unlinkedNodesCount == 0 && existingNodes.Count > 0) {
				// circular linked list, so pick any item to start with
				startKey = keySelector(sourceArr.First());
			} else if (unlinkedNodesCount == 1) {
				// one item is unlinked, so it's the start item
				startKey = unlinkedNodes.Single();
			} else {
				// many items are unlinked, can't pick start item
				throw new Exception(string.Format("Unable to pick a start node as there were {0} possible candidates. A start node is determined by a node that is not linked to by other nodes", unlinkedNodesCount));
			}
			#endregion

			return InLinkedListOrder(sourceArr, keySelector, linkSelector, startKey, isEndKey, includeEndKey);
		}

		public static IEnumerable<T> InLinkedListOrder<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, K> linkSelector, K startKey, Func<K, bool> isEndKey = null, bool includeEndKey = true) {
			var sourceArr = source as T[] ?? source.ToArray();
			if (!sourceArr.Any())
				yield break;

			if (isEndKey == null)
				isEndKey = x => false;

			var nodeLookup = sourceArr.ToLookup(keySelector);
			var visited = new HashSet<K>();
			var currentKey = startKey;
			do {
				if (!includeEndKey && isEndKey(currentKey))
					break;
				var linkedNodes = nodeLookup[currentKey];
				var linkedNodesCount = linkedNodes.Count();
				if (linkedNodesCount == 0)
					yield break; // last node
				if (linkedNodesCount > 1)
					throw new Exception(string.Format("Expected 1 node with key {0}, found {1}", currentKey, linkedNodes.Count()));


				var current = linkedNodes.Single();
				visited.Add(currentKey);
				yield return current;
				if (includeEndKey && isEndKey(currentKey))
					break;
				currentKey = linkSelector(current);

			} while (!visited.Contains(currentKey));
		}


		public static IEnumerable<Tuple<TFirst, TSecond>> Zip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second) {
			return first.Zip(second, Tuple.Create);
		}

		//public static IEnumerable<TResult> ZipWith<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) {
		//	if (first == null) throw new ArgumentNullException("first");
		//	if (second == null) throw new ArgumentNullException("second");
		//	if (resultSelector == null) throw new ArgumentNullException("resultSelector");
		//	return ZipIterator(first, second, resultSelector);
		//}

		//private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>
		//	(IEnumerable<TFirst> first,
		//	IEnumerable<TSecond> second,
		//	Func<TFirst, TSecond, TResult> resultSelector) {
		//	using (var e1 = first.GetEnumerator())
		//	using (var e2 = second.GetEnumerator())
		//		while (e1.MoveNext() && e2.MoveNext())
		//			yield return resultSelector(e1.Current, e2.Current);
		//}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T element) {
			return source.Except(new[] { element });
		}

		public static IEnumerable<T> Union<T>(this IEnumerable<T> source, T element) {
			return source.Union(new[] { element });
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T element) {
			return source.Concat(new[] { element });
		}

		public static ISet<T> ToSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null) {
			comparer ??= EqualityComparer<T>.Default;
			var hashSet = new HashSet<T>(comparer);
			foreach (var item in source)
				hashSet.Add(item);
			return hashSet;
		}

		public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<TKey> keyComparer = null)
			=> new Dictionary<TKey, TValue>(items, keyComparer);

		public static IDictionary<K, List<T>> ToMultiValueDictionary<K, T>(this IEnumerable<T> source, Func<T, K> keySelector, IEqualityComparer<K> keyComparer = null) {
			var result = new Dictionary<K, List<T>>(keyComparer);

			foreach (var item in source) {
				var itemKey = keySelector(item);
				if (!result.ContainsKey(itemKey)) {
					result[itemKey] = new List<T>();
				}
				result[itemKey].Add(item);
			}
			return result;
		}

		public static IDictionary<K, List<V>> ToMultiValueDictionary<K, V, T>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, V> valueSelector, IEqualityComparer<K> keyComparer = null) {
			var result = new Dictionary<K, List<V>>(keyComparer);
			foreach (var item in source) {
				var itemKey = keySelector(item);
				if (!result.ContainsKey(itemKey)) {
					result[itemKey] = new List<V>();
				}
				result[itemKey].Add(valueSelector(item));
			}
			return result;
		}

		public static MultiKeyDictionary<K1, K2, V> ToMultiKeyDictionary<S, K1, K2, V>(this IEnumerable<S> items, Func<S, K1> key1, Func<S, K2> key2, Func<S, V> value, IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null) {
			var dict = new MultiKeyDictionary<K1, K2, V>(key1Comparer, key2Comparer);
			foreach (S i in items) {
				dict.Add(key1(i), key2(i), value(i));
			}
			return dict;
		}

		public static MultiKeyDictionary<K1, K2, K3, V> ToMultiKeyDictionary<S, K1, K2, K3, V>(this IEnumerable<S> items, Func<S, K1> key1, Func<S, K2> key2, Func<S, K3> key3, Func<S, V> value, IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null) {
			var dict = new MultiKeyDictionary<K1, K2, K3, V>(key1Comparer, key2Comparer, key3Comparer);
			foreach (S i in items) {
				dict.Add(key1(i), key2(i), key3(i), value(i));
			}
			return dict;
		}

		public static MultiKeyDictionary<K1, K2, K3, K4, V> ToMultiKeyDictionary<S, K1, K2, K3, K4, V>(this IEnumerable<S> items, Func<S, K1> key1, Func<S, K2> key2, Func<S, K3> key3, Func<S, K4> key4, Func<S, V> value, IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null) {
			var dict = new MultiKeyDictionary<K1, K2, K3, K4, V>(key1Comparer, key2Comparer, key3Comparer, key4Comparer);
			foreach (S i in items) {
				dict.Add(key1(i), key2(i), key3(i), key4(i), value(i));
			}
			return dict;
		}

		public static MultiKeyDictionary<K1, K2, K3, K4, K5, V> ToMultiKeyDictionary<S, K1, K2, K3, K4, K5, V>(this IEnumerable<S> items, Func<S, K1> key1, Func<S, K2> key2, Func<S, K3> key3, Func<S, K4> key4, Func<S, K5> key5, Func<S, V> value, IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null, IEqualityComparer<K5> key5Comparer = null) {
			var dict = new MultiKeyDictionary<K1, K2, K3, K4, K5, V>(key1Comparer, key2Comparer, key3Comparer, key4Comparer, key5Comparer);
			foreach (S i in items) {
				dict.Add(key1(i), key2(i), key3(i), key4(i), key5(i), value(i));
			}
			return dict;
		}

		public static MultiKeyDictionary<K1, K2, K3, K4, K5, K6, V> ToMultiKeyDictionary<S, K1, K2, K3, K4, K5, K6, V>(this IEnumerable<S> items, Func<S, K1> key1, Func<S, K2> key2, Func<S, K3> key3, Func<S, K4> key4, Func<S, K5> key5, Func<S, K6> key6, Func<S, V> value, IEqualityComparer<K1> key1Comparer = null, IEqualityComparer<K2> key2Comparer = null, IEqualityComparer<K3> key3Comparer = null, IEqualityComparer<K4> key4Comparer = null, IEqualityComparer<K5> key5Comparer = null, IEqualityComparer<K6> key6Comparer = null) {
			var dict = new MultiKeyDictionary<K1, K2, K3, K4, K5, K6, V>(key1Comparer, key2Comparer, key3Comparer, key4Comparer, key5Comparer);
			foreach (S i in items) {
				dict.Add(key1(i), key2(i), key3(i), key4(i), key5(i), key6(i), value(i));
			}
			return dict;
		}

		public static EnumerableKeyDictionary<K, T> ToMultiKeyDictionary2<K, T>(this IEnumerable<T> source, params Func<T, K>[] keySelectors) {
			return ToMultiKeyDictionary2(source, (IEnumerable<Func<T, K>>)keySelectors);
		}

		public static EnumerableKeyDictionary<K, T> ToMultiKeyDictionary2<K, T>(this IEnumerable<T> source, IEnumerable<Func<T, K>> keySelectors) {
			return ToMultiKeyDictionary2Ex(source, keySelectors, (x) => x);
		}

		public static EnumerableKeyDictionary<K, V> ToMultiKeyDictionary2Ex<T, K, V>(this IEnumerable<T> source, IEnumerable<Func<T, K>> keySelectors, Func<T, V> valueSelector) {
			var result = new EnumerableKeyDictionary<K, V>();
			var keySelectorArr = keySelectors as Func<T, K>[] ?? keySelectors.ToArray();
			foreach (var item in source) {
				var key = keySelectorArr.Select(keySelector => keySelector(item));
				result.Add(key, valueSelector(item));
			}
			return result;
		}

		public static MultiKeyLookup<K, T> ToMultiKeyLookup<T, K>(this IEnumerable<T> source, Func<T, IEnumerable<K>> keySelector, IEqualityComparer<K> comparer = null) {
			return source.ToMultiKeyLookup(keySelector, item => item, comparer);
		}

		public static MultiKeyLookup<K, V> ToMultiKeyLookup<T, K, V>(this IEnumerable<T> source, Func<T, IEnumerable<K>> keySelector, Func<T, V> valueSelector, IEqualityComparer<K> comparer = null) {
			Guard.ArgumentNotNull(source, nameof(source));
			Guard.ArgumentNotNull(keySelector, nameof(keySelector));
			Guard.ArgumentNotNull(valueSelector, nameof(valueSelector));

			var lookup = new MultiKeyLookup<K, V>(comparer);

			foreach (var item in source) {
				var key = keySelector(item);
				var element = valueSelector(item);
				lookup.Add(key, element);
			}
			return lookup;
		}

		public static IExtendedList<T> ToExtendedList<T>(this IEnumerable<T> items) {
			if (items is IExtendedList<T> extList)
				return extList;
			extList = new ExtendedList<T>(items.Count());
			if (items is List<T> list) {
				extList.AddRange(list.GetRange(0, items.Count()));
			} else if (items is IList<T> ilist) {
				extList.AddRange(ilist);
			} else {
				extList.AddRange(items);
			}
			return extList;
		}

		// NOTE: ForEach applies action to all items then return enumerable, Apply applies action during enumeration 
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (T item in source)
				action(item);
		}

		public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> asyncAction) {
			foreach (var value in source) {
				await asyncAction(value);
			}
		}

		public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> asyncAction, CancellationToken cancellationToken) {
			foreach (var value in source) {
				if (cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException(cancellationToken);
				await asyncAction(value).WithCancellationToken(cancellationToken);
			}
		}

		public static int Update<T>(this IEnumerable<T> source, Action<T> action) {
			int updated = 0;
			foreach (T item in source) {
				action(item);
				updated++;
			}
			return updated;
		}

		// NOTE: ForEach applies action to all items then return enumerable, Apply applies action during enumeration
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Apply<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (var item in source) {
				action(item);
				yield return item;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> ApplyWithExceptionHandler<T>(this IEnumerable<T> source, Action<T> action, Action<T, Exception> handler) {
			foreach (var item in source) {
				try {
					action(item);
				} catch (Exception ex) {
					handler(item, ex);
				}
				yield return item;
			}
		}

		public static IEnumerable<IEnumerable<TSource>> Split<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			var group = new List<TSource>();
			foreach (var item in source) {
				if (predicate(item)) {
					yield return group.AsEnumerable();
					group = new List<TSource>();
				} else {
					group.Add(item);
				}
			}
			yield return group.AsEnumerable();
		}

		public static IEnumerable<T> Into<T>(this IEnumerable<T> source, ref T value1, ref T value2) {
			var len = source.Count();
			var enumerator = source.GetEnumerator();
			if (len > 0) {
				value1 = enumerator.Current;
				enumerator.MoveNext();
			}

			if (len > 1) {
				value2 = enumerator.Current;
				enumerator.MoveNext();
			}
			return source;
		}

		public static bool ContainSameElements<T>(this IEnumerable<T> source, IEnumerable<T> dest, IEqualityComparer<T> comparer = null) {
			var c = comparer ?? EqualityComparer<T>.Default;
			return source.Count() == dest.Count() && !source.Except(dest, c).Any();
		}

		public static bool ContainsAll<T>(this IEnumerable<T> source, params T[] matches) {
			return source.ContainsAll(matches.AsEnumerable());
		}

		public static bool ContainsAll<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer, params T[] matches) {
			return source.ContainsAll(matches.AsEnumerable(), comparer);
		}

		public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> matches, IEqualityComparer<T> comparer = null) {
			var c = comparer ?? EqualityComparer<T>.Default;
			return matches.All(x => source.Contains(x, c));
		}


		public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] matches) {
			return source.ContainsAny(matches.AsEnumerable());
		}

		public static bool ContainsAny<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer, params T[] matches) {
			return source.ContainsAny(matches.AsEnumerable(), comparer);
		}


		public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> matches, IEqualityComparer<T> comparer = null) {
			var c = comparer ?? EqualityComparer<T>.Default;
			return matches.Any(x => source.Contains(x, c));
		}

		/// <summary>
		/// Performs a binary search on the specified collection.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <typeparam name="TSearch">The type of the searched item.</typeparam>
		/// <param name="list">The list to be searched.</param>
		/// <param name="value">The value to search for.</param>
		/// <param name="comparer">The comparer that is used to compare the value with the list items.</param>
		/// <returns></returns>
		public static int BinarySearch<TItem, TSearch>(this IEnumerable<TItem> list, TSearch value, Func<TSearch, TItem, int> comparer) {
			Guard.ArgumentNotNull(list, nameof(list));
			var listArr = list as TItem[] ?? list.ToArray();
			return Tools.Collection.BinarySearch(listArr, value, 0, listArr.Length - 1, comparer);
		}

		/// <summary>
		/// Performs a binary search on the specified collection.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="list">The list to be searched.</param>
		/// <param name="value">The value to search for.</param>
		/// <returns></returns>
		public static int BinarySearch<TItem>(this IEnumerable<TItem> list, TItem value) {
			return BinarySearch(list, value, Comparer<TItem>.Default);
		}

		/// <summary>
		/// Performs a binary search on the specified collection.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="list">The list to be searched.</param>
		/// <param name="value">The value to search for.</param>
		/// <param name="comparer">The comparer that is used to compare the value with the list items.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<TItem>(this IEnumerable<TItem> list, TItem value, IComparer<TItem> comparer) {
			return list.BinarySearch(value, comparer.Compare);
		}

		public static IEnumerable<TItem> Randomize<TItem>(this IEnumerable<TItem> source)
			=> Randomize(source, Tools.Maths.RNG);

		public static IEnumerable<TItem> Randomize<TItem>(this IEnumerable<TItem> source, Random RNG)
			=> source.OrderBy(x => RNG.Next());

		public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> sequence, int partitionSize) {
			// HS 2019-02-26 removed iOS specific implementation due to .NET Standard port
			if (partitionSize < 1)
				throw new ArgumentOutOfRangeException(nameof(partitionSize), partitionSize, "must be equal to or greater than 1");
			return
				sequence
				.Select((x, i) => new { Value = x, BatchNo = i / partitionSize })
				.GroupAdjacentBy(x => x.BatchNo)
				.Select(g => g.Select(x => x.Value));
		}

		public static IEnumerable<IEnumerable<T>> PartitionBySize<T>(this IEnumerable<T> sequence, Func<T, int> sizeFunc, int partitionSize) {
			// HS 2019-02-26 removed iOS specific implementation due to .NET Standard port
			if (partitionSize < 1)
				throw new ArgumentOutOfRangeException(nameof(partitionSize), partitionSize, "must be equal to or greater than 1");
			long currentSum = 0;
			return
				sequence
				.Select((x, i) => new { Value = x, BatchNo = (currentSum += sizeFunc(x)) / partitionSize })
				.GroupAdjacentBy(x => x.BatchNo)
				.Select(g => g.Select(x => x.Value));
		}

		public static IEnumerable<T> Unpartition<T>(this IEnumerable<IEnumerable<T>> sequence) {
			return sequence.SelectMany(partition => partition);
		}

		public static IEnumerable<T> Unpartition<T>(this IEnumerable<T[]> sequence) {
			return sequence.Cast<IEnumerable<T>>().Unpartition();
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupAdjacentBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
			var last = default(TKey);
			var haveLast = false;
			var list = new List<TSource>();
			foreach (var s in source) {
				var k = keySelector(s);
				if (haveLast) {
					if (!k.Equals(last)) {
						yield return new GroupOfAdjacent<TSource, TKey>(list, last);
						list = new List<TSource> { s };
						last = k;
					} else {
						list.Add(s);
						last = k;
					}
				} else {
					list.Add(s);
					last = k;
					haveLast = true;
				}
			}
			if (haveLast)
				yield return new GroupOfAdjacent<TSource, TKey>(list, last);
		}

		public static IEnumerable<T> ForEachThenOnLast<T>(this IEnumerable<T> source, Action<T> eachAction, Action<T> lastAction) {
			source.Reverse().Skip(1).Reverse().ForEach(eachAction);
			var lastItem = source.LastOrDefault();
			if (lastItem != null) {
				lastAction(lastItem);
			}
			return source;
		}

		public static string ToDelimittedString<T>(this IEnumerable<T> source, string delimitter, string nullText = "", Func<T, string> toStringFunc = null) {
			if (toStringFunc == null)
				toStringFunc = (x) => x.ToString();

			StringBuilder stringBuilder = new StringBuilder();
			foreach (var t in source) {
				if (stringBuilder.Length > 0) {
					stringBuilder.Append(delimitter);
				}
				stringBuilder.Append(t != null ? toStringFunc(t) : nullText);
			}
			return stringBuilder.ToString();
		}

		public static IEnumerable<Tuple<T, int>> WithIndex<T>(this IEnumerable<T> items) {
			return items.Select((item, index) => Tuple.Create(item, index));
		}

		public static IEnumerable<EnumeratedItem<T>> WithDescriptions<T>(this IEnumerable<T> items) {
			// To avoid evaluating the whole collection up-front (which may be undesirable, for example
			// if the collection contains infinitely many members), read-ahead just one item at a time.

			// Get the first item
			var enumerator = items.GetEnumerator();
			if (!enumerator.MoveNext())
				yield break;
			T currentItem = enumerator.Current;
			int index = 0;

			while (true) {
				// Read ahead so we know whether we're at the end or not
				bool isLast = !enumerator.MoveNext();

				// Describe and yield the current item
				EnumeratedItemDescription description = (index % 2 == 0 ? EnumeratedItemDescription.Odd : EnumeratedItemDescription.Even);
				if (index == 0) description |= EnumeratedItemDescription.First;
				if (isLast) description |= EnumeratedItemDescription.Last;
				if (index > 0 && !isLast) description |= EnumeratedItemDescription.Interior;
				yield return new EnumeratedItem<T>(index, currentItem, description);

				// Terminate or continue
				if (isLast)
					yield break;
				index++;
				currentItem = enumerator.Current;
			}
		}

		// This provides a useful extension-like method to find the index of and item from IEnumerable<T>
		// This was based off of the Enumerable.Count<T> extension method.
		/// <summary>
		/// Returns the index of an item in a sequence.
		/// </summary>
		/// <typeparam name="T">The type of the elements of source.</typeparam>
		/// <param name="source">A sequence containing elements.</param>
		/// <param name="item">The item to locate.</param>        
		/// <returns>The index of the entry if it was found in the sequence; otherwise, -1.</returns>
		public static int EnumeratedIndexOf<TSource>(this IEnumerable<TSource> source, TSource item) {
			return EnumeratedIndexOf(source, item, EqualityComparer<TSource>.Default);
		}


		// This provides a useful extension-like method to find the index of and item from IEnumerable<T>
		// This was based off of the Enumerable.Count<T> extension method.
		/// <summary>
		/// Returns the index of an item in a sequence.
		/// </summary>
		/// <typeparam name="T">The type of the elements of source.</typeparam>
		/// <param name="source">A sequence containing elements.</param>
		/// <param name="item">The item to locate.</param>
		/// <param name="itemComparer">The item equality comparer to use.  Pass null to use the default comparer.</param>
		/// <returns>The index of the entry if it was found in the sequence; otherwise, -1.</returns>
		public static int EnumeratedIndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> itemComparer) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}

			int i = 0;
			foreach (T possibleItem in source) {
				if (itemComparer.Equals(item, possibleItem)) {
					return i;
				}
				i++;
			}
			return -1;
		}

		public static int EnumeratedIndexOf<TSource>(this IEnumerable<TSource> source, TSource item, Func<TSource, TSource, bool> itemComparer) {
			return source.EnumeratedIndexOf(item, new ActionEqualityComparer<TSource>(itemComparer));
		}

		public static int EnumeratedIndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}

			int i = 0;
			foreach (TSource possibleItem in source) {
				if (predicate(possibleItem)) {
					return i;
				}
				i++;
			}
			return -1;
		}

		public static T RandomElement<T>(this IEnumerable<T> sequence) {
			if (!sequence.Any())
				return default(T);

			return sequence.ElementAt(Tools.Maths.RNG.Next(0, sequence.Count()));
		}

		public static T MaxBy<T, R>(this IEnumerable<T> source, Func<T, R> selector) where R : IComparable<R> {
			return
				source
				.Select(t => new Tuple<T, R>(t, selector(t)))
				.Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;
		}

		public static T MinBy<T, R>(this IEnumerable<T> source, Func<T, R> selector) where R : IComparable<R> {
			return
				source
				.Select(t => new Tuple<T, R>(t, selector(t)))
				.Aggregate((max, next) => next.Item2.CompareTo(max.Item2) < 0 ? next : max).Item1;
		}

		#region AsHierarchy Extensions

		// Stefan Cruysberghs, July 2008, http://www.scip.be
		// <summary>
		// AsHierarchy extension methods for LINQ to Objects IEnumerable
		// </summary>



		/// <summary>
		/// LINQ to Objects (IEnumerable) AsHierachy() extension method
		/// </summary>
		/// <typeparam name="TEntity">Entity class</typeparam>
		/// <typeparam name="TProperty">Property of entity class</typeparam>
		/// <param name="allItems">Flat collection of entities</param>
		/// <param name="idProperty">Func delegete to Id/Key of entity</param>
		/// <param name="parentIdProperty">Func delegete to parent Id/Key</param>
		/// <returns>Hierarchical structure of entities</returns>
		public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
		  this IEnumerable<TEntity> allItems,
		  Func<TEntity, TProperty> idProperty,
		  Func<TEntity, TProperty> parentIdProperty) where TEntity : class {
			return CreateHierarchy(allItems, default(TEntity), idProperty, parentIdProperty, null, 0, 0);
		}

		/// <summary>
		/// LINQ to Objects (IEnumerable) AsHierachy() extension method
		/// </summary>
		/// <typeparam name="TEntity">Entity class</typeparam>
		/// <typeparam name="TProperty">Property of entity class</typeparam>
		/// <param name="allItems">Flat collection of entities</param>
		/// <param name="idProperty">Func delegete to Id/Key of entity</param>
		/// <param name="parentIdProperty">Func delegete to parent Id/Key</param>
		/// <param name="rootItemId">Value of root item Id/Key</param>
		/// <returns>Hierarchical structure of entities</returns>
		public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
		  this IEnumerable<TEntity> allItems,
		  Func<TEntity, TProperty> idProperty,
		  Func<TEntity, TProperty> parentIdProperty,
		  object rootItemId) where TEntity : class {
			return CreateHierarchy(allItems, default(TEntity), idProperty, parentIdProperty, rootItemId, 0, 0);
		}

		/// <summary>
		/// LINQ to Objects (IEnumerable) AsHierachy() extension method
		/// </summary>
		/// <typeparam name="TEntity">Entity class</typeparam>
		/// <typeparam name="TProperty">Property of entity class</typeparam>
		/// <param name="allItems">Flat collection of entities</param>
		/// <param name="idProperty">Func delegete to Id/Key of entity</param>
		/// <param name="parentIdProperty">Func delegete to parent Id/Key</param>
		/// <param name="rootItemId">Value of root item Id/Key</param>
		/// <param name="maxDepth">Maximum depth of tree</param>
		/// <returns>Hierarchical structure of entities</returns>
		public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
		  this IEnumerable<TEntity> allItems,
		  Func<TEntity, TProperty> idProperty,
		  Func<TEntity, TProperty> parentIdProperty,
		  object rootItemId,
		  int maxDepth) where TEntity : class {
			return CreateHierarchy(allItems, default(TEntity), idProperty, parentIdProperty, rootItemId, maxDepth, 0);
		}


		private static IEnumerable<HierarchyNode<TEntity>> CreateHierarchy<TEntity, TProperty>(
			IEnumerable<TEntity> allItems,
			TEntity parentItem,
			Func<TEntity, TProperty> idProperty,
			Func<TEntity, TProperty> parentIdProperty,
			object rootItemId,
			int maxDepth,
			int depth) where TEntity : class {
			IEnumerable<TEntity> childs;

			if (rootItemId != null) {
				childs = allItems.Where(i => idProperty(i).Equals(rootItemId));
			} else {
				if (parentItem == null) {
					childs = allItems.Where(i => parentIdProperty(i).Equals(default(TProperty)));
				} else {
					childs = allItems.Where(i => parentIdProperty(i).Equals(idProperty(parentItem)));
				}
			}

			if (childs.Count() > 0) {
				depth++;

				if ((depth <= maxDepth) || (maxDepth == 0)) {
					foreach (var item in childs)
						yield return
						  new HierarchyNode<TEntity>() {
							  Entity = item,
							  ChildNodes =
								CreateHierarchy(allItems.AsEnumerable(), item, idProperty, parentIdProperty, null, maxDepth, depth),
							  Depth = depth,
							  Parent = parentItem
						  };
				}
			}
		}
		#endregion
	}
}
