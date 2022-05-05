//-----------------------------------------------------------------------
// <copyright file="UIPaintingViewController.cs" company="Sphere 10 Software">
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
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using OpenGLES;
using System.Runtime.InteropServices;
using CoreGraphics;
using System.Collections.Generic;
using System.Text;
using Hydrogen;


namespace Hydrogen.iOS {

	public partial class UIPaintingViewController : UIFixedOrientationViewController {

		private UIPaintingView _paintingView;

		public UIPaintingViewController () : base() {
		}

		public UIPaintingViewController (UIInterfaceOrientation forcedOrientation) : base() {
		}

		public UIPaintingView PaintingView { get { return _paintingView; } }

	
		public override void ViewDidLoad ()
		{
			try {
				_paintingView = new UIPaintingView (GetPaintingViewFrame());
				View.AddSubview(_paintingView);
			} catch (Exception error) {
				SystemLog.Exception(error);
			}
		}

		public void Clear() {
			_paintingView.Erase();
		}

	

		protected virtual CGRect GetPaintingViewFrame() {
			throw new NotImplementedException("Needs fixing to support rotating views");
			//var size = GraphicsTool.CGSizeorOrientation(UIInterfaceOrientation.LandscapeLeft);
			//return new CGRect(0,0, size.Width, size.Height);
		}
	
	}
}

