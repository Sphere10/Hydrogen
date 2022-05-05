//-----------------------------------------------------------------------
// <copyright file="MessageBox.cs" company="Sphere 10 Software">
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
using CoreGraphics;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Hydrogen.iOS {
	public enum MessageBoxResult {
		None = 0,
		OK,
		Cancel,
		Yes,
		No
	}

	public enum MessageBoxButton {
		OK = 0,
		OKCancel,
		YesNo,
		YesNoCancel,
		None
	}

	public static class MessageBox {
		public static async Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton buttonType, bool yesIsDestructive = false, UIViewController parentController = null) {
			var result = MessageBoxResult.Cancel;
			var signal = new ManualResetEventSlim();
			var yesStyle = yesIsDestructive ? UIAlertActionStyle.Destructive : UIAlertActionStyle.Default;
			var buttons = new List<Tuple<string, MessageBoxResult, UIAlertActionStyle>>();
			parentController = parentController ?? Tools.iOSTool.GetTopMostController();
			switch (buttonType) {
				case MessageBoxButton.OK:
					buttons.Add(Tuple.Create("OK", MessageBoxResult.OK, yesStyle));
					break;
				
				case MessageBoxButton.OKCancel:
		            buttons.Add(Tuple.Create("Cancel", MessageBoxResult.Cancel, UIAlertActionStyle.Cancel));
					buttons.Add(Tuple.Create("OK", MessageBoxResult.OK, yesStyle));
					break;
				
				case MessageBoxButton.YesNo:
		            buttons.Add(Tuple.Create("No", MessageBoxResult.No, UIAlertActionStyle.Cancel));
					buttons.Add(Tuple.Create("Yes", MessageBoxResult.Yes, yesStyle));
					break;
				
				case MessageBoxButton.YesNoCancel:
					buttons.Add(Tuple.Create("Cancel", MessageBoxResult.Cancel, UIAlertActionStyle.Cancel));
		            buttons.Add(Tuple.Create("No", MessageBoxResult.No, UIAlertActionStyle.Default));
					buttons.Add(Tuple.Create("Yes", MessageBoxResult.Yes, yesStyle));
					break;
				case MessageBoxButton.None:
					break;
			}

			var alert = UIAlertController.Create(caption, messageBoxText, UIAlertControllerStyle.Alert);

			foreach (var button in buttons) {
				alert.AddAction(UIAlertAction.Create(button.Item1, button.Item3, (x) => {
					result = button.Item2;
					signal.Set();
				}));
			}
			/*if (parentController.PresentingViewController != null)
				parentController = parentController.PresentingViewController;

			var window = UIApplication.SharedApplication.KeyWindow;
			var root = window.RootViewController;
			var rootView = root.View;*/
			var window = new UIWindow();
			window.RootViewController = new UIViewController();
			window.BackgroundColor = UIColor.Clear;
			window.Bounds = parentController.View.Window.Bounds;
			window.TintColor = parentController.View.Window.TintColor;
			window.WindowLevel = UIApplication.SharedApplication.Windows.Last().WindowLevel + 1;
			window.MakeKeyAndVisible();
			await window.RootViewController.PresentViewControllerAsync(alert, true);
			await Task.Run(() => signal.Wait());
			window.Hidden = true;
			window.Dispose();
			return result;
		}

		public static Task Show(string messageBoxText) {
			return Show(messageBoxText, "", MessageBoxButton.OK);
		}

		public static Task Show(string messageBoxText, string caption) {
			return Show(messageBoxText, caption, MessageBoxButton.OK);
		}
	}

}
