//-----------------------------------------------------------------------
// <copyright file="IEnumerableExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Hydrogen;


public static class IAsyncEnumerableExtensions {

	public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable) {
		var list = new List<T>();
		await foreach(var item in asyncEnumerable)
			list.Add(item);
		return list;
	}

	public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> asyncEnumerable) 
		=> (await asyncEnumerable.ToListAsync()).ToArray();
}
