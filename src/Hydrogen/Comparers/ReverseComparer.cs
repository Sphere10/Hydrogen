using System;
using System.Collections.Generic;

namespace Hydrogen {
	/// <summary>
	/// Implementation of IComparer{T} based on another one;
	/// this simply reverses the original comparison.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ReverseComparer<T> : IComparer<T> {
		readonly IComparer<T> _comparer;

		/// <summary>
		/// Returns the original comparer; this can be useful to avoid multiple
		/// reversals.
		/// </summary>
		public IComparer<T> OriginalComparer => _comparer;

		/// <summary>
		/// Creates a new reversing comparer.
		/// </summary>
		/// <param name="original">The original comparer to use for comparisons.</param>
		public ReverseComparer(IComparer<T> original) {
			Guard.ArgumentNotNull(original, nameof(original));
			_comparer = original;
		}

		/// <summary>
		/// Returns the result of comparing the specified values using the original
		/// comparer, but reversing the order of comparison.
		/// </summary>
		public int Compare(T x, T y) => _comparer.Compare(y, x);
	}
}

