// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen {

	public interface IWriteOnlyExtendedList<in T> : IWriteOnlyExtendedCollection<T> {
		T this[int index] { set; }
		void Update(int index, T item);
		void UpdateRange(int index, IEnumerable<T> items);
		void Insert(int index, T item);
		void InsertRange(int index, IEnumerable<T> items);
		void RemoveAt(int index);
		void RemoveRange(int index, int count);
	}

}