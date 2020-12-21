using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework {

	/// <summary>
	/// Comparer to daisy-chain two existing comparers and 
	/// apply in sequence (i.e. sort by x then y)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class LinkedComparer<T> : IComparer<T> {
		private readonly IComparer<T> primary, secondary;

		/// <summary>
		/// Create a new LinkedComparer
		/// </summary>
		/// <param name="primary">The first comparison to use</param>
		/// <param name="secondary">The next level of comparison if the primary returns 0 (equivalent)</param>
		public LinkedComparer(IComparer<T> primary,IComparer<T> secondary) {
			this.primary = primary ?? throw new ArgumentNullException(nameof(primary));
			this.secondary = secondary ?? throw new ArgumentNullException(nameof(secondary));
		}

		int IComparer<T>.Compare(T x, T y) {
			var result = primary.Compare(x, y);
			return result == 0 ? secondary.Compare(x, y) : result;
		}
	}

}
