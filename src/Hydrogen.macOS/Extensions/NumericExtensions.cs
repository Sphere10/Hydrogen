//-----------------------------------------------------------------------
// <copyright file="NumericExtensions.cs" company="Sphere 10 Software">
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
using MonoMac.Foundation;

namespace Hydrogen
{
	public static class NumericExtensions
	{
		public static NSNumber ToNSNumber(this decimal value) {
			return NSNumber.FromDouble((double)value);
		}

		public static NSNumber ToNSNumber(this double value) {
			return NSNumber.FromDouble(value);
		}

		public static NSNumber ToNSNumber(this float value) {
			return NSNumber.FromFloat(value);
		}

		public static NSNumber ToNSNumber(this int value) {
			return NSNumber.FromUInt32((uint)value);
		}

	}
}
