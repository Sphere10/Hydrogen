//-----------------------------------------------------------------------
// <copyright file="ObservableCollection.cs" company="Sphere 10 Software">
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

using System.Collections.Generic;

namespace Hydrogen {

	public class ObservableExtendedCollection<TItem> : ObservableExtendedCollection<TItem, IExtendedCollection<TItem>> {

		public ObservableExtendedCollection()
			: this(new ExtendedList<TItem>()) {
		}

		public ObservableExtendedCollection(IExtendedCollection<TItem> internalCollection)
			: base(internalCollection) {
		}

	}

	public class ObservableExtendedCollection<TItem, TConcrete> : ObservableCollection<TItem, TConcrete>, IExtendedCollection<TItem>
		where TConcrete : IExtendedCollection<TItem> {

		public ObservableExtendedCollection(TConcrete internalCollection)
			: base(internalCollection) {
		}

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
}
