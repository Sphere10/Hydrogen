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

#if !NETSTANDARD2_1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework {
    public class ByteArrayComparer : IComparer<byte[]> {
        public static readonly ByteArrayComparer Instance = new ByteArrayComparer();

        public int Compare(byte[] x, byte[] y, int length) {
            if (x == null && y == null && length == 0)
                return 0;

            ReinterpretArray common = new ReinterpretArray();
            common.AsByteArray = x;
            ulong[] array1 = common.AsUInt64Array;
            common.AsByteArray = y;
            ulong[] array2 = common.AsUInt64Array;

            int len = length >> 3;
            int remainder = length & 7;

            int i = len;

            if (remainder > 0) {
                int shift = sizeof(ulong) - remainder;
                var v1 = (array1[i] << shift) >> shift;
                var v2 = (array2[i] << shift) >> shift;
                if (v1 < v2)
                    return -1;
                if (v1 > v2)
                    return 1;
            }

            i--;

            while (i >= 0) {
                var v1 = array1[i];
                var v2 = array2[i];
                if (v1 < v2)
                    return -1;
                if (v1 > v2)
                    return 1;

                i--;
            }

            return 0;
        }

        public int Compare(byte[] x, byte[] y) {
            if (x == null && y == null)
                return 0;

            if (x.Length == y.Length)
                return Compare(x, y, x.Length);

            for (int i = x.Length - 1, j = y.Length - 1, len = Math.Min(x.Length, y.Length); len > 0; i--, j--, len--) {
                if (x[i] < y[j])
                    return -1;
                if (x[i] > y[j])
                    return 1;
            }

            if (x.Length < y.Length)
                return -1;
            if (y.Length > y.Length)
                return 1;

            return 0;
        }
    }
}

#endif