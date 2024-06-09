// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrogen;

internal class ExtendedListHelper {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long IndexOfSequenceImpl<T>(IExtendedList<T> extendedList, IEnumerable<T> items) {
		var indices = extendedList.IndexOfRange(items).ToArray();
		if (indices.Length == 0)
			return -1;
		return Tools.Collection.RangeL(indices[0], indices.Length).SequenceEqual(indices) ? indices[0] : -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int RemoveRangeImpl<T>(IExtendedList<T> extendedList, IEnumerable<T> items) {
		return
			extendedList
				.IndexOfRange(items)
				.Where(index => index >= 0)
				.Apply(extendedList.RemoveAt)
				.Count();
	}

}
