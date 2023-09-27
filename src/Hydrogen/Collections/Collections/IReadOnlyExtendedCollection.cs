// Copyright (c) Sphere 10 Software, 2005+. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public interface IReadOnlyExtendedCollection<T> : IReadOnlyCollection<T> {
	bool Contains(T item);

	IEnumerable<bool> ContainsRange(IEnumerable<T> items);

	void CopyTo(T[] array, int arrayIndex);
}
