//-----------------------------------------------------------------------
// <copyright file="LevelMeter.cs" company="Sphere 10 Software">
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
using AudioToolbox;
using CoreGraphics;
using UIKit;

namespace Hydrogen.iOS
{
	public class LevelMeter : UIView
	{
		public AudioQueueLevelMeterState[] AudioLevelState {get;set;}
		public LevelMeter (CGRect rect ) :base (rect)
		{
			Padding = 10;
			this.Alpha = .8f;
		}
		public nfloat Padding {get;set;}
		public override void Draw (CGRect rect)
		{
			if(AudioLevelState == null || AudioLevelState.Length == 0)
			{
				base.Draw((CGRect)rect);
				return;
			}
			
			var width = (this.Frame.Width - (Padding * 2))/2;// AudioLevelState.Length;
			var height = (this.Frame.Height - (Padding * 2));
			var curX = Padding;
			var curY = Padding + height;
			UIColor.White.SetColor();
			
			var context = UIGraphics.GetCurrentContext();
			context.ClearRect((CGRect)this.Bounds);
			context.SetFillColor(UIColor.Green.CGColor);
			
			int index = 0;
			foreach(var level in AudioLevelState)
			{
				if(index > 1)
					continue;
				var power = level.AveragePower;
				if(power > 1)
					power = 1;
				else if(power < 0 )
					power = 0;
				context.FillRect(new CGRect(curX,curY,(width -  2),(height -2) * power * -1));
				curX += width;
				index ++;
			}
			
			//context.FillRect(new CGRect(curX,curY,(width - Padding) * AudioLevelState[0].AveragePower,height -2));
			//curY += height;
			//context.FillRect(new CGRect(curX,curY,(width - Padding) * AudioLevelState[1].AveragePower,height -2));
			context.Flush();
		}
	}
}

