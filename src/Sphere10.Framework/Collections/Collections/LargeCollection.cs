//-----------------------------------------------------------------------
// <copyright file="LargeCollection.cs" company="Sphere 10 Software">
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
using System.Linq;

namespace Sphere10.Framework {

	public class LargeCollection<TItem> : CollectionDecorator<TItem>, IDisposable {
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading { add => InternalPagedList.PageLoading += value; remove => InternalPagedList.PageLoading -= value; }
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded { add => InternalPagedList.PageLoaded += value; remove => InternalPagedList.PageLoaded -= value; }
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading { add => InternalPagedList.PageUnloading += value; remove => InternalPagedList.PageUnloading -= value; }
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded { add => InternalPagedList.PageUnloaded += value; remove => InternalPagedList.PageUnloaded -= value; }

		public LargeCollection(int pageSize, int maxOpenPages)
			: this(pageSize, maxOpenPages, null) {
		}

		public LargeCollection(int pageSize, int maxOpenPages, Func<TItem, int> itemSizer)
			: base(new MemoryPagedList<TItem>(pageSize, maxOpenPages, itemSizer)) {
		}

		protected MemoryPagedListBase<TItem> InternalPagedList => (MemoryPagedListBase<TItem>)base.InnerCollection;

		public IReadOnlyList<IMemoryPage<TItem>> Pages => new ReadOnlyListDecorator<IPage<TItem>, IMemoryPage<TItem>>(InternalPagedList.Pages);

		public void Dispose() {
			InternalPagedList.Dispose();
		}

	}
}