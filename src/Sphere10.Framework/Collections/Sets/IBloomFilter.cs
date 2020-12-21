using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	
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
