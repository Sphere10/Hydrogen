//-----------------------------------------------------------------------
// <copyright file="UICollectionViewImageCell.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using CoreGraphics;
using Hydrogen;
using Foundation;
using CoreAnimation;

namespace Hydrogen.iOS {

	public class UICollectionViewImageCell : UICollectionViewCell, ISpecialDisposable {
		public const string CellID = "UICollectionViewImageCell [fbb52a9f0a1640ceaf253bc080f94b89]";

		private CGSize _size;
		private UIImageView _imageView;

		[Foundation.Export("initWithFrame:")]
		public UICollectionViewImageCell(CGRect frame) : base(frame) {
			_size = frame.Size;
			_imageView = new UIImageView();
			_imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			_imageView.Layer.MinificationFilter = CALayer.FilterLinear;
			this.AddSubview(_imageView);
		}

		public UIImage Image {
			get {
				return _imageView.Image;
			}
			set {
				_imageView.Image = value;
				_imageView.Bounds = new CGRect(CGPoint.Empty, _size);
			}
		}

		public void SpecialDispose() {
			if (ContentView != null) {
				ContentView.DisposeEx();
			}
			if (_imageView != null) {
				_imageView.DisposeEx();
			}
			_imageView = null;

		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
			}
			base.Dispose(disposing);
		}
	}

}

