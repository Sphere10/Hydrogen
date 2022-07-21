using System;

namespace Hydrogen {

	/// <summary>
	/// Class representing a value which will be available some time in the future.
	/// </summary>
	public interface IFuture<out T> {
		/// <summary>
		/// Retrieves the value, if available, and throws InvalidOperationException
		/// otherwise.
		/// </summary>
		T Value { get; }
	}

	public static class IFutureExtensions {
		public static IFuture<TProjection> AsProjection<T, TProjection>(this IFuture<T> future, Func<T, TProjection> projection) 
			=> Tools.Values.Future.Projection(future, projection);
	}
}
