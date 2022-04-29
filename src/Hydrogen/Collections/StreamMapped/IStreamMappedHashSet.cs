using System.Collections.Generic;

namespace Sphere10.Framework;

public interface IStreamMappedHashSet<TItem> : ISet<TItem> {
	IClusteredStorage Storage { get; }
}
