using System.Collections.Generic;
using Hydrogen.Mapping;

namespace Hydrogen;

public interface IUniqueProjectionIndex<TKey> : IClusteredStreamsAttachment {

	IReadOnlyDictionary<TKey, long> Values { get ; }
}