namespace Hydrogen.Application;

public class Local<T> {

	public Local(T item) {
		Item = item;
	}

	public T Item { get; init; }
}
