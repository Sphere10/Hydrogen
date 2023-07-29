namespace Hydrogen;

public static class ComparerBuilder {
	public static IdempotentComparer<T> For<T>() => IdempotentComparer<T>.Instance;
}