//-----------------------------------------------------------------------
// <copyright file="MyMoviePlayer.cs" company="Sphere 10 Software">
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
using MediaPlayer;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Hydrogen.iOS
{
	[Foundation.Register("MyMoviePlayer")]
	public class MyMoviePlayer : UIView
	{
		MPMoviePlayerController mp;
		UILabel lblLoading;
		public NSUrl MovieUrl {get;set;}
		public Action Done {get;set;}
		
		public void Play()
		{
			mp.Play();
		}
		
		private NSObject notificationObserver;
		
		public MyMoviePlayer (IntPtr handle) : base(handle)
		{
		}
		
		[Foundation.Export("initWithCoder:")]
		public MyMoviePlayer (NSCoder coder) : base(coder)
		{
		}
		
		public MyMoviePlayer (CGRect rect): base (rect) {
		}
		
		public MyMoviePlayer () {}
		
		
		
		public override void WillMoveToSuperview (UIView newsuper)
		{
			if (newsuper == null)
				return;
			this.BackgroundColor = UIColor.Black;
			lblLoading= new UILabel(new CGRect(20,20,100,100));
			lblLoading.BackgroundColor = UIColor.Clear;
			lblLoading.Text = "Loading";
			lblLoading.TextColor = UIColor.White;
			lblLoading.Font = (UIFont)(UIFont.FromName ("Helvetica", 17f));
			this.AddSubview(lblLoading);
			notificationObserver  = NSNotificationCenter.DefaultCenter
				.AddObserver(new NSString("MPMoviePlayerPlaybackDidFinishNotification"), WillExitFullScreen );
			mp = new MPMoviePlayerController (MovieUrl);
			mp.ControlStyle = MPMovieControlStyle.Fullscreen;
			mp.View.Frame = (CGRect)this.Bounds;		
			mp.SetFullscreen(true,true);
			this.AddSubview(mp.View);	
			
			mp.Play();
		}
		
		private void WillExitFullScreen( NSNotification notification)
		{
			if (Done != null)
				Done();
		}
		
		public override void RemoveFromSuperview ()
		{
			lblLoading.RemoveFromSuperview();
			mp.View.RemoveFromSuperview();
			mp.Dispose();
			NSNotificationCenter.DefaultCenter.RemoveObserver (notificationObserver);
			base.RemoveFromSuperview ();
		}
		
	}
}

