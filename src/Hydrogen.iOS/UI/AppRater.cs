//-----------------------------------------------------------------------
// <copyright file="AppRater.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS
{
	public static class AppRater
	{
		static NSUserDefaults settings = new NSUserDefaults("appRater");
		public static int RunCountNeeded = 5;
		public static int DaysInstalledCountNeeded = 2;
		static string url =  @"itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=" + AppId;
		static string AppId = "";
		public static void AppLaunched(string appId)
		{
			AppId = appId;
			var version = NSBundle.MainBundle.InfoDictionary.ObjectForKey(new NSString("CFBundleVersion")).ToString();
			if(settings.StringForKey("lastInstalledVersion") != version)
			{
				ResetWarningIndicators();
				settings.SetString(version,"lastInstalledVersion");
				settings.Synchronize();
			}
			TryToRate();
			RunCount += 1;
			Console.WriteLine("runcount" + RunCount);
			
			
		}
		public static void DidSomethingSignificant()
		{
			TryToRate();
			RunCount += 1;
		}
		static void ResetWarningIndicators()
		{
			RunCount = 0;
			DateVersionInstalled = DateTime.UtcNow;
			ShouldRateThisVersion = true;
			DidRate = false;
		}
		static int RunCount {
			get{return (int)(settings.IntForKey("runCount"));}
			set {settings.SetInt(value,"runCount");
				settings.Synchronize();
			}
		}
		static DateTime DateVersionInstalled
		{
			get{return DateTime.Parse(settings.StringForKey("dateInstalled"));}
			set{settings.SetString(value.ToString(),"dateInstalled");
				settings.Synchronize();}
		}
		static bool ShouldRateThisVersion
		{
			get{return settings.BoolForKey("shouldRate");}
			set{settings.SetBool(value,"shouldRate");}
		}
		static void TryToRate()
		{
			if(ShouldRate())
				Rate();
		}
		public static bool DidRate{
			get{return settings.BoolForKey("didRateVersion");}
			set{settings.SetBool(value,"didRateVersion");
				settings.Synchronize();}
		}
		public static void Rate()
		{
			isRating = true;
			var version = NSBundle.MainBundle.InfoDictionary.ObjectForKey(new NSString("CFBundleVersion"));
			
			var name = NSBundle.MainBundle.InfoDictionary.ObjectForKey(new NSString("CFBundleName")).ToString();
			var alert = new UIAlertView("Rate " + name,"If you enjoyed using " + name +". Will you please take a moment to rate it? Thanks for your support",null,"No, Thanks", "Rate " + name,"Remind Me Later");
			alert.Clicked += delegate(object sender, UIButtonEventArgs e) {
				isRating = false;
				if((int)e.ButtonIndex == 0)
					ShouldRateThisVersion = false;
				else if((int)e.ButtonIndex == 1)
				{
					DidRate = true;
					UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
				}
				else if((int)e.ButtonIndex == 2)
					ResetWarningIndicators();
			};
			alert.Show();
		}
		static bool isRating = false;
		static bool ShouldRate()
		{
			if(isRating)
				return false;
			if(DidRate)
				return false;
			if(!ShouldRateThisVersion)
				return false;
			if((DateTime.UtcNow - DateVersionInstalled).Days < DaysInstalledCountNeeded)
				return false;
			if(RunCount < RunCountNeeded)
				return false;
			return true;
		}
	}
}

