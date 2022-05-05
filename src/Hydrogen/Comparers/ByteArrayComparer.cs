//-----------------------------------------------------------------------
// <copyright file="ByteArrayComparer.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

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
}