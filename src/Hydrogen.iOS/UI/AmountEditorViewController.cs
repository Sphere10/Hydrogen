//-----------------------------------------------------------------------
// <copyright file="AmountEditorViewController.cs" company="Sphere 10 Software">
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
using Foundation;
using UIKit;
using CoreGraphics;


namespace Hydrogen.iOS
{
	
	public partial class AmountEditorViewController : UIViewController
	{
		decimal _amount;
		string _title;
		
		public AmountEditorViewController (Decimal amount, string title) : base()
		{
			_amount = amount;
			_title = title;
		}
		
		private UIDecimalField _totalField;
		
		public override void ViewDidLoad ()
		{
			Title = _title;
			
			View.AddSubview(new UILabel {
				Frame = new CGRect(20,10,280,40),
				TextAlignment = UITextAlignment.Center,
				Font = (UIFont)(UIFont.SystemFontOfSize(20)),
				TextColor = UIColor.Gray,
				Text = "Type Amount"
			}); 
			
			_totalField = new UIDecimalField (_amount) {
				Frame = new CGRect(50,50,220,40),
				TextAlignment = UITextAlignment.Center,
				Font = (UIFont)(UIFont.BoldSystemFontOfSize(30)),
				Text = _amount.ToString("N2"),
				EnablesReturnKeyAutomatically = true,
				BorderStyle = UITextBorderStyle.RoundedRect,
			};
			
			View.AddSubview(_totalField);
			
			_totalField.BecomeFirstResponder();
		}
	}
}
