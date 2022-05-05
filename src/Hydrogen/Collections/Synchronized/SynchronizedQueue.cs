//-----------------------------------------------------------------------
// <copyright file="SynchronizedQueue.cs" company="Sphere 10 Software">
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
using System.Collections;

namespace Hydrogen {

	public class SynchronizedQueue<T> : SynchronizedObject, ICollection<T>, IReadOnlyCollection<T> {
		private readonly Queue<T> _internalQueue;

		public SynchronizedQueue()
            : this(new Queue<T>()) {
		}
        public SynchronizedQueue(Queue<T> internalQueue) {
            _internalQueue = internalQueue;
		}

        #region ICollection Implementation

        public void Add(T item) { using (EnterWriteScope()) _internalQueue.Enqueue(item); }
        public void Clear() { using(EnterWriteScope()) _internalQueue.Clear(); }
        public bool Contains(T item) { using (EnterReadScope()) return _internalQueue.Contains(item); }
        public int Count { get { using (EnterReadScope()) return _internalQueue.Count; } }
        public bool IsReadOnly => false;
        public void CopyTo(T[] array, int arrayIndex) { using (EnterReadScope())  _internalQueue.CopyTo(array, arrayIndex); }
        public bool Remove(T item) { throw new NotSupportedException();}
        public IEnumerator<T> GetEnumerator() { using (EnterReadScope()) return _internalQueue.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { using (EnterReadScope()) return (_internalQueue as IEnumerable).GetEnumerator(); }

        #endregion

		#region Methods

        public T Peek() {
            using (EnterReadScope())
                return _internalQueue.Peek();
        }

	    public void Enqueue(T value) {
            Add(value);
	    }

        public T Dequeue() {
            using (EnterWriteScope())
                return _internalQueue.Dequeue();
        }

		#endregion
	}

}

