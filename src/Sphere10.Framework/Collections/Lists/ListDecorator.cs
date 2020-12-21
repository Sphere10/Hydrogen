//-----------------------------------------------------------------------
// <copyright file="ListDecorator.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {

    public abstract class ListDecorator<T> : IList<T> {
        protected readonly IList<T> InternalList;

        protected ListDecorator() 
            : this(new List<T>()) {
        }

        protected ListDecorator(IList<T> internalList) {
            InternalList = internalList;
        }

        public virtual int IndexOf(T item) {
            return InternalList.IndexOf(item);
        }

        public virtual void Insert(int index, T item) {
            InternalList.Insert(index, item);
        }

		public virtual bool Remove(T item) {
			return InternalList.Remove(item);
		}

        public virtual void RemoveAt(int index) {
            InternalList.RemoveAt(index);
        }

        public virtual T this[int index] {
            get => InternalList[index];
            set => InternalList[index] = value;
        }

        public virtual void Add(T item) {
            InternalList.Add(item);
        }

        public virtual void Clear() {
            InternalList.Clear();
        }

        public virtual bool Contains(T item) {
            return InternalList.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex) {
            InternalList.CopyTo(array, arrayIndex);
        }

        public virtual int Count => InternalList.Count;

        public virtual bool IsReadOnly => InternalList.IsReadOnly;

        public virtual IEnumerator<T> GetEnumerator() {
            return InternalList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return InternalList.GetEnumerator();
        }
    }
}
