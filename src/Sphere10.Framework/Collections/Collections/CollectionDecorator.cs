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
    public abstract class CollectionDecorator<T> : ICollection<T> {
        protected readonly ICollection<T> InnerCollection;

		protected CollectionDecorator(ICollection<T> innerCollection) {
            InnerCollection = innerCollection;
        }

        public virtual IEnumerator<T> GetEnumerator() {
            return InnerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public virtual void Add(T item) {
            InnerCollection.Add(item);
        }

        public virtual void Clear() {
            InnerCollection.Clear();
        }

        public virtual bool Contains(T item) {
            return InnerCollection.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex) {
            InnerCollection.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item) {
            return InnerCollection.Remove(item);
        }

        public virtual int Count => InnerCollection.Count;

		public virtual bool IsReadOnly => InnerCollection.IsReadOnly;
	}
}
