using System;
using System.Collections.Generic;

namespace Hydrogen;

public interface ITransactionalDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ITransactionalFile, ILoadable, ISynchronizedObject, IDisposable {
}
