//-----------------------------------------------------------------------
// <copyright file="CollectionDecorator.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework {
	public abstract class CollectionDecorator<TItem, TCollection> : ICollection<TItem> where TCollection : ICollection<TItem> {
		protected readonly TCollection InnerCollection;

		protected CollectionDecorator(TCollection innerCollection) {
			InnerCollection = innerCollection;
		}

		public virtual IEnumerator<TItem> GetEnumerator() {
			return InnerCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public virtual void Add(TItem item) {
			InnerCollection.Add(item);
		}

		public virtual void Clear() {
			InnerCollection.Clear();
		}

		public virtual bool Contains(TItem item) {
			return InnerCollection.Contains(item);
		}

		public virtual void CopyTo(TItem[] array, int arrayIndex) {
			InnerCollection.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(TItem item) {
			return InnerCollection.Remove(item);
		}

		public virtual int Count => InnerCollection.Count;

		public virtual bool IsReadOnly => InnerCollection.IsReadOnly;
	}

	public abstract class CollectionDecorator<TItem> : CollectionDecorator<TItem, ICollection<TItem>> {
		protected CollectionDecorator(ICollection<TItem> innerCollection)
			: base(innerCollection) {
		}
	}
}
