// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen {

	public interface IWriteOnlyExtendedCollection<in T> {
		void Add(T item);
		void AddRange(IEnumerable<T> items);
		bool Remove(T item);
		IEnumerable<bool> RemoveRange(IEnumerable<T> items);
		void Clear();
	}

	public static class IWriteOnlyExtendedCollectionExtensions {

		public static void AddRange<T>(this IWriteOnlyExtendedCollection<T> collection, params T[] items) {
			collection.AddRange((IEnumerable<T>)items);
		}
	}
}