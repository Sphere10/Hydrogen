namespace Sphere10.Framework {

	interface INullOp<T> {
		bool HasValue(T value);
		bool AddIfNotNull(ref T accumulator, T value);
	}

}