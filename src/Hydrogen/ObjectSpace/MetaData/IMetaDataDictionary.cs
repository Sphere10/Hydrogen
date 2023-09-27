using System;
using System.Collections.Generic;

namespace Hydrogen;

public interface IMetaDataDictionary<TKey> : IObjectContainerMetaDataProvider {
	IReadOnlyDictionary<TKey, long> Dictionary { get; }
}
