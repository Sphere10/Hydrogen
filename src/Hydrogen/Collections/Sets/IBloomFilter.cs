using System.Collections.Generic;

namespace Hydrogen {


	public interface IBloomFilter<in TItem> : IEnumerable<bool> {

		int HashRounds { get; }

		int FilterLength { get; }

		int Count { get; }

		decimal Error { get; }

		void Add(TItem item);

		void Clear();

		bool Contains(TItem item);

		void UnionWith(IEnumerable<TItem> other);

	}
}
