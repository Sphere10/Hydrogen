//-----------------------------------------------------------------------
// <copyright file="ActionMouseDelegate.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	public sealed class ActionMouseDelegate : MouseDelegate {
	
		public ActionMouseDelegate() {
		}

		public Action<NSEvent> MouseMovedAction { get; set; }
		public Action<NSEvent> MouseEnteredAction { get; set; }
		public Action<NSEvent> CursorUpdateAction { get; set; }
		public Action<NSEvent> MouseExitedAction { get; set; }

		public sealed override void MouseMoved(NSEvent theEvent) {
			if (MouseMovedAction != null)
				MouseMovedAction(theEvent);
		}


		public sealed override void MouseEntered (NSEvent theEvent)	{
			if (MouseEnteredAction != null) 
				MouseEnteredAction(theEvent);
		}
		
		public sealed override void CursorUpdate (NSEvent theEvent)	{
			if (CursorUpdateAction != null)
				CursorUpdateAction(theEvent);
		}
		
		public sealed override void MouseExited (NSEvent theEvent) {
			if (MouseExitedAction != null)
				MouseExitedAction(theEvent);
		}
	}
}

