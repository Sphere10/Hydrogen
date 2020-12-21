//-----------------------------------------------------------------------
// <copyright file="SortedCollection.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {

	public class SortedCollection<T> : CollectionDecorator<T> where T : IComparable<T> {

		public SortedCollection() : this(new List<T>()) {
		}

		public SortedCollection(IList<T> internalList)
			: base(internalList) {
		}

		protected IList<T> InternalList => (IList<T>)base.InnerCollection;

		public override void Add(T item) {
			var index = InternalList.BinarySearch(item);
			var insertionPoint = index >= 0 ? index : ~index;
			InternalList.Insert(insertionPoint, item);
		}

	}
}
