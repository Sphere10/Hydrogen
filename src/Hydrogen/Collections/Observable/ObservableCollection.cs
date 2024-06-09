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

public class ObservableCollection<TItem, TConcrete> : CollectionDecorator<TItem, TConcrete>, IObservableCollection
	where TConcrete : ICollection<TItem> {

	public event EventHandlerEx<object, EventTraits> Accessing;
	public event EventHandlerEx<object, EventTraits> Accessed;
	public event EventHandlerEx<object, EventTraits> Reading;
	public event EventHandlerEx<object, EventTraits> Read;
	public event EventHandlerEx<object, EventTraits> Mutating;
	public event EventHandlerEx<object, EventTraits> Mutated;
	public event EventHandlerEx<object, CountingEventArgs> Counting;
	public event EventHandlerEx<object, CountedEventArgs> Counted;
	public event EventHandlerEx<object, SearchingMembershipEventArgs<TItem>> SearchingMembership;
	public event EventHandlerEx<object, SearchedMembershipEventArgs<TItem>> SearchedMembership;
	public event EventHandlerEx<object, AddingEventArgs<TItem>> Adding;
	public event EventHandlerEx<object, AddedEventArgs<TItem>> Added;
	public event EventHandlerEx<object, RemovingItemsEventArgs<TItem>> RemovingItems;
	public event EventHandlerEx<object, RemovedItemsEventArgs<TItem>> RemovedItems;
	public event EventHandlerEx<object, PreEventArgs> Clearing;
	public event EventHandlerEx<object, PostEventArgs> Cleared;
	public event EventHandlerEx<object, PreEventArgs<CallArgs>> Copying;
	public event EventHandlerEx<object, PostEventArgs<CallArgs>> Copied;
	public event EventHandlerEx<object, EnumeratingItemEventArgs> EnumeratingItem;
	public event EventHandlerEx<object, EnumeratedItemEventArgs<TItem>> EnumeratedItem;

	public ObservableCollection(TConcrete internalCollection)
		: base(internalCollection) {
		EventFilter = EventTraits.All;
	}

	public virtual bool SuppressNotifications { get; set; }

	public EventTraits EventFilter { get; set; }

	public override int Count =>
		DoOperation(
			EventTraits.Count,
			() => base.Count,
			() => new CountingEventArgs(),
			result => new CountedEventArgs { Result = result },
			NotifyCounting,
			NotifyCounted
		);

	public override bool Contains(TItem item) =>
		DoOperation(
			EventTraits.Search,
			() => base.Contains(item),
			() => new SearchingMembershipEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(item) },
			result => new SearchedMembershipEventArgs<TItem> { Result = new[] { result } },
			(preEventArgs) => {
				OnSearchingMembership(preEventArgs);
				SearchingMembership?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnSearchedMembership(postEventArgs);
				SearchedMembership?.Invoke(this, postEventArgs);
			}
		);

	public override void Add(TItem item) =>
		DoOperation(
			EventTraits.Add,
			() => {
				base.Add(item);
				return 0;
			},
			() => new AddingEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(item) },
			_ => new AddedEventArgs<TItem>(),
			(preEventArgs) => {
				OnAdding(preEventArgs);
				Adding?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnAdded(postEventArgs);
				Added?.Invoke(this, postEventArgs);
			}
		);

	public override bool Remove(TItem item) =>
		DoOperation(
			EventTraits.Remove,
			() => base.Remove(item),
			() => new RemovingItemsEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(item) },
			result => new RemovedItemsEventArgs<TItem> { Result = new[] { result } },
			(preEventArgs) => {
				OnRemovingItems(preEventArgs);
				RemovingItems?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnRemovedItems(postEventArgs);
				RemovedItems?.Invoke(this, postEventArgs);
			}
		);

	public override void Clear() =>
		DoOperation(
			EventTraits.Remove,
			() => {
				base.Clear();
				return 0;
			},
			() => new PreEventArgs(),
			_ => new PostEventArgs(),
			(preEventArgs) => {
				OnClearing(preEventArgs);
				Clearing?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnCleared(postEventArgs);
				Cleared?.Invoke(this, postEventArgs);
			}
		);

	public override void CopyTo(TItem[] array, int arrayIndex) =>
		DoOperation(
			EventTraits.Remove,
			() => {
				base.CopyTo(array, arrayIndex);
				return 0;
			},
			() => new PreEventArgs<CallArgs>(),
			_ => new PostEventArgs<CallArgs>(),
			(preEventArgs) => {
				OnCopying(preEventArgs);
				Copying?.Invoke(this, preEventArgs);
			},
			(postEventArgs) => {
				OnCopied(postEventArgs);
				Copied?.Invoke(this, postEventArgs);
			}
		);

	public override IEnumerator<TItem> GetEnumerator() {
		return new ObservableEnumerator(this, base.GetEnumerator());
	}

	protected virtual void OnAccessing(EventTraits eventType) {
	}

	protected virtual void OnAccessed(EventTraits eventType) {
	}

	protected virtual void OnReading(EventTraits eventType) {
	}

	protected virtual void OnRead(EventTraits eventType) {
	}

	protected virtual void OnMutating(EventTraits eventType) {
	}

	protected virtual void OnMutated(EventTraits eventType) {
	}

	protected virtual void OnCounting(CountingEventArgs args) {
	}

	protected virtual void OnCounted(CountedEventArgs args) {
	}

	protected virtual void OnSearchingMembership(SearchingMembershipEventArgs<TItem> args) {
	}

	protected virtual void OnSearchedMembership(SearchedMembershipEventArgs<TItem> args) {
	}

	protected virtual void OnAdding(AddingEventArgs<TItem> args) {
	}

	protected virtual void OnAdded(AddedEventArgs<TItem> args) {
	}

	protected virtual void OnRemovingItems(RemovingItemsEventArgs<TItem> args) {
	}

	protected virtual void OnRemovedItems(RemovedItemsEventArgs<TItem> args) {
	}

	protected virtual void OnClearing(PreEventArgs args) {
	}

	protected virtual void OnCleared(PostEventArgs args) {
	}

	protected virtual void OnCopying(PreEventArgs<CallArgs> args) {
	}

	protected virtual void OnCopied(PostEventArgs<CallArgs> args) {
	}

	protected virtual void OnEnumeratingItem(EnumeratingItemEventArgs args) {
	}

	protected virtual void OnEnumeratedItem(EnumeratedItemEventArgs<TItem> args) {
	}

	protected void NotifyCounting(CountingEventArgs args) {
		OnCounting(args);
		Counting?.Invoke(this, args);
	}

	protected void NotifyCounted(CountedEventArgs args) {
		OnCounted(args);
		Counted?.Invoke(this, args);
	}

	protected void NotifySearchingMembership(SearchingMembershipEventArgs<TItem> args) {
		SearchingMembership?.Invoke(this, args);
	}

	protected void NotifySearchedMembership(SearchedMembershipEventArgs<TItem> args) {
		SearchedMembership?.Invoke(this, args);
	}

	protected void NotifyAdding(AddingEventArgs<TItem> args) {
		Adding?.Invoke(this, args);
	}

	protected void NotifyAdded(AddedEventArgs<TItem> args) {
		Added?.Invoke(this, args);
	}

	protected void NotifyRemovingItems(RemovingItemsEventArgs<TItem> args) {
		RemovingItems?.Invoke(this, args);
	}

	protected void NotifyRemovedItems(RemovedItemsEventArgs<TItem> args) {
		RemovedItems?.Invoke(this, args);
	}

	protected TOpResult DoOperation<TOpResult, TPre, TPost>(
		EventTraits operationTrait,
		Func<TOpResult> opFunc,
		Func<TPre> preOpEventArgsConstructor,
		Func<TOpResult, TPost> postOpArgsConstructor,
		Action<TPre> preEventNotify,
		Action<TPost> postEventNotify)
		where TPre : PreEventArgs
		where TPost : PostEventArgs {
		Guard.ArgumentNotNull(preEventNotify, nameof(preEventNotify));
		Guard.ArgumentNotNull(postEventNotify, nameof(postEventNotify));
		Guard.ArgumentInRange((uint)operationTrait, (uint)EventTraits.Count, (uint)EventTraits.Remove, nameof(operationTrait));

		// Optimized Case: notifications suppressed, by-pass all notification workflow
		if (SuppressNotifications)
			return opFunc();

		// Notify accessing
		if (EventFilter.HasFlag(EventTraits.Access) && EventFilter.HasFlag(EventTraits.Access)) {
			OnAccessing(operationTrait);
			Accessing?.Invoke(this, operationTrait);
		}

		// Notify reading
		if (operationTrait.HasFlag(EventTraits.Read) && EventFilter.HasFlag(EventTraits.Read)) {
			OnReading(operationTrait);
			Reading?.Invoke(this, operationTrait);
		}

		// Notify mutating
		if (operationTrait.HasFlag(EventTraits.Write) && EventFilter.HasFlag(EventTraits.Write)) {
			OnMutating(operationTrait);
			Mutating?.Invoke(this, operationTrait);
		}

		// Pre-operation notification & interception workflow
		TPre preOpEventArgs = null;
		if (EventFilter.HasFlag(operationTrait)) {
			preOpEventArgs = preOpEventArgsConstructor();
			preEventNotify?.Invoke(preOpEventArgs);
		}

		// Evaluate
		var result = opFunc();

		// Notify accessed
		if (operationTrait.HasFlag(EventTraits.Access) && EventFilter.HasFlag(EventTraits.Access)) {
			OnAccessed(operationTrait);
			Accessed?.Invoke(this, operationTrait);
		}

		// Notify read
		if (operationTrait.HasFlag(EventTraits.Read) && EventFilter.HasFlag(EventTraits.Read)) {
			OnRead(operationTrait);
			Read?.Invoke(this, operationTrait);
		}


		// Notify mutated
		if (operationTrait.HasFlag(EventTraits.Write) && EventFilter.HasFlag(EventTraits.Write)) {
			OnMutated(operationTrait);
			Mutated?.Invoke(this, operationTrait);
		}

		// Post-operation notification
		var postOpEventArgs = postOpArgsConstructor(result);
		if (EventFilter.HasFlag(operationTrait) &&
		    preOpEventArgs is PreEventArgs<ItemsCallArgs<TItem>> preOpEventArgs2 &&
		    postOpEventArgs is PostEventArgs<ItemsCallArgs<TItem>> postOp2) {
			postOp2.CallArgs = preOpEventArgs2.CallArgs;
			postEventNotify.Invoke(postOpEventArgs);
		}

		return result;
	}


	private class ObservableEnumerator : EnumeratorDecorator<TItem> {
		private readonly ObservableCollection<TItem, TConcrete> _collection;
		private int _index;

		public ObservableEnumerator(ObservableCollection<TItem, TConcrete> collection, IEnumerator<TItem> enumerator)
			: base(enumerator) {
			_collection = collection;
			_index = 0;
		}

		public override bool MoveNext() {
			var result = _collection.DoOperation(
				EventTraits.Fetch,
				() => base.MoveNext(),
				() => new EnumeratingItemEventArgs(),
				_ => new EnumeratedItemEventArgs<TItem> { Result = Current },
				(preEventArgs) => {
					_collection.OnEnumeratingItem(preEventArgs);
					_collection.EnumeratingItem?.Invoke(this, preEventArgs);
				},
				(postEventArgs) => {
					_collection.OnEnumeratedItem(postEventArgs);
					_collection.EnumeratedItem?.Invoke(this, postEventArgs);
				}
			);
			_index++;
			return result;
		}

		public override void Reset() {
			base.Reset();
			_index = 0;
		}
	}

}


public class ObservableCollection<TItem> : ObservableCollection<TItem, ICollection<TItem>> {

	public ObservableCollection()
		: this(new ExtendedList<TItem>()) {
	}
	public ObservableCollection(ICollection<TItem> internalCollection)
		: base(internalCollection) {
	}
}
