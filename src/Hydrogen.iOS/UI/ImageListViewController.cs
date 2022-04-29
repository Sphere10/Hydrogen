//-----------------------------------------------------------------------
// <copyright file="ImageListViewController.cs" company="Sphere 10 Software">
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
using System.Collections;
using System.Collections.Generic;
using CoreGraphics;

namespace Hydrogen.iOS
{


	public class ImageListViewController : UIViewController
	{

		public override void ViewDidLoad ()
		{
			View.AddSubview(new ImageListView{
				Frame = (CGRect)View.Frame
			});
		}		
		
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return false;
        }
		
	}
	
	public class ImageListView : UIScrollView {
		
	}
}
