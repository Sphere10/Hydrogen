// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
