using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamMappedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ILoadable {

		IClusteredStorage Storage { get; }

		internal object InternalList { get; }  // this is used to get the internal list container (parent of Storage)

		TKey ReadKey(int index);

		TValue ReadValue(int index);

		bool TryFindKey(TKey key, out int index);

		bool TryFindValue(TKey key, out int index, out TValue value);

		void RemoveAt(int index);

		

	}
	
}
