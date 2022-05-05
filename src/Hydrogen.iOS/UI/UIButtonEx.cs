//-----------------------------------------------------------------------
// <copyright file="UIButtonEx.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS {

	/// <summary>
	/// Inherit from this to workaround the bug seen here (http://stackoverflow.com/questions/14081410/extended-uibutton-border-is-not-initially-drawn)
	/// </summary>
	public abstract class UIButtonEx : UIButton {

		public UIButtonEx(UIButtonType buttonType) : base(buttonType) { }

		public override CGRect Frame {
			get {
				return (CGRect)base.Frame;
			}
			set {
				var temp = TranslatesAutoresizingMaskIntoConstraints;
				TranslatesAutoresizingMaskIntoConstraints = false;
				var constraints = new [] {
(NSLayoutConstraint)(					NSLayoutConstraint.Create(this, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1.0f, value.Width)),
(NSLayoutConstraint)(					NSLayoutConstraint.Create(this, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1.0f, value.Height)
)				};
				AddConstraints(constraints);
				SizeToFit();
				RemoveConstraints(constraints);
				base.Frame = value;
				TranslatesAutoresizingMaskIntoConstraints = temp;
			}
		}
	}
}

