//-----------------------------------------------------------------------
// <copyright file="UIDatePickerExtensions.cs" company="Sphere 10 Software">
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
using UIKit;
using CoreGraphics;
using CoreGraphics;
using Foundation;
using Hydrogen;

namespace Hydrogen.iOS
{
	public static class UIDatePickerExtensions {
		
		public static DateTime GetSafeDate(this UIDatePicker datePicker) {
			return DateTime.SpecifyKind( datePicker.Date.ToDateTime().ToLocalTime(), DateTimeKind.Unspecified);

		}

		public static void SetSafeDate(this UIDatePicker datePicker, DateTime dateTime, bool animated = false) {
			datePicker.SetDate(DateTime.SpecifyKind(dateTime, DateTimeKind.Local));
		}

		public static void SetDate(this UIDatePicker datePicker, DateTime dateTime, bool animated = false) {
			datePicker.SetDate(dateTime.ToNSDate(), animated);
		}

	}
}

