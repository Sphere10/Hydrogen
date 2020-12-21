//-----------------------------------------------------------------------
// <copyright file="SynchronizedHeap.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Threading;

namespace Sphere10.Framework {
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
