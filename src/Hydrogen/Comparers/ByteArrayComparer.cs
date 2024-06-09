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

public class ByteArrayComparer : IComparer<byte[]> {
	public static readonly ByteArrayComparer Instance = new ByteArrayComparer();

	public int Compare(byte[] x, byte[] y) {
		return Compare(x.AsSpan(), y.AsSpan());
	}

	public static int Compare(byte[] x, byte[] y, int length) {
		return Compare(x.AsSpan(0, length), y.AsSpan(0, length));
	}

	public static int Compare(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) {
		return x.SequenceCompareTo(y) switch {
			> 0 => 1,
			< 0 => -1,
			_ => 0
		};
	}

}
