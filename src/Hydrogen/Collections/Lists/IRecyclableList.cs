// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Hydrogen;

/// <summary>
/// A list whose indices are recycled when removing and re-adding items. Existing holes are re-used before increasing the size of the list.
/// 
/// - Items can be added dynamically, like in an array or linked list.
/// - Items can be removed, and their index or "slot" is marked as free.
/// - Subsequent additions will reuse these free slots before increasing the size of the data structure.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRecyclableList<T> : IExtendedList<T>, IRecyclableList {
	void Add(T item, out long index);

	void IRecyclableList.Add(object item, out long index) 
		=> Add((T) item, out index);

	void IRecyclableList.Update(long index, object item) 
		=> Update(index, (T)item);

}

public interface IRecyclableList  {
	long ListCount { get; }

	long RecycledCount { get; }

	void Recycle(long index);

	bool IsRecycled(long index);

	void Add(object item, out long index);

	void Update(long index, object item);
}