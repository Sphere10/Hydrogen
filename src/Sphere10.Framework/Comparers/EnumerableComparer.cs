//-----------------------------------------------------------------------
// <copyright file="EnumerableComparer.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Misc {
	/// <summary>
	/// Compares two sequences.
	/// </summary>
	/// <typeparam name="T">Type of item in the sequences.</typeparam>
	public class EnumerableComparer<T> : IComparer<IEnumerable<T>> {
		private readonly IComparer<T> _elementComparer;

		public EnumerableComparer() {
            _elementComparer = Comparer<T>.Default;
        }

        public EnumerableComparer(IComparer<T> comparer) {
            _elementComparer = comparer;
        }

        public int Compare(IEnumerable<T> x, IEnumerable<T> y) {
	        using IEnumerator<T> leftIt = x.GetEnumerator();
	        using IEnumerator<T> rightIt = y.GetEnumerator();
	        while (true) {
		        bool left = leftIt.MoveNext();
		        bool right = rightIt.MoveNext();

		        if (!(left || right)) return 0;

		        if (!left) return -1;
		        if (!right) return 1;

		        int itemResult = _elementComparer.Compare(leftIt.Current, rightIt.Current);
		        if (itemResult != 0) return itemResult;
	        }

        }
    }
}
