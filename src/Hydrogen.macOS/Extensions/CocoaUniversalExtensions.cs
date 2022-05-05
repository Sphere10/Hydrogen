//-----------------------------------------------------------------------
// <copyright file="CocoaUniversalExtensions.cs" company="Sphere 10 Software">
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
	public static class CocoaUniversalExtensions {

		public static NSObject ToNSObject(this object obj) {
			NSObject retval = null;
			if (obj is NSObject) {
				retval = obj as NSObject;
			} else {
				if (obj is string) {
					retval = ((string)obj).ToNSString();
				} else if (obj is Int16 || obj is Int32 || obj is Int64) {
					retval = new NSNumber((long)obj);
				} else if (obj is UInt16 || obj is UInt32 || obj is UInt64) {
					retval = new NSNumber((ulong)obj);
				} else if (obj is Single || obj is Double || obj is Decimal) {
					retval = new NSNumber((double)obj);
				} else {
					retval = obj.ToString().ToNSString();
				}
			}
			return retval;
		}

		public static object FromNSObject<T>(this NSObject obj) {
			object retval = default(T);
			if (obj is NSString) {
				retval = ((NSString)obj).ToString();
			} else if (obj is NSNumber) {
				retval = (T)(object)((NSNumber)obj).DoubleValue;
			}
			return retval;
		}

	}
}

