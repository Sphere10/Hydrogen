using System.Linq;

namespace Hydrogen;

public interface IProjectionIndex<TKey> : IClusteredStreamsAttachment {
	ILookup<TKey, long> Values { get ; }
}
