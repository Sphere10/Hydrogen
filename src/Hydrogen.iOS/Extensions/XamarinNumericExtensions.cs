//-----------------------------------------------------------------------
// <copyright file="XamarinNumericExtensions.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS {
    public static class XamarinNumericExtensions {

        public static nfloat ClipTo(this nfloat value, nfloat min, nfloat max) {
            if (value < min) {
                return min;
            } else if (value > max) {
                return max;
            }
            return value;
        }


        public static nint ClipTo(this nint value, nint min, nint max) {
            if (value < min) {
                return min;
            } else if (value > max) {
                return max;
            }
            return value;
        }

        public static nuint ClipTo(this nuint value, nuint min, nuint max) {
            if (value < min) {
                return min;
            } else if (value > max) {
                return max;
            }
            return value;
        }


        public static void Repeat(this nint times, Action action) {
            for (var i = 0; i < times; i++) {
                action();
            }
        }

    }
}
