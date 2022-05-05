//-----------------------------------------------------------------------
// <copyright file="NSDateExtensions.cs" company="Sphere 10 Software">
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
#if MONOTOUCH
using MonoTouch.Foundation;
#else
using MonoMac.Foundation;
#endif

namespace Hydrogen {
	public static class DateExtensions
	{

		public static NSDate ToNSDate(this DateTime dateTime) {
			return NSDate.FromTimeIntervalSinceReferenceDate((dateTime.ToUniversalTime()-(new DateTime(2001,1,1,0,0,0))).TotalSeconds);
		}
		
		public static DateTime ToDateTime(this NSDate nsDate) {
			return (new DateTime(2001,1,1,0,0,0)).AddSeconds(nsDate.SecondsSinceReferenceDate);
		}

	}
}

