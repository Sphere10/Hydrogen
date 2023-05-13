// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen {
	public class SynchronizedHeap<T> : SynchronizedCollection<T>, IHeap<T> where T : IComparable<T> {
		private readonly IHeap<T> _internalHeap;

		public SynchronizedHeap() : this(new BinaryHeap<T>()) {
		}

		public SynchronizedHeap(IHeap<T> internalHeap) : base(internalHeap) {
			_internalHeap = internalHeap;
		}

		#region IHeap Implementation

		public T Pop() {
			using (EnterWriteScope()) return _internalHeap.Pop();
		}

		public T Peek() {
			using (EnterReadScope()) return _internalHeap.Peek();
		}

		#endregion
	}
}
