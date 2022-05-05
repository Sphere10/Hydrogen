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

using System.Collections;
using System.Collections.Generic;

namespace Hydrogen {
	public abstract class CollectionDecorator<TItem, TConcrete> : ICollection<TItem> where TConcrete : ICollection<TItem> {
		protected TConcrete InternalCollection;

		protected CollectionDecorator(TConcrete innerCollection) {
			InternalCollection = innerCollection;
		}

		public virtual IEnumerator<TItem> GetEnumerator() {
			return InternalCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public virtual void Add(TItem item) {
			InternalCollection.Add(item);
		}

		public virtual void Clear() {
			InternalCollection.Clear();
		}

		public virtual bool Contains(TItem item) {
			return InternalCollection.Contains(item);
		}

		public virtual void CopyTo(TItem[] array, int arrayIndex) {
			InternalCollection.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(TItem item) {
			return InternalCollection.Remove(item);
		}

		public virtual int Count => InternalCollection.Count;

		public virtual bool IsReadOnly => InternalCollection.IsReadOnly;
	}

	public abstract class CollectionDecorator<TItem> : CollectionDecorator<TItem, ICollection<TItem>> {
		protected CollectionDecorator(ICollection<TItem> innerCollection)
			: base(innerCollection) {
		}
	}
}
