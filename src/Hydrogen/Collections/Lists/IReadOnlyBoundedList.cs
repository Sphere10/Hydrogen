namespace Sphere10.Framework {

	public interface IReadOnlyBoundedList<T> : IReadOnlyExtendedList<T> {
		int FirstIndex { get; }
	}

}