//-----------------------------------------------------------------------
// <copyright file="ActionEqualityComparer.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	public class CastedEqualityComparer<TItem, TBase> : IEqualityComparer<TBase> where TItem : TBase {
		private readonly IEqualityComparer<TItem> _comparer;

		public CastedEqualityComparer(IEqualityComparer<TItem> equalityComparer) {
			Guard.ArgumentNotNull(equalityComparer, nameof(equalityComparer));
			_comparer = equalityComparer;
		}

		public bool Equals(TBase? x, TBase? y)
			=> _comparer.Equals((TItem)x, (TItem)y);

		public int GetHashCode(TBase obj)
			=> _comparer.GetHashCode((TItem)obj);
	}
}
