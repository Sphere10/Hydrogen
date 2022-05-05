//-----------------------------------------------------------------------
// <copyright file="UIDecimalField.cs" company="Sphere 10 Software">
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
	public partial class UIDecimalField : UITextField
	{
		public decimal Value {
				get { return UIDecimalField.GetAmountFromString(Text); }
				set { Text = value.ToString("N2"); }
		}
		
		public UIDecimalField (Decimal currentValue): base()
		{
			Value = currentValue;
			Initialize();
		}
		
		public UIDecimalField (IntPtr ptr) : base(ptr) {
			Initialize();
		}
		
		protected void Initialize() {
			KeyboardType = UIKeyboardType.NumberPad;
			Delegate = new UIDecimalFieldDelegate();
		}
			
		private class UIDecimalFieldDelegate : UITextFieldDelegate {
			public override bool ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString)
			{
				var newText = textField.Text.Remove((int)range.Location, (int)range.Length);
				newText = newText.Insert((int)range.Location, replacementString);
						
				if (newText.Length>0){
					textField.Text = (UIDecimalField.GetAmountFromString(newText)).ToString("N2");
					
					return false;
				}
				
				return false;
			}
		
		}
		
		private static decimal GetAmountFromString(string text){
			if (text.Length==0)
				return 0;
			
				var cleanedUpText = "";
			foreach (char c in text){
				if (Char.IsDigit(c)) cleanedUpText+=c;
			}
			return (decimal.Parse(cleanedUpText))/100;
		}
	}
}
