//-----------------------------------------------------------------------
// <copyright file="TimespanExtensions.cs" company="Sphere 10 Software">
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

namespace Hydrogen;
public static class TupleExtensions {

	public static T1 Unpack<T1, T2>(this Tuple<T1, T2> tuple, out T2 value)  {
		value = tuple.Item2;
		return tuple.Item1;
	}

	public static KeyValuePair<T1, T2> ToKeyValuePair<T1, T2>(this Tuple<T1, T2> tuple) => new(tuple.Item1, tuple.Item2);

	public static KeyValuePair<T1, T2> ToKeyValuePair<T1, T2>(this (T1, T2) tuple) => new(tuple.Item1, tuple.Item2);

}

