namespace Hydrogen {

	public interface IReadOnlyBoundedList<T> : IReadOnlyExtendedList<T> {
		int FirstIndex { get; }
	}

}