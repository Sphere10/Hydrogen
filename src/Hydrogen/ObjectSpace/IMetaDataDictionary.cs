using System;
using System.Collections.Generic;

namespace Hydrogen;

public interface IMetaDataDictionary<TKey> : IDisposable {
	IReadOnlyDictionary<TKey, long> Dictionary { get; }
}
