//-----------------------------------------------------------------------
// <copyright file="LoadingHUDView.cs" company="Sphere 10 Software">
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

//
//  LoadingHUDView.cs
//
//  Converted to iOS on 1/18/09 - Eduardo Scoz || http://escoz.com
//  Originaly created by Devin Ross on 7/2/09 - tapku.com || http://github.com/devinross/tapkulibrary
//
/*
 
 Permission is hereby granted, free of charge, to any person
 obtaining a copy of this software and associated documentation
 files (the "Software"), to deal in the Software without
 restriction, including without limitation the rights to use,
 copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the
 Software is furnished to do so, subject to the following
 conditions:
 
 The above copyright notice and this permission notice shall be
 included in all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 OTHER DEALINGS IN THE SOFTWARE.
 
 */

using UIKit;
using CoreGraphics;
using CoreGraphics;
using Foundation;
using System;

namespace Hydrogen.iOS {
	public class LoadingHUDView : UIView {
	
		public static int WIDTH_MARGIN = 20;
		public static int HEIGHT_MARGIN = 20;
	
		string _title, _message;
		UIActivityIndicatorView _activity;
		bool _hidden;
		UIFont titleFont = (UIFont)(UIFont.BoldSystemFontOfSize(16));
		UIFont messageFont = (UIFont)(UIFont.SystemFontOfSize(13));
	
		public string Title {
			get { return _title; }
			set {
				_title = value; 
				this.SetNeedsDisplay();
			}
		}
	
		public string Message {
			get { return _message; }
			set {
				_message = value; 
				this.SetNeedsDisplay();
			}
		}
	
		public LoadingHUDView(string title, string message) {
			Title = title;
			Message = message;
			_activity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
			_hidden = true;
			this.BackgroundColor = UIColor.Clear;
			Frame = new CGRect(0,0,320,480);
			this.AddSubview(_activity);
		}
	
		public LoadingHUDView(string title) : this(title, null) {}
	
		public void StartAnimating() {
			if (!_hidden) return;
	
			_hidden = false;
			this.SetNeedsDisplay();
			this.Superview.BringSubviewToFront(this);
			_activity.StartAnimating();
		}
	
		public void StopAnimating() {
			if (_hidden) return;
			
			_hidden = true;
			this.SetNeedsDisplay();
			this.Superview.SendSubviewToBack(this);
			_activity.StopAnimating();
		}
	
	
		protected void AdjustHeight() {
			CGSize titleSize = calculateHeightOfTextForWidth(_title, titleFont, 200, UILineBreakMode.TailTruncation);
			CGSize messageSize = calculateHeightOfTextForWidth(_message, messageFont, 200, UILineBreakMode.WordWrap);
	
			var textHeight = titleSize.Height + messageSize.Height;
			
			CGRect r = (CGRect)this.Frame;
			r.Size = new CGSize(300, textHeight + 20);
			this.Frame = r;
		}
	
		public override void Draw (CGRect rect)
		{
			if (_hidden) return;
	
			int width, rWidth, rHeight, x;
			CGSize titleSize = calculateHeightOfTextForWidth(_title, titleFont, 200, UILineBreakMode.TailTruncation);
			CGSize messageSize = calculateHeightOfTextForWidth(_message, messageFont, 200, UILineBreakMode.WordWrap);
	
			if (_title.Length<1) titleSize.Height = 0;
			if (_message==null || _message.Length<1) messageSize.Height = 0;
			
			rHeight = (int)(titleSize.Height+HEIGHT_MARGIN*2 + _activity.Frame.Size.Height);
			rHeight += (int)(messageSize.Height>0 ? messageSize.Height + 10 : 0);
			rWidth = width = (int)Math.Max(titleSize.Width, messageSize.Width);
			rWidth += WIDTH_MARGIN * 2;
			x = (320-rWidth) /2;
	
			_activity.Center = new CGPoint(320/2, HEIGHT_MARGIN + 20 + _activity.Frame.Size.Height/2);
	
			// Rounded rectangle
			CGRect areaRect = new CGRect(x, HEIGHT_MARGIN, rWidth, rHeight);
			this.DrawRoundRectangle(areaRect, 8, (UIColor)(UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.75f))); // alpha = 0.75
			
			// Title
			UIColor.White.SetColor();
			var textRect = new CGRect(x+WIDTH_MARGIN, _activity.Frame.Size.Height + 25 + HEIGHT_MARGIN,
				width, titleSize.Height);
			 CGSize titleDrawSize = UIKit.UIStringDrawing.DrawString(_title, textRect, titleFont, UILineBreakMode.TailTruncation, UITextAlignment.Center);
	
			// Description
			UIColor.White.SetColor();
			textRect.Y += titleDrawSize.Height+10;
			textRect = new CGRect(textRect.Location, new CGSize(textRect.Size.Width, messageSize.Height));
			
			if (_message!=null)
                UIKit.UIStringDrawing.DrawString(_message, textRect, messageFont, UILineBreakMode.WordWrap, UITextAlignment.Center);
		}
	
		protected CGSize calculateHeightOfTextForWidth(string text, UIFont font, float width, UILineBreakMode lineBreakMode){
			return text==null? new CGSize(0, 0) : UIKit.UIStringDrawing.StringSize(text, font, new CGSize(width, 300), lineBreakMode);
		}
	
		
	}

}
