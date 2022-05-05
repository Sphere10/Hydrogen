//-----------------------------------------------------------------------
// <copyright file="BadgeView.cs" company="Sphere 10 Software">
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
using CoreGraphics;
using UIKit;

namespace Hydrogen.iOS {
	public class BadgeView : UILabel {
		private const float InternalPadding = 10.0f;
		public const float DefaultHeight = 15.0f;
		public const float DefaultFontSize = 12.0f;
		public const float DefaultBorderWidth = 2.0f;
		public static UIColor DefaultBackgroundColor = UIColor.Blue;
		public static UIColor DefaultForegroundColor = UIColor.White;
		public static UIColor DefaultBorderColor = UIColor.White;
		private float _height;
		private int _badgeNumber;
		private CGSize _numberSize;

		public BadgeView() : this(string.Empty) {
		}

		public BadgeView(int badgeNumber) : this(DefaultBackgroundColor, DefaultForegroundColor, DefaultBorderColor, DefaultHeight, DefaultFontSize, DefaultBorderWidth) {
			_badgeNumber = badgeNumber;
		}


		public BadgeView(string text) : this(DefaultBackgroundColor, DefaultForegroundColor, DefaultBorderColor, DefaultHeight, DefaultFontSize, DefaultBorderWidth) {
			Text = text;
		}

		public BadgeView(UIColor backgroundColor, UIColor textColor, UIColor borderColor, float height = DefaultHeight, float fontSize = DefaultFontSize, float borderWidth = DefaultBorderWidth) {
			_height = height;
			Font = (UIFont)(UIFont.BoldSystemFontOfSize(fontSize));
			BackgroundColor = backgroundColor;
			TextColor = textColor;
			UserInteractionEnabled = false;
			Layer.CornerRadius = _height/2;
			Layer.MasksToBounds = true;
			Layer.BorderWidth = borderWidth;
			Layer.BorderColor = borderColor.CGColor;
			TextAlignment = UITextAlignment.Center;
		}

		public int BadgeNumber {
			get { return _badgeNumber; }
			set { Text = (_badgeNumber = value).ToString(); }
		}

		public override string Text {
			get { return base.Text; }
			set
			{
				base.Text = value;
				CalculateSize();
				Alpha = (string.IsNullOrEmpty(value) || value == "0") ? 0 : 1;
				SetNeedsDisplay();
			}
		}

		void CalculateSize() {
            _numberSize = UIKit.UIStringDrawing.StringSize(Text, Font);
			_numberSize.Width += InternalPadding;
			Frame = new CGRect(Frame.Location, new CGSize((nfloat)Math.Max(_numberSize.Width, _height), _height));
		}
	}
}

