namespace Hydrogen;

public static class EqualityComparerBuilder {
	public static IdempotentEqualityComparer<T> For<T>() => IdempotentEqualityComparer<T>.Instance;
}
