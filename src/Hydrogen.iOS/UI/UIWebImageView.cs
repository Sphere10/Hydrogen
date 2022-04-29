//-----------------------------------------------------------------------
// <copyright file="UIWebImageView.cs" company="Sphere 10 Software">
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

// --------------
// ESCOZ.COM
// --------------
using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Hydrogen.iOS {

	public partial class UIWebImageView : UIImageView {

		NSMutableData imageData;
		UIActivityIndicatorView indicatorView;
	
		public UIWebImageView (IntPtr handle) : base(handle) {
			Initialize ();
		}

		[Foundation.Export("initWithCoder:")]
		public UIWebImageView (NSCoder coder) : base(coder)	{
			Initialize ();
		}


		void Initialize () {
			indicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
			indicatorView.HidesWhenStopped = true;
			var width  = (this.Frame.Width-20)/2;
			var height = (this.Frame.Height-20)/2;
			indicatorView.Frame = new CGRect(width, height,20,20);
			this.AddSubview(indicatorView);
		}
		
		public UIWebImageView(CGRect frame) {
			Initialize();
			
			indicatorView.Frame = 
				new CGRect (
                	frame.Size.Width/2,
                	frame.Size.Height/2,
                  	indicatorView.Frame.Size.Width,
                 	indicatorView.Frame.Size.Height
				);

		}
		
		public UIWebImageView(CGRect frame, string url):base(frame){
			Initialize();
			Frame = frame;
			DownloadImage(url);
		}
		
		public void DownloadImage(string url) {
			indicatorView.StartAnimating();
			var request = new NSUrlRequest(new NSUrl(url));
			var connection = new NSUrlConnection(request, new ConnectionDelegate(this), true);

		}
		
		class ConnectionDelegate : NSUrlConnectionDelegate {
		    readonly UIWebImageView _view;
			
			public ConnectionDelegate(UIWebImageView view){
				_view = view;
			}
			
             
			public void ReceivedData (NSUrlConnection connection, NSData data)
			{
				if (_view.imageData==null)
					_view.imageData = new NSMutableData();
				
				_view.imageData.AppendData(data);	
			}
			
			public void FinishedLoading (NSUrlConnection connection)
			{
				_view.indicatorView.StopAnimating();
				UIImage downloadedImage = UIImage.LoadFromData(_view.imageData);
				_view.imageData = null;
				_view.Image = downloadedImage;
			}
		}
	}
}
