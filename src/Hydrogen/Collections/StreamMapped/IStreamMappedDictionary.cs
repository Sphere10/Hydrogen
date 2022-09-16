using System.Collections.Generic;


namespace Hydrogen {

	public interface IStreamMappedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ILoadable {

		IClusteredStorage Storage { get; }

		TKey ReadKey(int index);

		TValue ReadValue(int index);

		bool TryFindKey(TKey key, out int index);

		bool TryFindValue(TKey key, out int index, out TValue value);

		void RemoveAt(int index);

	}
	
}
