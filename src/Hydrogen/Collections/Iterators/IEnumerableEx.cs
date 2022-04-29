using System.Collections.Generic;

namespace Sphere10.Framework {

	/// <summary>
	/// Strongly-typed poly enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEnumerableEx<out T> : IEnumerable<T>, IReadOnlyCollection<T> {
	}

}
