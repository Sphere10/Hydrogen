//-----------------------------------------------------------------------
// <copyright file="HashSetExtensions.cs" company="Sphere 10 Software">
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

using System.Collections.Generic;

namespace Sphere10.Framework {

	public static class HashSetExtensions {

		public static HashSet<T> AddRange<T>(this HashSet<T> source, IEnumerable<T> values) {
			values.ForEach(obj => source.Add(obj));
			return source;
		}
	}
}
