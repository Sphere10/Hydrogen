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
public class ProjectionEqualityComparer<TSource, TKey> : IEqualityComparer<TSource> {
	readonly Func<TSource, TKey> _projection;
	readonly IEqualityComparer<TKey> _comparer;

	/// <summary>
	/// Creates a new instance using the specified projection, which must not be null.
	/// The default comparer for the projected type is used.
	/// </summary>
	/// <param name="projection">Projection to use during comparisons</param>
	public ProjectionEqualityComparer(Func<TSource, TKey> projection)
		: this(projection, null) {
	}

	/// <summary>
	/// Creates a new instance using the specified projection, which must not be null.
	/// </summary>
	/// <param name="projection">Projection to use during comparisons</param>
	/// <param name="comparer">The comparer to use on the keys. May be null, in
	/// which case the default comparer will be used.</param>
	public ProjectionEqualityComparer(Func<TSource, TKey> projection, IEqualityComparer<TKey> comparer) {
		this._comparer = comparer ?? EqualityComparer<TKey>.Default;
		this._projection = projection ?? throw new ArgumentNullException(nameof(projection));
	}

	/// <summary>
	/// Compares the two specified values for equality by applying the projection
	/// to each value and then using the equality comparer on the resulting keys. Null
	/// references are never passed to the projection.
	/// </summary>
	public bool Equals(TSource x, TSource y) {
		if (x == null && y == null) {
			return true;
		}
		if (x == null || y == null) {
			return false;
		}
		return _comparer.Equals(_projection(x), _projection(y));
	}

	/// <summary>
	/// Produces a hash code for the given value by projecting it and
	/// then asking the equality comparer to find the hash code of
	/// the resulting key.
	/// </summary>
	public int GetHashCode(TSource obj) {
		if (obj == null) {
			throw new ArgumentNullException("obj");
		}
		return _comparer.GetHashCode(_projection(obj));
	}
}
