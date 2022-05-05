//-----------------------------------------------------------------------
// <copyright file="UIFixedOrientationViewController.cs" company="Sphere 10 Software">
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

	public partial class UIFixedOrientationViewController : UIViewController
	{
		private int _ignoreRestoreOrientation;
		private bool _forceOrientation;
		private UIInterfaceOrientation _orientationOnEntry;
		private UIInterfaceOrientation _forcedOrientation;

		public UIFixedOrientationViewController () : base() {
			_ignoreRestoreOrientation = 0;
			_forceOrientation = false;
		}

		public UIFixedOrientationViewController (UIInterfaceOrientation forcedOrientation) : base() {
			_ignoreRestoreOrientation = 0;
			_forceOrientation = true;
			_forcedOrientation = forcedOrientation;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()	{
			switch (_forcedOrientation) {
			case UIInterfaceOrientation.LandscapeLeft:
				return UIInterfaceOrientationMask.LandscapeLeft;
			case UIInterfaceOrientation.LandscapeRight:
				return UIInterfaceOrientationMask.LandscapeRight;
			case UIInterfaceOrientation.Portrait:
				return UIInterfaceOrientationMask.Portrait;
			case UIInterfaceOrientation.PortraitUpsideDown:
				return UIInterfaceOrientationMask.PortraitUpsideDown;
			}
			return UIInterfaceOrientationMask.All;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)	{
			//return base.ShouldAutorotateToInterfaceOrientation (toInterfaceOrientation);
			if (toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft)
				return true;
			return false;
		}

		public void IgnoreRestoreOrientation(int times) {
			_ignoreRestoreOrientation = times;
		}

		public override void ViewWillAppear (bool animated)
		{
			if (_ignoreRestoreOrientation == 0) {
				// The way to get rid of this is to supply GetSupportedInterfaceOrientations at the root level controller (or one just below)
				if (_forceOrientation) {
                    _orientationOnEntry = Tools.iOSTool.CurrentInterfaceOrientation;
					if (_orientationOnEntry != _forcedOrientation)
                        Tools.iOSTool.SetOrientation(_forcedOrientation);
				}
			} else {
				_ignoreRestoreOrientation =  (_ignoreRestoreOrientation - 1).ClipTo(0, int.MaxValue);
			}
		}

		public override void ViewWillDisappear (bool animated) {
			if (_forceOrientation) {
                if (Tools.iOSTool.CurrentInterfaceOrientation != _orientationOnEntry) 
					if (_ignoreRestoreOrientation == 0)
                        Tools.iOSTool.SetOrientation(_orientationOnEntry);
			}
		}
	
	}
}



