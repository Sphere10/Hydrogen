// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public interface IWriteOnlyExtendedList<in T> : IWriteOnlyExtendedCollection<T> {
	T this[int index] { set; }
	T this[long index] { set; }

	void Update(long index, T item);

	void UpdateRange(long index, IEnumerable<T> items);

	void Insert(long index, T item);

	void InsertRange(long index, IEnumerable<T> items);

	void RemoveAt(long index);

	void RemoveRange(long index, long count);
}
