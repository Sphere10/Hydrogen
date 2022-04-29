//-----------------------------------------------------------------------
// <copyright file="GenericTableViewCell.cs" company="Sphere 10 Software">
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

	public class GenericTableViewCell : Hydrogen.iOS.BadgeCell {
		private const float DefaultImageViewWidth = 40.0f;
		private const float DefaultImageViewHeight = 40.0f;
		private readonly CGSize _imageViewSize;


		public GenericTableViewCell(UITableViewCellStyle style, string reuseID, CGSize? imageViewSize = null) : base(style, reuseID, false) {
			_imageViewSize = imageViewSize != null ? imageViewSize.Value : new CGSize(DefaultImageViewWidth, DefaultImageViewHeight);
			if (ImageView != null)
				ImageView.Layer.MasksToBounds = true;
		}

		public override void LayoutSubviews() {
			base.LayoutSubviews();
			if (ImageView != null && ImageView.Frame != CGRect.Empty) {
				var desiredWidth = _imageViewSize.Width;
				var desiredHeight = _imageViewSize.Height;
				var imgFrame = ImageView.Frame;
				var w = imgFrame.Size.Width;
				var h = imgFrame.Size.Height;
				if (w <= desiredWidth)
					return;
				var widthSub = w - desiredWidth;
				var heightSub = h - desiredHeight;
				ImageView.Frame = new CGRect(imgFrame.Location.X, imgFrame.Location.Y, desiredWidth, desiredHeight);
				this.TextLabel.Frame = new CGRect(TextLabel.Frame.Location.X - widthSub, TextLabel.Frame.Location.Y, TextLabel.Frame.Size.Width + widthSub, TextLabel.Frame.Size.Height);
				if (DetailTextLabel != null)
					this.DetailTextLabel.Frame = new CGRect(DetailTextLabel.Frame.Location.X - widthSub, DetailTextLabel.Frame.Location.Y, DetailTextLabel.Frame.Size.Width + widthSub, DetailTextLabel.Frame.Size.Height);
				if (this.AccessoryView != null) {
					var xyz = this.AccessoryView.Frame;
					var xxx = 1;
				}

			}
		}
	}

}

