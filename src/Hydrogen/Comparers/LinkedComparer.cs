// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen {

	/// <summary>
	/// Comparer to daisy-chain two existing comparers and 
	/// apply in sequence (i.e. sort by x then y)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class LinkedComparer<T> : IComparer<T> {
		private readonly IComparer<T> _primary, _secondary;

		/// <summary>
		/// Create a new LinkedComparer
		/// </summary>
		/// <param name="primary">The first comparison to use</param>
		/// <param name="secondary">The next level of comparison if the primary returns 0 (equivalent)</param>
		public LinkedComparer(IComparer<T> primary,IComparer<T> secondary) {
			_primary = primary ?? throw new ArgumentNullException(nameof(primary));
			_secondary = secondary ?? throw new ArgumentNullException(nameof(secondary));
		}

		int IComparer<T>.Compare(T x, T y) {
			var result = _primary.Compare(x, y);
			return result == 0 ? _secondary.Compare(x, y) : result;
		}
	}

}
