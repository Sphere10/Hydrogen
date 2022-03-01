using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IClusteredDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
		IClusteredStorage Storage { get; }
	}


}
