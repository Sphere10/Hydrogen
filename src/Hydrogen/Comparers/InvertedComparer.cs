// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Implementation of IComparer{T} based on another one;
/// this simply reverses the original comparison.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class InvertedComparer<T> : IComparer<T> {

	/// <summary>
	/// Returns the original comparer; this can be useful to avoid multiple
	/// reversals.
	/// </summary>
	public IComparer<T> OriginalComparer { get; }

	/// <summary>
	/// Creates a new reversing comparer.
	/// </summary>
	/// <param name="original">The original comparer to use for comparisons.</param>
	public InvertedComparer(IComparer<T> original) {
		Guard.ArgumentNotNull(original, nameof(original));
		OriginalComparer = original;
	}

	/// <summary>
	/// Returns the result of comparing the specified values using the original
	/// comparer, but reversing the order of comparison.
	/// </summary>
	public int Compare(T x, T y) => OriginalComparer.Compare(y, x);
}
