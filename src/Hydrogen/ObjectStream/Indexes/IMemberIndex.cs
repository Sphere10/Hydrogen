using System.Linq;

namespace Hydrogen;

public interface IMemberIndex<TKey> {
	ILookup<TKey, long> Lookup { get ; }
}
