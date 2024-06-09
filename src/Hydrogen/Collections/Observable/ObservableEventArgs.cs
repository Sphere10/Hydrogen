// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class CountingEventArgs : PreEventArgs<CallArgs> {
}


public sealed class CountedEventArgs : PostEventArgs<CallArgs, long> {
}


public sealed class SearchingMembershipEventArgs<T> : PreEventArgs<ItemsCallArgs<T>> {
}


public sealed class SearchedMembershipEventArgs<T> : PostEventArgs<ItemsCallArgs<T>, IEnumerable<bool>> {
}


public sealed class SearchingLocationEventArgs<T> : PreEventArgs<ItemsCallArgs<T>> {
}


public sealed class SearchedLocationEventArgs<T> : PostEventArgs<ItemsCallArgs<T>, IEnumerable<long>> {
}


public sealed class EnumeratingItemEventArgs : PreEventArgs {
}


public sealed class EnumeratedItemEventArgs<T> : PostEventArgs<VoidCallArgs, T> {
}


public sealed class EnumeratingEventArgs : PreEventArgs {
}


public sealed class EnumeratedEventArgs<T> : PostEventArgs<VoidCallArgs, IEnumerable<T>> {
}


public sealed class FetchingByRangeEventArgs : PreEventArgs<IndexCountCallArgs> {
}


public sealed class FetchedByRangeEventArgs<T> : PostEventArgs<IndexCountCallArgs, IEnumerable<T>> {
}


public sealed class FetchingByItemsEventArgs<TKey> : PreEventArgs<ItemsCallArgs<TKey>> {
}


public sealed class FetchedByItemsEventArgs<TKey, TValue> : PostEventArgs<ItemsCallArgs<TKey>, IEnumerable<TValue>> {
}


public sealed class AddingEventArgs<T> : PreEventArgs<ItemsCallArgs<T>> {
}


public sealed class AddedEventArgs<T> : PostEventArgs<ItemsCallArgs<T>> {
}


public sealed class InsertingEventArgs<T> : PreEventArgs<IndexItemsCallArgs<T>> {
}


public sealed class InsertedEventArgs<T> : PostEventArgs<IndexItemsCallArgs<T>> {
}


public sealed class UpdatingByRangeEventArgs<T> : PreEventArgs<IndexItemsCallArgs<T>> {
}


public sealed class UpdatedByRangeEventArgs<T> : PostEventArgs<IndexItemsCallArgs<T>> {
}


public sealed class UpdatingByItemsEventArgs<T> : PreEventArgs<ItemsCallArgs<T>> {
}


public sealed class UpdatedByItemsEventArgs<T> : PostEventArgs<ItemsCallArgs<T>> {
}


public sealed class RemovingRangeEventArgs : PreEventArgs<IndexCountCallArgs> {
}


public sealed class RemovedRangeEventArgs : PostEventArgs<IndexCountCallArgs> {
}


public sealed class RemovingItemsEventArgs<T> : PreEventArgs<ItemsCallArgs<T>> {
}


public sealed class RemovedItemsEventArgs<T> : PostEventArgs<ItemsCallArgs<T>, IEnumerable<bool>> {
}
