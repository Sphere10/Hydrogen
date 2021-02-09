using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Core.Storage {

    public interface IKeyValueStore<T> : IReadWriteSafeObject {
        IQueryable<T> GetKeys();
        Stream OpenRead(T key);
        Stream OpenWrite(T key);
    }
}
