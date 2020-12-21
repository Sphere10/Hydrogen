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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework {
    public class ByteArrayEqualityComparer : IEqualityComparer<byte[]> {
        public static readonly ByteArrayEqualityComparer Instance = new ByteArrayEqualityComparer();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(byte[] x, byte[] y) {
			return EqualsImplementation(x, y);
        }

        public static bool EqualsImplementation(byte[] x, byte[] y) {
			var xIsNull = x == null;
			var yIsNull = y == null;
			if(xIsNull && yIsNull)
				return true;

			if (xIsNull || yIsNull)
				return false;

			if (x.Length != y.Length)
				return false;

			ReinterpretArray common = new ReinterpretArray();
			common.AsByteArray = x;
			ulong[] array1 = common.AsUInt64Array;
			common.AsByteArray = y;
			ulong[] array2 = common.AsUInt64Array;

			int length = x.Length;
			int remainder = length & 7;
			int len = length >> 3;

			int i = 0;

			while (i + 7 < len) {
				if (array1[i] != array2[i] ||
					array1[i + 1] != array2[i + 1] ||
					array1[i + 2] != array2[i + 2] ||
					array1[i + 3] != array2[i + 3] ||
					array1[i + 4] != array2[i + 4] ||
					array1[i + 5] != array2[i + 5] ||
					array1[i + 6] != array2[i + 6] ||
					array1[i + 7] != array2[i + 7])
					return false;

				i += 8;
			}

			if (i + 3 < len) {
				if (array1[i] != array2[i] ||
					array1[i + 1] != array2[i + 1] ||
					array1[i + 2] != array2[i + 2] ||
					array1[i + 3] != array2[i + 3])
					return false;

				i += 4;
			}

			if (i + 1 < len) {
				if (array1[i] != array2[i] ||
					array1[i + 1] != array2[i + 1])
					return false;

				i += 2;
			}

			if (i < len) {
				if (array1[i] != array2[i])
					return false;

				i += 1;
			}

			if (remainder > 0) {
				int shift = sizeof(ulong) - remainder;
				if ((array1[i] << shift) >> shift != (array2[i] << shift) >> shift)
					return false;
			}

			return true;
        }

        public int GetHashCode(byte[] obj) {
            return obj.GetHashCodeSimple();
        }
    }
}
