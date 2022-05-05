//-----------------------------------------------------------------------
// <copyright file="UIZoomableViewController.cs" company="Sphere 10 Software">
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
	// https://github.com/StuartMorris0/SPMZoomableUIImageView/blob/master/ZoomableScrollView/ZoomableScrollView/ViewController.m
	// http://codehappily.wordpress.com/2013/09/26/ios-how-to-use-uiscrollview-with-auto-layout-pure-auto-layout/
	[Foundation.Register]
	public class UIZoomableViewController : UIViewController {
		private bool _isInternalScrollView;
		private UIScrollView _scrollView;
		private UIView _contentView;

		public UIZoomableViewController() : this(new UIScrollView()) {
			_isInternalScrollView = true;
		}

		public UIZoomableViewController(UIScrollView scrollView) {
			_scrollView = scrollView;
			_scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
			_scrollView.Layer.BorderColor = UIColor.Blue.CGColor;
			_scrollView.Layer.BorderWidth = 2;
			_scrollView.BackgroundColor = UIColor.Gray;
		}

		public UIView ContentView {
			get {
				return _contentView;
			}
			set {
				if (_contentView != null)
					_contentView.RemoveFromSuperview();
				_contentView = value;
				if (IsViewLoaded) {
					SetupScales();
				}
			}
		}

		public override void ViewDidLoad() {
			base.ViewDidLoad();
			_contentView.TranslatesAutoresizingMaskIntoConstraints = false;
			_contentView.Frame = new CGRect(CGPoint.Empty, _contentView.Frame.Size);
			_scrollView.AddSubviewDockFull(_contentView);
			_scrollView.ViewForZoomingInScrollView = (x) => _contentView;
			if (_isInternalScrollView) 
				this.View.AddSubviewDockFull(_scrollView);
		}

		public void SetupScales() {

			// Set up the minimum & maximum zoom scales
			var scrollViewFrameSize = _scrollView.Frame.Size;
			var scaleWidth = scrollViewFrameSize.Width / ContentView.IntrinsicContentSize.Width;
			var scaleHeight = scrollViewFrameSize.Height / ContentView.IntrinsicContentSize.Height;
            var minScale = (nfloat)Math.Max(scaleWidth, scaleHeight);

			_scrollView.MinimumZoomScale = 0.1f; // minScale;
			_scrollView.MaximumZoomScale = 2.0f;
			_scrollView.ZoomScale = minScale;

			CenterScrollViewContents();
		}

		private void CenterScrollViewContents() {
			return;
			var scrollViewBounds = _scrollView.Bounds;
			var contentViewSize = _contentView.Frame.Size;

			_scrollView.Bounds = new CGRect(
				new CGPoint(
					-(contentViewSize.Width - scrollViewBounds.Width) / 2.0f,
					-(contentViewSize.Height - scrollViewBounds.Height) / 2.0f
				),
				_scrollView.Bounds.Size
			);
			_scrollView.LayoutSubviews();

			// This method centers the scroll view contents also used on did zoom

		}


		public override void ViewDidLayoutSubviews() {
			base.ViewDidLayoutSubviews();
			/*Console.WriteLine("UIScrollView.Frame = (X:{0}, Y:{1}, W:{2}, H:{3})", _scrollView.Frame.X,_scrollView.Frame.Y,_scrollView.Frame.Width,_scrollView.Frame.Height);
			Console.WriteLine("UIScrollView.Bounds = ({0}, {1}, {2}, {3})", _scrollView.Bounds.X,_scrollView.Bounds.Y,_scrollView.Bounds.Width,_scrollView.Bounds.Height);
			Console.WriteLine("UIScrollView.Size = (W:{0}, H:{1})", _scrollView.Frame.Size.Width, _scrollView.Frame.Size.Height);
			Console.WriteLine("UIScrollView.ZoomScale = {0}", _scrollView.ZoomScale);
			Console.WriteLine("UIScrollView.ContentSize = (W:{0}, H:{1})", _scrollView.ContentSize.Width, _scrollView.ContentSize.Height);
			Console.WriteLine("UIImageView.Frame = (X:{0}, Y:{1}, W:{2}, H:{3})", _contentView.Frame.X,_contentView.Frame.Y,_contentView.Frame.Width,_contentView.Frame.Height);
			Console.WriteLine("UIImageView.Bounds = ({0}, {1}, {2}, {3})", _contentView.Bounds.X,_contentView.Bounds.Y,_contentView.Bounds.Width,_contentView.Bounds.Height);
			Console.WriteLine("UIImageView.Size = (W:{0}, H:{1})", _contentView.Frame.Size.Width, _contentView.Frame.Size.Height);
			Console.WriteLine("Image.Size = (W:{0}, H:{1})", _contentView.IntrinsicContentSize.Width, _contentView.IntrinsicContentSize.Height);*/
		}
	}
}

