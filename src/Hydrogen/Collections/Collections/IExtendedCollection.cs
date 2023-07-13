// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen; 

public interface IExtendedCollection<T> : ICollection<T>, IReadOnlyExtendedCollection<T>, IWriteOnlyExtendedCollection<T> {
	new long Count { get; }
	new void Add(T item);
	new void Clear();
	new bool Contains(T item);
	new void CopyTo(T[] array, int arrayIndex);
	new bool Remove(T item);
}