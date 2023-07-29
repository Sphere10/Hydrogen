using System;
using System.Collections.Generic;

namespace Hydrogen;

public class IdempotentComparer<T> : IComparer<T> {
	public static readonly IdempotentComparer<T> Instance = new();

	private IdempotentComparer() {
	}

	public int Compare(T x, T y) {
		return 0;
	}

	public IComparer<T> StartWith<TMember>(Func<T, TMember> member, IComparer<TMember> memberComparer = null) 
		=> new ProjectionComparer<T, TMember>(member, memberComparer);

}
