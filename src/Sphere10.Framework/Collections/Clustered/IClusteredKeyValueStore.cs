using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Collections.Stream {
	public interface IClusteredKeyValueStore<TKey> : IClusteredList<KeyValuePair<TKey, byte[]>> {

		TKey ReadKey(int index);

		byte[] ReadValue(int index);

	}

}
