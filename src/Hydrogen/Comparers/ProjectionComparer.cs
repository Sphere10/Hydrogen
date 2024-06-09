// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;


/// <summary>
/// Comparer which projects each element of the comparison to a key, and then compares
/// those keys using the specified (or default) comparer for the key type.
/// </summary>
/// <typeparam name="TSource">Type of elements which this comparer will be asked to compare</typeparam>
/// <typeparam name="TKey">Type of the key projected from the element</typeparam>
public class ProjectionComparer<TSource, TKey> : IComparer<TSource> {
	private readonly IComparer<TKey> _comparer;
	private readonly Func<TSource, TKey> _projection;
	
	/// <summary>
	/// Creates a new instance using the specified projection, which must not be null.
	/// The default comparer for the projected type is used.
	/// </summary>
	/// <param name="projection">Projection to use during comparisons</param>
	public ProjectionComparer(Func<TSource, TKey> projection)
		: this(projection, null) {
	}

	/// <summary>
	/// Creates a new instance using the specified projection, which must not be null.
	/// </summary>
	/// <param name="projection">Projection to use during comparisons</param>
	/// <param name="comparer">The comparer to use on the keys. May be null, in
	/// which case the default comparer will be used.</param>
	public ProjectionComparer(Func<TSource, TKey> projection, IComparer<TKey> comparer) {
		Guard.ArgumentNotNull(projection, nameof(projection));
		_comparer = comparer ?? Comparer<TKey>.Default;
		_projection = projection;
	}

	/// <summary>
	/// Compares x and y by projecting them to keys and then comparing the keys. 
	/// Null values are not projected; they obey the
	/// standard comparer contract such that two null values are equal; any null value is
	/// less than any non-null value.
	/// </summary>
	public int Compare(TSource x, TSource y) {
		// Don't want to project from nullity
		if (x == null && y == null) {
			return 0;
		}

		if (x == null) {
			return -1;
		}

		if (y == null) {
			return 1;
		}
		return _comparer.Compare(_projection(x), _projection(y));
	}
}
