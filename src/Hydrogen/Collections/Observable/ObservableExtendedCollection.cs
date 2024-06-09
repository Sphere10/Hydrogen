// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class ObservableExtendedCollection<TItem, TConcrete> : ObservableCollection<TItem, TConcrete>, IExtendedCollection<TItem>
	where TConcrete : IExtendedCollection<TItem> {

	public ObservableExtendedCollection(TConcrete internalCollection)
		: base(internalCollection) {
	}


	int ICollection<TItem>.Count => checked((int)Count);

	public new virtual long Count =>
		DoOperation(
			EventTraits.Count,
			() => base.Count,
			() => new CountingEventArgs(),
			result => new CountedEventArgs { Result = result },
			NotifyCounting,
			NotifyCounted
		);


	public virtual IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) =>
		DoOperation(
			EventTraits.Search,
			() => InternalCollection.ContainsRange(items),
			() => new SearchingMembershipEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(items) },
			result => new SearchedMembershipEventArgs<TItem> { Result = result },
			(preEventArgs) => {
				OnSearchingMembership(preEventArgs);
				NotifySearchingMembership(preEventArgs);
			},
			(postEventArgs) => {
				OnSearchedMembership(postEventArgs);
				NotifySearchedMembership(postEventArgs);
			}
		);

	public virtual void AddRange(IEnumerable<TItem> items) =>
		DoOperation(
			EventTraits.Add,
			() => {
				InternalCollection.AddRange(items);
				return 0;
			},
			() => new AddingEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(items) },
			_ => new AddedEventArgs<TItem>(),
			(preEventArgs) => {
				OnAdding(preEventArgs);
				NotifyAdding(preEventArgs);
			},
			(postEventArgs) => {
				OnAdded(postEventArgs);
				NotifyAdded(postEventArgs);
			}
		);

	public virtual IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) =>
		DoOperation(
			EventTraits.Remove,
			() => InternalCollection.RemoveRange(items),
			() => new RemovingItemsEventArgs<TItem> { CallArgs = new ItemsCallArgs<TItem>(items) },
			result => new RemovedItemsEventArgs<TItem>(),
			(preEventArgs) => {
				OnRemovingItems(preEventArgs);
				NotifyRemovingItems(preEventArgs);
			},
			(postEventArgs) => {
				OnRemovedItems(postEventArgs);
				NotifyRemovedItems(postEventArgs);
			}
		);
}


public class ObservableExtendedCollection<TItem> : ObservableExtendedCollection<TItem, IExtendedCollection<TItem>> {

	public ObservableExtendedCollection()
		: this(new ExtendedList<TItem>()) {
	}

	public ObservableExtendedCollection(IExtendedCollection<TItem> internalCollection)
		: base(internalCollection) {
	}

}
