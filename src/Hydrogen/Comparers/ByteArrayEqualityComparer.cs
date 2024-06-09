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

public class ByteArrayEqualityComparer : IEqualityComparer<byte[]> {
	public static readonly ByteArrayEqualityComparer Instance = new();

	public bool Equals(byte[] x, byte[] y) 
		=> x is null && y is null || 
		x is not null && y is not null &&  Equals(x.AsSpan(), y.AsSpan());

	public static bool Equals(byte[] x, byte[] y, int length)
		=> x is null && y is null || 
		   x is not null && y is not null && Equals(x.AsSpan(0, length), y.AsSpan(0, length));

	public static bool Equals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) => x.SequenceEqual(y);

	public int GetHashCode(byte[] obj) => obj.GetHashCodeSimple();

}
