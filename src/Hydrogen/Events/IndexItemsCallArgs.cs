// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class IndexItemsCallArgs<T> : CallArgs {
	public IndexItemsCallArgs(int index, T item) : base(index, new[] { item }) {
	}

	public IndexItemsCallArgs(int index, IEnumerable<T> items) : base(index, items) {
	}

	public int Index {
		get => (int)base[0];
		set => base[0] = value;
	}

	public IEnumerable<T> Items {
		get => (IEnumerable<T>)base[1];
		set => base[1] = value;
	}
}
