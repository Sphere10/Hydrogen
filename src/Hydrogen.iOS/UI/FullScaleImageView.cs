//-----------------------------------------------------------------------
// <copyright file="FullScaleImageView.cs" company="Sphere 10 Software">
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
	public class FullScaleImageView : UIScrollView {

		public FullScaleImageView (UIImage image) {

			InternalImageView = new UIImageView (image);

			this.AddSubview (InternalImageView);
			this.ContentSize = InternalImageView.Frame.Size;
			this.MinimumZoomScale = 0.001f;
			this.MaximumZoomScale = 100.0f;


		//	this.SetZoomScale(scale, false);
			this.MultipleTouchEnabled = true;
			this.ViewForZoomingInScrollView = delegate(UIScrollView scrollView) {
				return InternalImageView;
			};      
		}


		public UIImageView InternalImageView { get; private set; }
	}


	public class FullScaleImageViewController : UIViewController {

		public FullScaleImageViewController (UIImage image)	{
			ImageView = new FullScaleImageView(image);
		}

		public FullScaleImageView ImageView { get; set; }

		public override void ViewDidLoad ()	{
			ImageView.Frame = (CGRect)this.View.Frame;
			View.Add(ImageView);
		}
	
		public override void ViewWillAppear (bool animated)	{
			var minScreen = (nfloat)Math.Min (ImageView.Bounds.Width, ImageView.Bounds.Height);
            var minImage = (nfloat)Math.Min(ImageView.ContentSize.Width, ImageView.ContentSize.Height);
			if (minImage > 0.001 || minScreen > 0.001) {
				var scale =  minScreen / minImage;
				ImageView.SetZoomScale(scale, false);
			}
		}
	}
}

