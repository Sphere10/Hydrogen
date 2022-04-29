//-----------------------------------------------------------------------
// <copyright file="NSComboBoxDelegate.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	public class NSComboBoxDelegateEx : NSComboBoxDelegate {

		public event EventHandler SelectionChanged;
		public event EventHandler SelectionChanging;
		public event EventHandler WillDissmiss;
		public event EventHandler WillPopUp;

		public sealed override void comboBoxSelectionDidChange(NSNotification notification) {
			if (SelectionChanged != null) {
				SelectionChanged(notification.Object, EventArgs.Empty);
			}
		}
		
		public sealed override void comboBoxSelectionIsChanging(NSNotification notification) {
			if (SelectionChanging != null) {
				SelectionChanging(notification.Object, EventArgs.Empty);
			}
		}
		
		public sealed override void comboBoxWillDismiss(NSNotification notification) {
			if (WillDissmiss != null) {
				WillDissmiss(notification.Object, EventArgs.Empty);
			}
		}
		
		public sealed override void comboBoxWillPopUp(NSNotification notification)	{
			if (WillPopUp != null) {
				WillPopUp(notification.Object, EventArgs.Empty);
			}
		}

	
	}
}

