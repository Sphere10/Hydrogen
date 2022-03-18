//-----------------------------------------------------------------------
// <copyright file="ICache.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;

namespace Sphere10.Framework {

	// ICache is a fully functional cache of <object, object>
	public interface ICache : ISynchronizedObject {

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

		object this[object index] { get; set; }

        void BulkLoad(IEnumerable<KeyValuePair<object, object>> bulkLoadedValues);

        void Invalidate(object key);

        void Remove(object key);

		void Flush();

	}

	public interface ICache<TKey, TValue> : ICache {

		new event EventHandlerEx<TKey, TValue> ItemFetched;
		
		new event EventHandlerEx<TKey, CachedItem<TValue>> ItemRemoved;

		new IEnumerable<CachedItem<TValue>> CachedItems { get; }

		bool ContainsCachedItem(TKey key);

		CachedItem<TValue> Get(TKey key);

		void Set(TKey key, TValue value);

		TValue this[TKey index] { get; set; }

		void BulkLoad(IEnumerable<KeyValuePair<TKey, TValue>> bulkLoadedValues);

		void Invalidate(TKey key);

		void Remove(TKey key);

	}

}
