using System;
using System.Collections.Generic;

namespace Hydrogen.ObjectSpace.MetaData;

public interface IMetaDataDictionary<TKey> : IDisposable {
	IReadOnlyDictionary<TKey, long> Dictionary { get; }
}
