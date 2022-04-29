//-----------------------------------------------------------------------
// <copyright file="UIHorizontalButton.cs" company="Sphere 10 Software">
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
using CoreAnimation;

namespace Hydrogen.iOS
{
	public class UIHorizontalButton : UIButton
	{
		private bool _Initialized;
		
		public UIColor Color { get; set; }		
		public UIColor HighlightColor { get; set; }
		
		public string _Title = string.Empty;
		public new string Title 
		{ 
			get { return _Title; } 
			set 
			{ 
				_Title = value;
				
				SetNeedsDisplay();
			} 
		}
		
		public UIHorizontalButton(CGRect rect): base(rect)
		{
			Color = UIColor.FromRGB(88f, 170f, 34f);
			HighlightColor = UIColor.Black;
		}
		
		public void Init(CGRect rect)
		{
			Layer.MasksToBounds = true;
			Layer.CornerRadius = 8;
			
			var gradientFrame = rect;
			
			var shineFrame = gradientFrame;
			shineFrame.Y += 1;
			shineFrame.X += 1;
			shineFrame.Width -= 2;
			shineFrame.Height = (shineFrame.Height / 2);
			
			var shineLayer = new CAGradientLayer();
			shineLayer.Frame = shineFrame;
			shineLayer.Colors = new CoreGraphics.CGColor[] { (UIColor.White.ColorWithAlpha (0.75f)).CGColor, (UIColor.White.ColorWithAlpha (0.10f)).CGColor };
			//shineLayer.CornerRadius = 8;
			
			var backgroundLayer = new CAGradientLayer();
			backgroundLayer.Frame = gradientFrame;
			backgroundLayer.StartPoint = new CGPoint(0f,0.5f);
			backgroundLayer.EndPoint = new CGPoint(1.0f,0.5f);
			backgroundLayer.Colors = new CoreGraphics.CGColor[] { (Color.ColorWithAlpha(0.99f)).CGColor, UIColor.FromRGB(173,255,47).CGColor ,(Color.ColorWithAlpha(0.80f)).CGColor };
			
			
			var highlightLayer = new CAGradientLayer();
			highlightLayer.Frame = gradientFrame;
			
			Layer.AddSublayer(backgroundLayer);
			Layer.AddSublayer(highlightLayer);
			Layer.AddSublayer(shineLayer);
			
			VerticalAlignment = UIControlContentVerticalAlignment.Center;
			Font = (UIFont)(UIFont.BoldSystemFontOfSize (17));
			SetTitle (Title, UIControlState.Normal);
			SetTitleColor (UIColor.White, UIControlState.Normal);
			SetTitleShadowColor(UIColor.DarkGray,UIControlState.Normal);
			this.TitleShadowOffset = new CGSize(1f,1f);
			_Initialized = true;
		}
		
		public override void Draw(CGRect rect)
		{
			base.Draw((CGRect)rect);
			
			if(!_Initialized)
				Init((CGRect)rect);
			
			var highlightLayer = Layer.Sublayers[1] as CAGradientLayer;
			
			if (Highlighted)
			{
				if (HighlightColor == UIColor.Blue) 
				{
					highlightLayer.Colors = new CoreGraphics.CGColor[] { HighlightColor.ColorWithAlpha(0.60f).CGColor, HighlightColor.ColorWithAlpha(0.95f).CGColor };
				} 
				else 
				{
					highlightLayer.Colors = new CoreGraphics.CGColor[] { HighlightColor.ColorWithAlpha(0.10f).CGColor, HighlightColor.ColorWithAlpha(0.40f).CGColor };
				}
				
			}
			
			highlightLayer.Hidden = !Highlighted;
		}
		
		public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
		{
			if (uievent.Type == UIEventType.Touches)
			{
				SetNeedsDisplay();
			}
			
			return base.BeginTracking(uitouch, uievent); 
		}
		
		public override void EndTracking(UITouch uitouch, UIEvent uievent)
		{
			if (uievent.Type == UIEventType.Touches)
			{
				SetNeedsDisplay();
			}
			
			base.EndTracking(uitouch, uievent);
		}
	}
}

