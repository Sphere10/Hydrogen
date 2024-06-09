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

public class ObservableDictionary<TKey, TValue, TConcrete> : ObservableCollection<KeyValuePair<TKey, TValue>, TConcrete>, IDictionary<TKey, TValue>
	where TConcrete : IDictionary<TKey, TValue> {

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

	public ObservableDictionary(TConcrete internalDictionary)
		: base(internalDictionary) {
	}

	public void Add(TKey key, TValue value) =>
		DoOperation(
			EventTraits.Add,
			() => {
				InternalCollection.Add(key, value);
				return 0;
			},
			() => new AddingEventArgs<KeyValuePair<TKey, TValue>> { CallArgs = new ItemsCallArgs<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value)) },
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
			() => InternalCollection.ContainsKey(key),
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
		() => InternalCollection.Remove(key),
		() => new RemovingItemsEventArgs<TKey> { CallArgs = new ItemsCallArgs<TKey>(key) },
		result => new RemovedItemsEventArgs<TKey> { Result = new[] { result } },
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
				var success = InternalCollection.TryGetValue(key, out var val);
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
			() => InternalCollection[key],
			() => new FetchingByItemsEventArgs<TKey> { CallArgs = new ItemsCallArgs<TKey>(key) },
			result => new FetchedByItemsEventArgs<TKey, Tuple<bool, TValue>> { Result = new[] { Tuple.Create(true, result) } },
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
			() => InternalCollection[key] = value,
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
			() => InternalCollection.Keys,
			() => new EnumeratingEventArgs(),
			result => new EnumeratedEventArgs<TKey> { Result = result },
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
			() => InternalCollection.Values,
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
public class ObservableDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue, IDictionary<TKey, TValue>> {

	public ObservableDictionary()
		: this(new Dictionary<TKey, TValue>()) {
	}

	public ObservableDictionary(IDictionary<TKey, TValue> internalDictionary)
		: base(internalDictionary) {
	}
}
