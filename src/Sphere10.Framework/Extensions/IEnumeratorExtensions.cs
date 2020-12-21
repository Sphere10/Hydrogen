using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public static class IEnumeratorExtensions {

		public static IEnumerator<T> OnMoveNext<T>(this IEnumerator<T> enumerator, Action action) {
			return new OnMoveNextEnumerator<T>(enumerator, action);
		}

		
		public static IEnumerator<T> OnDispose<T>(this IEnumerator<T> enumerator, Action action) {
			return new OnDisposeEnumerator<T>(enumerator, action);
		}

		public static IEnumerator<T> WithMemory<T>(this IEnumerator<T> enumerator) {
			return new MemorizingEnumerator<T>(enumerator);
		}

		public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator) {
			while (enumerator.MoveNext())
				yield return enumerator.Current;
		}

	}

}