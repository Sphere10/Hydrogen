// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class IndexItemsCallArgs<T> : CallArgs {
	public IndexItemsCallArgs(long index, T item) : base(index, new[] { item }) {
	}

	public IndexItemsCallArgs(long index, IEnumerable<T> items) : base(index, items) {
	}

	public long Index {
		get => (int)base[0];
		set => base[0] = value;
	}

	public IEnumerable<T> Items {
		get => (IEnumerable<T>)base[1];
		set => base[1] = value;
	}
}
