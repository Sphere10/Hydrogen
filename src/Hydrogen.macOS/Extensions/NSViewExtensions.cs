//-----------------------------------------------------------------------
// <copyright file="NSViewExtensions.cs" company="Sphere 10 Software">
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
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.ObjCRuntime;


namespace Hydrogen {
	public static class NSViewExtensions {

		private static IntPtr selConvertRectToBacking_ = Selector.GetHandle("convertRectToBacking:");

		[Export("convertRectToBacking:")]
		public static RectangleF ConvertRectToBacking(this NSView view, RectangleF aRect)
		{
			RectangleF result;
			if (view.GetPrivateFieldValue<bool>("IsDirectBinding")) {
				Messaging.RectangleF_objc_msgSend_stret_RectangleF(out result, view.Handle, NSViewExtensions.selConvertRectToBacking_, aRect);
			}
			else
			{
				Messaging.RectangleF_objc_msgSendSuper_stret_RectangleF(out result, view.SuperHandle, NSViewExtensions.selConvertRectToBacking_, aRect);
			}
			return result;
		}
	}
}

