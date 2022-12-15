namespace Hydrogen.Application;

public interface INamedLookup<out T> {
	T this[string name] { get; }
}
