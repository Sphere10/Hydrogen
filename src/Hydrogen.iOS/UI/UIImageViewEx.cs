//-----------------------------------------------------------------------
// <copyright file="UIImageViewEx.cs" company="Sphere 10 Software">
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
using Foundation;

namespace Hydrogen.iOS {
	public class UIImageViewEx : UIImageView	{
		public event EventHandler TouchBegan;
		public event EventHandler TouchEnded;

		public UIImageViewEx (CGRect frame) : base(frame) {
			Initialize();
		}

		public UIImageViewEx (UIImage image) : base(image) {
			Initialize();
		}

		private void Initialize() {
			UserInteractionEnabled = true;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)	{
			if (TouchBegan != null)
				TouchBegan(this, EventArgs.Empty);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)	{
			if (TouchEnded != null)
				TouchEnded(this, EventArgs.Empty);
		}


	}
}

