﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Extensions to IComparer
/// </summary>
public static class IComparerExtensions {

	/// <summary>
	/// Combines a comparer with a second comparer to implement composite sort
	/// behaviour.
	/// </summary>
	public static IComparer<T> ThenBy<T>(this IComparer<T> firstComparer, IComparer<T> secondComparer) {
		return new CompositeComparer<T>(firstComparer, secondComparer);
	}

	/// <summary>
	/// Combines a comparer with a projection to implement composite sort behaviour.
	/// </summary>
	public static IComparer<T> ThenBy<T, TKey>(this IComparer<T> firstComparer, Func<T, TKey> projection) {
		return new CompositeComparer<T>(firstComparer, new ProjectionComparer<T, TKey>(projection));
	}
}
