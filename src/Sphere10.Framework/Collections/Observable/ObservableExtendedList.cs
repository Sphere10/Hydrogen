//-----------------------------------------------------------------------
// <copyright file="ObservableExtendedList.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Sphere10.Framework.Collections;

namespace Sphere10.Framework {

	public class ObservableExtendedList<TItem> : ObservableExtendedCollection<TItem>, IExtendedList<TItem> {
		protected new readonly IExtendedList<TItem> InnerCollection;

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

		public ObservableExtendedList()
			: this(new ExtendedList<TItem>()) {
		}

		public ObservableExtendedList(IExtendedList<TItem> internalExtendedList)
			: base(internalExtendedList) {
			InnerCollection = (IExtendedList<TItem>)base.InnerCollection;
		}


		public int IndexOf(TItem item) => DoOperation(
			EventTraits.Search,
			() => InnerCollection.IndexOf(item),
			() => new SearchingLocationEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(item) },
			result => new SearchedLocationEventArgs<TItem> { Result = new[] { result } },
			(preEventArgs) => {
				OnSearchingLocation(preEventArgs);
				SearchingLocation?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnSearchedLocation(postEventArgs);
				SearchedLocation?.Invoke(this, postEventArgs);
			}
		);


		public IEnumerable<int> IndexOfRange(IEnumerable<TItem> items) => DoOperation(
			EventTraits.Search,
			() => InnerCollection.IndexOfRange(items),
			() => new SearchingLocationEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(items) },
			result => new SearchedLocationEventArgs<TItem> { Result = result  },
			(preEventArgs) => {
				OnSearchingLocation(preEventArgs);
				SearchingLocation?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnSearchedLocation(postEventArgs);
				SearchedLocation?.Invoke(this, postEventArgs);
			}
		);


		public TItem Read(int index) => DoOperation(
			EventTraits.Fetch,
			() => InnerCollection.Read(index),
			() => new FetchingByRangeEventArgs { CallArgs = new IndexCountCallArgs(index, 1) },
			result => new FetchedByRangeEventArgs<TItem> {
				Result = new[] { result }
			},
			(preEventArgs) => {
				OnFetching(preEventArgs);
				Fetching?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnFetched(postEventArgs);
				Fetched?.Invoke(this, postEventArgs);
			}
		);

		public IEnumerable<TItem> ReadRange(int index, int count) => DoOperation(
			EventTraits.Fetch,
			() => InnerCollection.ReadRange(index, count),
			() => new FetchingByRangeEventArgs { CallArgs = new IndexCountCallArgs(index, count) },
			result => new FetchedByRangeEventArgs<TItem> {
				Result = result
			},
			(preEventArgs) => {
				OnFetching(preEventArgs);
				Fetching?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnFetched(postEventArgs);
				Fetched?.Invoke(this, postEventArgs);
			}
		);

		public virtual void Update(int index, TItem item) => DoOperation(
			EventTraits.Update,
			() => {
				InnerCollection.Update(index, item);
				return 0;
			},
			() => new UpdatingByRangeEventArgs<TItem> { CallArgs = new IndexItemsCallArgs<TItem>(index, item) },
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

		public virtual void UpdateRange(int index, IEnumerable<TItem> items) => DoOperation(
			EventTraits.Update,
			() => {
				InnerCollection.UpdateRange(index, items);
				return 0;
			},
			() => new UpdatingByRangeEventArgs<TItem> { CallArgs = new IndexItemsCallArgs<TItem>(index, items) },
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

		public virtual void Insert(int index, TItem item) => DoOperation(
			EventTraits.Insert,
			() => {
				InnerCollection.Insert(index, item);
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

		public virtual void InsertRange(int index, IEnumerable<TItem> items) => DoOperation(
			EventTraits.Insert,
			() => {
				InnerCollection.InsertRange(index, items);
				return 0;
			},
			() => new InsertingEventArgs<TItem> { CallArgs = new IndexItemsCallArgs<TItem>(index, items) },
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
				InnerCollection.RemoveAt(index);
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

		public virtual void RemoveRange(int index, int count) => DoOperation(
			EventTraits.Remove,
			() => {
				InnerCollection.RemoveRange(index, count);
				return 0;
			},
			() => new RemovingRangeEventArgs { CallArgs = new IndexCountCallArgs(index, count) },
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
			get => Read(index);
			set => Update(index, value);
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
}