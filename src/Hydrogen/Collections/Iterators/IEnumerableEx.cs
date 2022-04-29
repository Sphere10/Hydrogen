using System.Collections.Generic;

namespace Hydrogen {

	/// <summary>
	/// Strongly-typed poly enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEnumerableEx<out T> : IEnumerable<T>, IReadOnlyCollection<T> {
	}

}
