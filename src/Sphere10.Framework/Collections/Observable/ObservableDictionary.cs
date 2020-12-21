//-----------------------------------------------------------------------
// <copyright file="ObservableDictionary.cs" company="Sphere 10 Software">
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Sphere10.Framework {

    public class ObservableDictionary<TKey, TValue> : ObservableCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue> {
		protected new readonly IDictionary<TKey, TValue> InnerCollection;
		public event EventHandlerEx<object, SearchingMembershipEventArgs<TKey>> SearchingKeyMembership;
		public event EventHandlerEx<object, SearchedMembershipEventArgs<TKey>> SearchedKeyMembership;
		public event EventHandlerEx<object, FetchingByItemsEventArgs<TKey>> Fetching;
		public event EventHandlerEx<object, FetchedByItemsEventArgs<TKey, Tuple<bool, TValue>>> Fetched;
		public event EventHandlerEx<object, EnumeratingEventArgs> FetchingKeysCollection;
		public event EventHandlerEx<object, EnumeratedEventArgs<TKey>> FetchedKeysCollection;
		public event EventHandlerEx<object, EnumeratingEventArgs> FetchingValuesCollection;
		public event EventHandlerEx<object, EnumeratedEventArgs<TValue>> FetchedValuesCollection;
		public event EventHandlerEx<object, UpdatingByItemsEventArgs<KeyValuePair<TKey, TValue>>> Updating;
		public event EventHandlerEx<object, UpdatedByItemsEventArgs<KeyValuePair<TKey, TValue>>> Updated;
		public event EventHandlerEx<object, RemovingItemsEventArgs<TKey>> RemovingKeys;
		public event EventHandlerEx<object, RemovedItemsEventArgs<TKey>> RemovedKeys;

		public ObservableDictionary()
			: this(new Dictionary<TKey, TValue>()) {
		}

		public ObservableDictionary(IDictionary<TKey, TValue> internalDictionary)
			: base(internalDictionary) {
			InnerCollection = (IDictionary<TKey, TValue>)base.InnerCollection;
		}


		public void Add(TKey key, TValue value) =>
			DoOperation(
				EventTraits.Add,
				() => {
					InnerCollection.Add(key, value);
					return 0;
				},
				() => new AddingEventArgs<KeyValuePair<TKey, TValue>> { CallArgs = new ItemsCallArgs<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey,TValue>(key, value)) },
				_ => new AddedEventArgs<KeyValuePair<TKey, TValue>>(),
				(preEventArgs) => {
					OnAdding(preEventArgs);
					NotifyAdding(preEventArgs);
				},
				(postEventArgs) => {
					OnAdded(postEventArgs);
					NotifyAdded(postEventArgs);
				}
			);


		public bool ContainsKey(TKey key) =>
			DoOperation(
				EventTraits.Search,
				() => InnerCollection.ContainsKey(key),
				() => new SearchingMembershipEventArgs<TKey> { CallArgs = new ItemsCallArgs<TKey>(key) },
				result => new SearchedMembershipEventArgs<TKey> { Result = new[] { result } },
				(preEventArgs) => {
					OnSearchingKeyMembership(preEventArgs);
					SearchingKeyMembership?.Invoke(this, preEventArgs);
				},
				(postEventArgs) => {
					OnSearchedKeyMembership(postEventArgs);
					SearchedKeyMembership?.Invoke(this, postEventArgs);
				}
			);

		public bool Remove(TKey key) => DoOperation(
			EventTraits.Remove,
			() => InnerCollection.Remove(key),
			() => new RemovingItemsEventArgs<TKey> { CallArgs = new ItemsCallArgs<TKey>(key) },
			result => new RemovedItemsEventArgs<TKey> { Result = new [] { result }},
			(preEventArgs) => {
				OnRemovingKeys(preEventArgs);
				RemovingKeys?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnRemovedKeys(postEventArgs);
				RemovedKeys?.Invoke(this, postEventArgs);
			}
		);

		public bool TryGetValue(TKey key, out TValue value) {
			Tuple<bool, TValue> result = default;
			DoOperation(
				EventTraits.Fetch,
				() => {
					var success = InnerCollection.TryGetValue(key, out var val);
					result = Tuple.Create(success, val);
					return result;
				},
				() => new FetchingByItemsEventArgs<TKey> { CallArgs = new ItemsCallArgs<TKey>(key) },
				result => new FetchedByItemsEventArgs<TKey, Tuple<bool, TValue>> { Result = new[] { result } },
				(preEventArgs) => {
					OnFetching(preEventArgs);
					Fetching?.Invoke(this, preEventArgs);
				},
				(postEventArgs) => {
					OnFetched(postEventArgs);
					Fetched?.Invoke(this, postEventArgs);
				}
			);
			if (result.Item1) {
				value = result.Item2;
				return true;
			}
			value = default;
			return false;
		}

		public TValue this[TKey key] {
			get => DoOperation(
				EventTraits.Fetch,
				() => InnerCollection[key],
				() => new FetchingByItemsEventArgs<TKey>{ CallArgs = new ItemsCallArgs<TKey>(key) },
				result => new FetchedByItemsEventArgs<TKey, Tuple<bool, TValue>> { Result = new [] { Tuple.Create(true, result) } },
				(preEventArgs) => {
					OnFetching(preEventArgs);
					Fetching?.Invoke(this, preEventArgs);
				},
				(postEventArgs) => {
					OnFetched(postEventArgs);
					Fetched?.Invoke(this, postEventArgs);
				}
			);

			set => DoOperation(
				EventTraits.Update,
				() => InnerCollection[key] = value,
				() => new UpdatingByItemsEventArgs<KeyValuePair<TKey, TValue>> { CallArgs = new ItemsCallArgs<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value)) },
				result => new UpdatedByItemsEventArgs<KeyValuePair<TKey, TValue>>(),
				(preEventArgs) => {
					OnUpdating(preEventArgs);
					Updating?.Invoke(this, preEventArgs);
				},
				(postEventArgs) => {
					OnUpdated(postEventArgs);
					Updated?.Invoke(this, postEventArgs);
				}
			);
		}


		public ICollection<TKey> Keys =>
			DoOperation(
				EventTraits.Fetch,
				() => InnerCollection.Keys,
				() => new EnumeratingEventArgs(), 
				result => new EnumeratedEventArgs<TKey> { Result = result},
				(preEventArgs) => {
					OnFetchingKeysCollection(preEventArgs);
					FetchingKeysCollection?.Invoke(this, preEventArgs);
				},
				(postEventArgs) => {
					OnFetchedKeysCollection(postEventArgs);
					FetchedKeysCollection?.Invoke(this, postEventArgs);
				}
			);

		public ICollection<TValue> Values =>
			DoOperation(
				EventTraits.Fetch,
				() => InnerCollection.Values,
				() => new EnumeratingEventArgs(),
				result => new EnumeratedEventArgs<TValue> { Result = result },
				(preEventArgs) => {
					OnFetchingValuesCollection(preEventArgs);
					FetchingValuesCollection?.Invoke(this, preEventArgs);
				},
				(postEventArgs) => {
					OnFetchedValuesCollection(postEventArgs);
					FetchedValuesCollection?.Invoke(this, postEventArgs);
				}
			);

		protected virtual void OnSearchingKeyMembership(SearchingMembershipEventArgs<TKey> args) {
		}

		protected virtual void OnSearchedKeyMembership(SearchedMembershipEventArgs<TKey> args) {
		}

		protected virtual void OnFetching(FetchingByItemsEventArgs<TKey> args) {
		}

		protected virtual void OnFetched(FetchedByItemsEventArgs<TKey, Tuple<bool, TValue>> args) {
		}

		protected virtual void OnFetchingKeysCollection(EnumeratingEventArgs args) {
		}

		protected virtual void OnFetchedKeysCollection(EnumeratedEventArgs<TKey> args) {
		}

		protected virtual void OnFetchingValuesCollection(EnumeratingEventArgs args) {
		}

		protected virtual void OnFetchedValuesCollection(EnumeratedEventArgs<TValue> args) {
		}

		protected virtual void OnUpdating(UpdatingByItemsEventArgs<KeyValuePair<TKey, TValue>> args) {
		}

		protected virtual void OnUpdated(UpdatedByItemsEventArgs<KeyValuePair<TKey, TValue>> args) {
		}

		protected virtual void OnRemovingKeys(RemovingItemsEventArgs<TKey> args) {
		}

		protected virtual void OnRemovedKeys(RemovedItemsEventArgs<TKey> args) {
		}
		
	}
}