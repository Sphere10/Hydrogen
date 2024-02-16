using Hydrogen.ObjectSpaces;

namespace Hydrogen;

public interface IStreamMappedCollection {
	ObjectContainer ObjectContainer { get; }
}

public interface IStreamMappedCollection<TItem> : IStreamMappedCollection {
	new ObjectContainer<TItem> ObjectContainer { get; }

}