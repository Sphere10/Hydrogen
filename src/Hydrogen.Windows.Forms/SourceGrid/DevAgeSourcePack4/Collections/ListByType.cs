// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace DevAge.Collections;

/// <summary>
/// A collection of object with a special method that returns an object compatible with a specified Type, GetByType(Type).
/// </summary>
public abstract class ListByType<T> : System.Collections.Generic.List<T> {
	/// <summary>
	/// Returns an object of the list that is compatible from the specified type. 
	/// The Type is compared using the IsAssignableFrom method. If there isn't a compatible object returns null.
	/// </summary>
	/// <param name="searchType"></param>
	/// <returns></returns>
	public T GetByType(Type searchType) {
		for (int i = 0; i < this.Count; i++) {
			if (searchType.IsAssignableFrom(this[i].GetType()))
				return this[i];
		}

		return default(T);
	}
}
