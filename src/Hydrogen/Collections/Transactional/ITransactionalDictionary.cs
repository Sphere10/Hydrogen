using System;
using System.Collections.Generic;

namespace Sphere10.Framework;

public interface ITransactionalDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ITransactionalFile, ILoadable, ISynchronizedObject, IDisposable {
}
