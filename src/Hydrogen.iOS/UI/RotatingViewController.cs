//-----------------------------------------------------------------------
// <copyright file="RotatingViewController.cs" company="Sphere 10 Software">
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
using Foundation;
using UIKit;
using CoreGraphics;

namespace Hydrogen.iOS
{
	
	[Foundation.Register("RotatingViewController")]
	public class RotatingViewController : UIViewController
	{
		public UIViewController LandscapeLeftViewController {get;set;}
		public UIViewController LandscapeRightViewController {get;set;}
		public UIViewController PortraitViewController {get;set;}
		
		private NSObject notificationObserver;

		public RotatingViewController (IntPtr handle) : base(handle)
		{
		}

		[Foundation.Export("initWithCoder:")]
		public RotatingViewController (NSCoder coder) : base(coder)
		{
		}

		public RotatingViewController (string nibName, NSBundle bundle) : base(nibName, bundle)
		{
		}
		
		public RotatingViewController () {}

		public override void ViewWillAppear (bool animated)
		{
			_showView(PortraitViewController.View);
		}
		private void _showView(UIView view){
			
			if (this.NavigationController!=null)
				NavigationController.SetNavigationBarHidden(view!=PortraitViewController.View, false);
			
			_removeAllViews();
			view.Frame = (CGRect)this.View.Frame;
			View.AddSubview(view);
			
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		
		public override void ViewDidLoad()
		{
			notificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIDeviceOrientationDidChangeNotification"), DeviceRotated );
		}
		
		public override void ViewDidAppear (bool animated)
		{
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
		}

		
		private void DeviceRotated(NSNotification notification){
			
			Console.WriteLine("rotated! "+UIDevice.CurrentDevice.Orientation);
			switch (UIDevice.CurrentDevice.Orientation){
				
				case  UIDeviceOrientation.Portrait:
					_showView(PortraitViewController.View);
					break;
				
				case UIDeviceOrientation.LandscapeLeft:
					_showView(LandscapeLeftViewController.View);
				
					break;
				case UIDeviceOrientation.LandscapeRight:
					_showView(LandscapeRightViewController.View);
					break;
			}
		}
		
		private void _removeAllViews(){
			PortraitViewController.View.RemoveFromSuperview();
			LandscapeLeftViewController.View.RemoveFromSuperview();
			LandscapeRightViewController.View.RemoveFromSuperview();
		}
		protected void OnDeviceRotated(){
			
		}
			                  
			                  
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (notificationObserver);
		}
			
	}
}
