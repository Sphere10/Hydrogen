using System;
using System.Collections;
using Hydrogen.ObjectSpaces;

namespace Hydrogen;

public interface IStreamMappedCollection {
	ObjectStream ObjectStream { get; }
	
	void Clear();
}

public interface IStreamMappedCollection<TItem> : IStreamMappedCollection {
	new ObjectStream<TItem> ObjectStream { get; }

}