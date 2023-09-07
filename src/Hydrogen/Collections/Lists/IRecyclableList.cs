// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
public interface IRecyclableList<T> : IExtendedList<T> {
	long ListCount { get; }

	long RecycledCount { get; }

	void Recycle(long index);

	bool IsRecycled(long index);

}