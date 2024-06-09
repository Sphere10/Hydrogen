// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public static class TupleExtensions {

	public static T1 Unpack<T1, T2>(this Tuple<T1, T2> tuple, out T2 value) {
		value = tuple.Item2;
		return tuple.Item1;
	}

	public static KeyValuePair<T1, T2> ToKeyValuePair<T1, T2>(this Tuple<T1, T2> tuple) => new(tuple.Item1, tuple.Item2);

	public static KeyValuePair<T1, T2> ToKeyValuePair<T1, T2>(this (T1, T2) tuple) => new(tuple.Item1, tuple.Item2);

}
