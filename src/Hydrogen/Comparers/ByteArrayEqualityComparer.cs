// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class ByteArrayEqualityComparer : IEqualityComparer<byte[]> {
	public static readonly ByteArrayEqualityComparer Instance = new();

	public bool Equals(byte[] x, byte[] y) => Equals(x.AsSpan(), y.AsSpan());

	public static bool Equals(byte[] x, byte[] y, int length) => Equals(x.AsSpan(0, length), y.AsSpan(0, length));

	public static bool Equals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) => x.SequenceEqual(y);

	public int GetHashCode(byte[] obj) => obj.GetHashCodeSimple();

}
