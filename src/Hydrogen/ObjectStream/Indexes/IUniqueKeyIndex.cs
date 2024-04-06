using System.Collections.Generic;
using Hydrogen.Mapping;

namespace Hydrogen;

public interface IUniqueKeyIndex<TKey> {

	
	IReadOnlyDictionary<TKey, long> Dictionary { get ; }
}