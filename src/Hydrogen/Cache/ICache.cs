// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen {

	// ICache is a fully functional cache of <object, object>
	public interface ICache : ISynchronizedObject {

		event EventHandlerEx<object> ItemFetching;
		event EventHandlerEx<object, object> ItemFetched;
		event EventHandlerEx<object, CachedItem> ItemRemoved;

		IEnumerable<CachedItem> CachedItems { get; }

		int ItemCount { get; }

		long CurrentSize { get; }

        long MaxCapacity { get; }

        DateTime LastUpdateOn { get; }

        DateTime LastAccessedOn { get; }

        long TotalAccesses { get; }

		TimeSpan ExpirationDuration { get; }

        ExpirationPolicy ExpirationPolicy { get; }

        CacheReapPolicy ReapPolicy { get; }

        NullValuePolicy NullValuePolicy { get; }

        bool ContainsCachedItem(object key);

        CachedItem Get(object key);

        void Set(object key, object value);

        void BulkLoad(IEnumerable<KeyValuePair<object, object>> bulkLoadedValues);

        void Invalidate(object key);

        void Remove(object key);

		void Flush();

		object this[object index] { get; set; }

	}

	public interface ICache<TKey, TValue> : ICache {

		new event EventHandlerEx<TKey, TValue> ItemFetched;
		
		new event EventHandlerEx<TKey, CachedItem<TValue>> ItemRemoved;

		new IEnumerable<CachedItem<TValue>> CachedItems { get; }

		bool ContainsCachedItem(TKey key);

		CachedItem<TValue> Get(TKey key);

		void Set(TKey key, TValue value);

		void BulkLoad(IEnumerable<KeyValuePair<TKey, TValue>> bulkLoadedValues);

		void Invalidate(TKey key);

		void Remove(TKey key);

		TValue this[TKey index] { get; set; }

	}

}
