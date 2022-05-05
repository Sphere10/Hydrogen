//-----------------------------------------------------------------------
// <copyright file="NoOverlapTableViewCell.cs" company="Sphere 10 Software">
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
	public class NoOverlapTableViewCell : UITableViewCell {
		
		public float? DetailTextLabelWidth = 0;  //this is here because it seems to be god damn impossible to calculate the size of the other elements when they use AutoresizingMask
		public CGSize MaxEntryPosition;
		public CGRect ContentViewBounds;
		
		public NoOverlapTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier) {
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			float indicatorWidth = 0;
			if (this.Accessory == UITableViewCellAccessory.DetailDisclosureButton
			    || this.Accessory == UITableViewCellAccessory.Checkmark) {
				indicatorWidth = 26f;
			}
			else if (this.Accessory == UITableViewCellAccessory.DisclosureIndicator) {
				indicatorWidth = 16f;
			}
			
			if (DetailTextLabel != null) {
				var yOffset = (ContentViewBounds.Height - MaxEntryPosition.Height) / 2 - 1;
				var adjust = (ContentViewBounds.Width - this.ContentView.Bounds.Width);
				var width = ContentViewBounds.Width - MaxEntryPosition.Width - indicatorWidth - adjust - 10f;
				var x = MaxEntryPosition.Width;
				if (DetailTextLabelWidth.HasValue) {
					width = DetailTextLabelWidth.Value;
					x = ContentViewBounds.Width - 30f - indicatorWidth - DetailTextLabelWidth.Value;
					TextLabel.Frame = new CoreGraphics.CGRect(10f, yOffset, x, MaxEntryPosition.Height);
				}
				DetailTextLabel.Frame = new CoreGraphics.CGRect(x, yOffset, width, MaxEntryPosition.Height);
			}
			
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin;
		}
	}
}

