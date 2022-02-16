using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework {
	public interface IStreamKeyValueStore<TKey, out THeader, TRecord> : IStreamPersistedList<KeyValuePair<TKey, byte[]>, THeader, TRecord>
		where THeader : IStreamStorageHeader
		where TRecord : IStreamKeyRecord {

		TKey ReadKey(int index);

		byte[] ReadValue(int index);

	}

}
