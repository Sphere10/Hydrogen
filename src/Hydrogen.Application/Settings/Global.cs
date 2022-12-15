namespace Hydrogen.Application;

public class Global<T> {

	public Global(T item) {
		Item = item;
	}

	public T Item { get; set; }
}
