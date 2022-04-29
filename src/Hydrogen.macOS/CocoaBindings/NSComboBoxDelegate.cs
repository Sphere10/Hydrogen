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

namespace Sphere10.Framework {
	
	[Model, Register("NSComboBoxDelegate", true)]
	public class NSComboBoxDelegate : NSTextFieldDelegate  {
		
		[Export("comboBoxSelectionDidChange:")]
		public virtual void comboBoxSelectionDidChange(NSNotification notification)
		{
			throw new You_Should_Not_Call_base_In_This_Method();
		}
		
		[Export("comboBoxSelectionIsChanging:")]
		public virtual void comboBoxSelectionIsChanging(NSNotification notification)
		{
			throw new You_Should_Not_Call_base_In_This_Method();
		}
		
		[Export("comboBoxWillDismiss:")]
		public virtual void comboBoxWillDismiss(NSNotification notification)
		{
			throw new You_Should_Not_Call_base_In_This_Method();
		}
		
		
		[Export("comboBoxWillPopUp:")]
		public virtual void comboBoxWillPopUp(NSNotification notification)
		{
			throw new You_Should_Not_Call_base_In_This_Method();
		}
		
	}
	
	
}
