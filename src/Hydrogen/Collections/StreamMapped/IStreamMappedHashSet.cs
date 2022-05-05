using System.Collections.Generic;

namespace Hydrogen;

public interface IStreamMappedHashSet<TItem> : ISet<TItem> {
	IClusteredStorage Storage { get; }
}
