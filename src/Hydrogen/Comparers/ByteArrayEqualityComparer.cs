//-----------------------------------------------------------------------
// <copyright file="ByteArrayEqualityComparer.cs" company="Sphere 10 Software">
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
using System.Runtime.CompilerServices;

namespace Hydrogen {
	public class ByteArrayEqualityComparer : IEqualityComparer<byte[]> {
        public static readonly ByteArrayEqualityComparer Instance = new ByteArrayEqualityComparer();


        public bool Equals(byte[] x, byte[] y) {
	        return Equals(x.AsSpan(), y.AsSpan());
        }

        public static bool Equals(byte[] x, byte[] y, int length) {
	        return Equals(x.AsSpan(0, length), y.AsSpan(0, length));
        }

        public static bool Equals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) {
	        return x.SequenceEqual(y);
        }


        public int GetHashCode(byte[] obj) => obj.GetHashCodeSimple();

	}
}
