using System.Collections.Generic;

namespace Hydrogen;

public interface IUniqueProjectionIndex<TKey> : IClusteredStreamsAttachment {

	IReadOnlyDictionary<TKey, long> Values { get ; }
}