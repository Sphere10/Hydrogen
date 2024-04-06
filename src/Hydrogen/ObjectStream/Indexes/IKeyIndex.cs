using System.Linq;

namespace Hydrogen;

public interface IKeyIndex<TKey> {
	ILookup<TKey, long> Lookup { get ; }
}
