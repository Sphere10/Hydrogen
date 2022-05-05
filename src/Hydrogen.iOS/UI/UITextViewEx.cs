//-----------------------------------------------------------------------
// <copyright file="UITextViewEx.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS
{
	public class UITextViewEx : UITextView	{
		public UITextViewEx (UIColor backgroundColor, float cornerRadius = 8.0f) {
			Initialize(backgroundColor, cornerRadius);
		}

		private void Initialize(UIColor backgroundColor, float cornerRadius) {
			this.BackgroundColor =backgroundColor;
			this.Layer.BorderWidth = 1.0f;
			this.Layer.BorderColor = UIColor.Gray.CGColor;
			this.Layer.CornerRadius = cornerRadius;
			this.Layer.ShadowRadius = cornerRadius;
			this.Layer.MasksToBounds = true;
			this.UserInteractionEnabled = true;
		}
	}
}

