using System.Collections.Generic;

namespace Sphere10.Framework;

public interface IClusteredHashSet<TItem> : ISet<TItem> {
	IClusteredStorage Storage { get; }
}
