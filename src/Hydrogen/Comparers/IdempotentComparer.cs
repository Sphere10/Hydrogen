using System.Collections.Generic;

namespace Hydrogen;

public class IdempotentComparer<T> : IComparer<T> {
	public static readonly IdempotentComparer<T> Instance = new();

	private IdempotentComparer() {
	}

	public int Compare(T x, T y) {
		return 0;
	}
}
