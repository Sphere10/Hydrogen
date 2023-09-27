using System;
using System.Collections.Generic;

namespace Hydrogen;

public class IdempotentEqualityComparer<T> : IEqualityComparer<T> {
	public static readonly IdempotentEqualityComparer<T> Instance = new();

	private IdempotentEqualityComparer() {
	}

	public bool Equals(T x, T y) => true;

	public int GetHashCode(T obj) => 0;

	public IEqualityComparer<T> By<TMember>(Func<T, TMember> member, IEqualityComparer<TMember> memberComparer = null) 
		=> new ProjectionEqualityComparer<T, TMember>(member, memberComparer);
	
}
