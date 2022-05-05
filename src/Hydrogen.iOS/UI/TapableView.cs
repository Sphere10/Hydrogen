//-----------------------------------------------------------------------
// <copyright file="TapableView.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS
{

	public delegate void ViewTap (UIView view);
	public class TapableView : UIView
	{
		private bool multipleTouches = false;
		public ViewTap Tapped;
		public ViewTap DoubleTapped;
		private bool twoFingerTapIsPossible = true;
		
		public TapableView () : base()
		{
			
		}
		public TapableView (CGRect rect) : base(rect)
		{
			
		}
		
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			try
			{
				if ((uint)evt.TouchesForView (this).Count > 1)
					multipleTouches = true;
				if ((uint)evt.TouchesForView (this).Count > 2)
					twoFingerTapIsPossible = true;
			}
			catch
			{
				
			}
			base.TouchesBegan (touches, evt);
		}
		
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			try
			{
				bool allTouchesEnded = ((uint)touches.Count == (uint)evt.TouchesForView (this).Count);
				
				// first check for plain single/double tap, which is only possible if we haven't seen multiple touches
				if (!multipleTouches)
				{
					var touch = (UITouch)touches.AnyObject;
					// tapLocation = touch.LocationInView(this);
					if ((int)touch.TapCount == 1)
					{
						if (Tapped != null)
							Tapped (this);
					}
					else if ((int)touch.TapCount == 2)
					{
						if (DoubleTapped != null)
							DoubleTapped (this);
					}
				}
			}
			catch
			{
			}
			base.TouchesEnded (touches, evt);
		}
		
	}
	/*
	public delegate void ViewTap (UIView view);
	public class TapableView : UIView
	{
		private bool multipleTouches = false;
		public ViewTap Tapped;
		public ViewTap DoubleTapped;
		private bool twoFingerTapIsPossible = true;
		
		public TapableView () : base()
		{
			
		}
		public TapableView (CGRect rect) : base(rect)
		{
			
		}
		
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			try
			{
				if (evt.TouchesForView (this).Count > 1)
					multipleTouches = true;
				if (evt.TouchesForView (this).Count > 2)
					twoFingerTapIsPossible = true;
			}
			catch
			{
				
			}
			base.TouchesBegan (touches, evt);
		}
		
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			try
			{
				bool allTouchesEnded = (touches.Count == evt.TouchesForView (this).Count);
				
				// first check for plain single/double tap, which is only possible if we haven't seen multiple touches
				if (!multipleTouches)
				{
					var touch = (UITouch)touches.AnyObject;
					// tapLocation = touch.LocationInView(this);
					if (touch.TapCount == 1)
					{
						if (Tapped != null)
							Tapped (this);
					}
					else if (touch.TapCount == 2)
					{
						if (DoubleTapped != null)
							DoubleTapped (this);
					}
				}
			}
			catch
			{
			}
			base.TouchesEnded (touches, evt);
		}
		
	}*/
}

