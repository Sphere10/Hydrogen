﻿using System.IO;
using System.Linq;
using System.Threading;
using Hydrogen;

namespace Hydrogen.DApp.Core.Storage {

    public abstract class KeyValueStoreBase<T> : SynchronizedResource, IKeyValueStore<T> {

        public Stream OpenRead(T key) {
            using (EnterReadScope()) {
                return OpenReadInternal(key);
            }
        }


        public IQueryable<T> GetKeys() {
            using (EnterReadScope()) {
                return GetKeysInternal();
            }
        }


        public Stream OpenWrite(T key) {
            using (EnterWriteScope()) {
                return OpenWriteInternal(key);
            }
        }

        protected abstract Stream OpenReadInternal(T key);

        protected abstract IQueryable<T> GetKeysInternal();

        protected abstract Stream OpenWriteInternal(T key);

    }
}