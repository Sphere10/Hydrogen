using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hydrogen;

namespace Hydrogen.DApp.Core.Storage {

    public interface IKeyValueStore<T> : ISynchronizedObject {
        IQueryable<T> GetKeys();
        Stream OpenRead(T key);
        Stream OpenWrite(T key);
    }
}
