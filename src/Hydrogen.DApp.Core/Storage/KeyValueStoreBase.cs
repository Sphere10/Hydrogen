// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.Linq;

namespace Hydrogen.DApp.Core.Storage;

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
