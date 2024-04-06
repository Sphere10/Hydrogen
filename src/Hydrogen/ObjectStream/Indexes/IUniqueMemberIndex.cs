using System.Collections.Generic;
using Hydrogen.Mapping;

namespace Hydrogen;

public interface IUniqueMemberIndex<TKey> {

	
	IReadOnlyDictionary<TKey, long> Dictionary { get ; }
}