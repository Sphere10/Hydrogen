//-----------------------------------------------------------------------
// <copyright file="UIScrollViewEx.cs" company="Sphere 10 Software">
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
using Hydrogen;

namespace Hydrogen.iOS
{
	public class UIScrollViewEx : UIScrollView
	{
		public UIScrollViewEx (CGRect frame) : this(frame, UIColor.White, UIColor.LightGray) {
		}

		public UIScrollViewEx (CGRect frame, UIColor startColor, UIColor endColor) : base(frame) {
			GradientStartColor = startColor;
			GradientEndColor = endColor;
		}

		public UIColor GradientStartColor { get; set; }

		public UIColor GradientEndColor { get; set; }

		public override void Draw (CGRect rect)	{
			var currentContext = UIGraphics.GetCurrentContext();
			var locations = new nfloat[] { 0.0f, 1.0f};
			var colors = new[] { GradientStartColor.CGColor, GradientEndColor.CGColor };
			var rgbColorspace = CGColorSpace.CreateDeviceRGB();
			var glossGradient = new CGGradient(rgbColorspace, colors, locations);
			currentContext.DrawLinearGradient(glossGradient, CGPoint.Empty, Bounds.BottomRight(), CGGradientDrawingOptions.DrawsAfterEndLocation);
			glossGradient.Dispose();
			rgbColorspace.Dispose ();
			base.BackgroundColor = UIColor.Clear;
			base.Draw((CGRect)rect);
		}

	}
}

