//-----------------------------------------------------------------------
// <copyright file="LoadingHUDViewController.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreGraphics;


namespace Hydrogen.iOS
{
	
	public partial class LoadingHUDViewController : UIViewController
	{
		
		LoadingHUDView _hud = new LoadingHUDView("Loading", "Wait for 5 more seconds and " +
			"this   text will automatically disappear");
		
		
		LoadingHUDView _hudSmall = new LoadingHUDView("Loading");
		
		public override void ViewWillAppear (bool animated)
		{
			View.AddSubview(_hud);
			View.AddSubview(_hudSmall);
			
			Title = "LoadingHUDView";
			
			UIButton btnDone = UIButton.FromType (UIButtonType.RoundedRect);
			btnDone.SetTitle("Large Message", UIControlState.Normal);
			btnDone.Frame = new CGRect(20,300, 280, 40);
			btnDone.TouchUpInside += HandleButtonTouchUpInside;
			View.AddSubview(btnDone);
			
			UIButton btnSmall = UIButton.FromType (UIButtonType.RoundedRect);
			btnSmall.SetTitle("Small Message", UIControlState.Normal);
			btnSmall.Frame = new CGRect(20,350, 280, 40);
			btnSmall.TouchUpInside += HandleBtnSmallTouchUpInside;;
			View.AddSubview(btnSmall);
			
			View.AddSubview(new UILabel{Text="Click to show the HUD View",
				Frame = new CGRect(0, 100, 320, 40), 
			TextAlignment = UITextAlignment.Center});
			
		}

		void HandleBtnSmallTouchUpInside (object sender, EventArgs e)
		{
			_hudSmall.StartAnimating();
			NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(5), (timer)=>_hudSmall.StopAnimating());
		}

		void HandleButtonTouchUpInside (object sender, EventArgs e)
		{
			_hud.StartAnimating();
			NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(5), (timer)=>_hud.StopAnimating());
		}
	}
}
