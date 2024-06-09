// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class ObservableList<TItem> : ObservableList<TItem, IList<TItem>> {
	public ObservableList()
		: this(new List<TItem>()) {
	}

	public ObservableList(IList<TItem> internalList)
		: base(internalList) {
	}
}


public class ObservableList<TItem, TConcrete> : ObservableCollection<TItem, TConcrete>, IList<TItem>
	where TConcrete : IList<TItem> {

	public event EventHandlerEx<object, SearchingLocationEventArgs<TItem>> SearchingLocation;
	public event EventHandlerEx<object, SearchedLocationEventArgs<TItem>> SearchedLocation;
	public event EventHandlerEx<object, FetchingByRangeEventArgs> Fetching;
	public event EventHandlerEx<object, FetchedByRangeEventArgs<TItem>> Fetched;
	public event EventHandlerEx<object, InsertingEventArgs<TItem>> Inserting;
	public event EventHandlerEx<object, InsertedEventArgs<TItem>> Inserted;
	public event EventHandlerEx<object, UpdatingByRangeEventArgs<TItem>> Updating;
	public event EventHandlerEx<object, UpdatedByRangeEventArgs<TItem>> Updated;
	public event EventHandlerEx<object, RemovingRangeEventArgs> RemovingRange;
	public event EventHandlerEx<object, RemovedRangeEventArgs> RemovedRange;

	public ObservableList(TConcrete internalList)
		: base(internalList) {
	}

	public int IndexOf(TItem item) => DoOperation(
		EventTraits.Search,
		() => InternalCollection.IndexOf(item),
		() => new SearchingLocationEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(item) },
		result => new SearchedLocationEventArgs<TItem> { Result = new long[] { result } },
		(preEventArgs) => {
			OnSearchingLocation(preEventArgs);
			SearchingLocation?.Invoke(this, preEventArgs);
		},
		(postEventArgs) => {
			OnSearchedLocation(postEventArgs);
			SearchedLocation?.Invoke(this, postEventArgs);
		}
	);

	public void Insert(int index, TItem item) => DoOperation(
		EventTraits.Insert,
		() => {
			InternalCollection.Insert(index, item);
			return 0;
		},
		() => new InsertingEventArgs<TItem> { CallArgs = new IndexItemsCallArgs<TItem>(index, item) },
		_ => new InsertedEventArgs<TItem>(),
		(preEventArgs) => {
			OnInserting(preEventArgs);
			Inserting?.Invoke(this, preEventArgs);
		},
		(postEventArgs) => {
			OnInserted(postEventArgs);
			Inserted?.Invoke(this, postEventArgs);
		}
	);

	public void RemoveAt(int index) => DoOperation(
		EventTraits.Remove,
		() => {
			InternalCollection.RemoveAt(index);
			return 0;
		},
		() => new RemovingRangeEventArgs { CallArgs = new IndexCountCallArgs(index, 1) },
		_ => new RemovedRangeEventArgs(),
		(preEventArgs) => {
			OnRemovingRange(preEventArgs);
			RemovingRange?.Invoke(this, preEventArgs);
		},
		(postEventArgs) => {
			OnRemovedRange(postEventArgs);
			RemovedRange?.Invoke(this, postEventArgs);
		}
	);

	public TItem this[int index] {
		get => DoOperation(
			EventTraits.Fetch,
			() => InternalCollection[index],
			() => new FetchingByRangeEventArgs { CallArgs = new IndexCountCallArgs(index, 1) },
			result => new FetchedByRangeEventArgs<TItem> { Result = new[] { result } },
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
			() => InternalCollection[index] = value,
			() => new UpdatingByRangeEventArgs<TItem> { CallArgs = new IndexItemsCallArgs<TItem>(index, value) },
			result => new UpdatedByRangeEventArgs<TItem>(),
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

	protected virtual void OnSearchingLocation(SearchingLocationEventArgs<TItem> args) {
	}

	protected virtual void OnSearchedLocation(SearchedLocationEventArgs<TItem> args) {
	}

	protected virtual void OnFetching(FetchingByRangeEventArgs args) {
	}

	protected virtual void OnFetched(FetchedByRangeEventArgs<TItem> args) {
	}

	protected virtual void OnUpdating(UpdatingByRangeEventArgs<TItem> args) {
	}

	protected virtual void OnUpdated(UpdatedByRangeEventArgs<TItem> args) {
	}

	protected virtual void OnInserting(InsertingEventArgs<TItem> args) {
	}

	protected virtual void OnInserted(InsertedEventArgs<TItem> args) {
	}

	protected virtual void OnRemovingRange(RemovingRangeEventArgs args) {
	}

	protected virtual void OnRemovedRange(RemovedRangeEventArgs args) {
	}

}
