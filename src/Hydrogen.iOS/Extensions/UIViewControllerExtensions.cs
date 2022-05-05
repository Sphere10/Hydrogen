//-----------------------------------------------------------------------
// <copyright file="UIViewControllerExtensions.cs" company="Sphere 10 Software">
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
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Hydrogen.iOS {
    public static class UIViewControllerExtensions {


		public static Task EnsureUIThread(this UIViewController viewController, Func<Task> function) {
			return Tools.iOSTool.InvokeOnUIThread(function);
		}

		public static Task<T1> EnsureUIThread<T1>(this UIViewController viewController, Func<Task<T1>> function) {
			return Tools.iOSTool.InvokeOnUIThread(function);
		}

        public static UIViewController FindPresentedViewController(this UIViewController viewController) {
            if (viewController == null)
                return null;

            if (viewController.PresentedViewController != null)
                return viewController.PresentedViewController;

            foreach (var childViewController in viewController.ChildViewControllers) {
                var presentedController = FindPresentedViewController(childViewController);
                if (presentedController != null)
                    return presentedController;
            }

            return null;
        }

        // from: http://stackoverflow.com/questions/18860614/uiview-real-frame-in-ios7
        public static CGRect GetUsableContentSize(this UIViewController viewController) {
            CGRect rect;
            UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
            CGRect statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;
            nfloat statusBarHeight = 0.0f;

            var view = viewController.View;

            if (view.Superview != null)
                rect = view.Superview.Frame; // the frame will be relative to the superview. Note in iOS6 the root superview frame includes the
            else
                rect = view.Frame;


            // REMOVED in iOS 8.3
            /*if (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown)
            {
                rect.Height += rect.Y; // total height
                statusBarHeight = statusBarFrame.Height;
            }
            else
            {
                rect.Height += rect.X; // total height
                statusBarHeight = statusBarFrame.Width;
            }*/

            // iOS 8.3 seems to properly orient view frames
            rect.Height += rect.Y; // total height
            statusBarHeight = statusBarFrame.Height;


            nfloat extraHeight = 0;    // this is going to be how much extra height we have to take off of the total height of the frame,
            // which is going to be the status bar+navbar+toolbar heights
            nfloat topHeight = 0;      // this is the height of the stuff before the frame, which we are going to use as an offset


            if (!UIApplication.SharedApplication.StatusBarHidden)
                extraHeight += statusBarHeight;

            if (viewController.NavigationController != null && !viewController.NavigationController.NavigationBar.Hidden)
                extraHeight += viewController.NavigationController.NavigationBar.Frame.Height;

            topHeight = extraHeight;
            if (viewController.NavigationController != null && !viewController.NavigationController.Toolbar.Hidden)
                extraHeight += viewController.NavigationController.Toolbar.Frame.Height;

            rect.Y = topHeight - rect.Y;  // in iOS6 the status bar and navbar is already included in the origin but not in iOS7 .
            if (rect.Y < 0)
                rect.Y = 0;


            //rect.Size.Height -= extraHeight;    // subtract status/nav and toolbar heights from the total height left in the superview
            //rect.Height -= extraHeight;    // subtract status/nav and toolbar heights from the total height left in the superview

            return rect;
        }


    }
}

