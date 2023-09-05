using System;
using System.Linq;

namespace Hydrogen.ObjectSpace.MetaData;

public interface IMetaDataLookup<TKey> : IDisposable {
	ILookup<TKey, long> Lookup { get; }
}