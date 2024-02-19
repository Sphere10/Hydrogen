using Hydrogen.ObjectSpaces;

namespace Hydrogen;

public interface IStreamMappedCollection {
	ObjectStream ObjectStream { get; }
}

public interface IStreamMappedCollection<TItem> : IStreamMappedCollection {
	new ObjectStream<TItem> ObjectStream { get; }

}