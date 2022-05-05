//-----------------------------------------------------------------------
// <copyright file="MouseDelegate.cs" company="Sphere 10 Software">
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
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Drawing;

namespace Hydrogen {

	public class MouseDelegate : NSObject {

		public NSView View { get; set; }
		
		[Export("mouseMoved:")]
		public virtual void MouseMoved (NSEvent theEvent) {
		}

		[Export("mouseEntered:")]
		public virtual void MouseEntered (NSEvent theEvent)	{
		}
		
		[Export("cursorUpdate::")]
		public virtual void CursorUpdate (NSEvent theEvent)	{
		}
		
		[Export("mouseExited:")]
		public virtual void MouseExited (NSEvent theEvent) {
		}
	}
}

