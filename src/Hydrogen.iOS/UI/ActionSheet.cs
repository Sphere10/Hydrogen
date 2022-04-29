//-----------------------------------------------------------------------
// <copyright file="ActionSheet.cs" company="Sphere 10 Software">
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
using AVFoundation;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Hydrogen;

namespace Hydrogen.iOS {

    public static class ActionSheet
    {

        public static Task<int?> ShowInCell(UIView view, string title, UIActionSheetStyle style, params string[] options)
        {
            return ShowInternal(view, title, style, options);
        }

        public static Task<int?> ShowFromButtonItem(UIBarButtonItem item, bool animated, string title, UIActionSheetStyle style, params string[] options)
        {
            return ShowInternal(item, title, style, options);
        }

        public static Task<int?> ShowFromTabBar(UITabBar tabBar, string title, UIActionSheetStyle style, params string[] options)
        {
            return ShowInternal(tabBar, title, style, options);
        }

        public static Task<int?> ShowFromToolbar(UIToolbar toolbar, string title, UIActionSheetStyle style, params string[] options)
        {
            return ShowInternal(toolbar, title, style, options);
        }

        public static Task<int?> Show(object userInterfaceObject, string title, UIActionSheetStyle style, params string[] options)
        {
            return ShowInternal(userInterfaceObject, title, style, options);
        }

        private static async Task<int?> ShowInternal(object uiViewObject, string title, UIActionSheetStyle style, params string[] options)
        {
            var signal = new ManualResetEventSlim();
            int? retval = null;
            var alertController = UIAlertController.Create(title ?? string.Empty, string.Empty, UIAlertControllerStyle.ActionSheet);

            if (uiViewObject != null)
                alertController.PopoverPresentationController?.SetPresentationAnchor(uiViewObject);

            for (int i = 0; i < options.Length; i++)
            {
                var option = options[i];
                var index = i;
                alertController.AddAction(
                    UIAlertAction.Create(option, UIAlertActionStyle.Default,
                      (x) => {
                          retval = index;
                          signal.Set();
                      }
                    )
                );
            }
            alertController.AddAction(
                UIAlertAction.Create(
                    "Cancel",
                    UIAlertActionStyle.Cancel,
                    (x) => {
                        retval = null;
                        signal.Set();
                    }
                )
            );

            Tools.iOSTool.GetTopMostController().PresentViewController(alertController, true, null);
            await Task.Run(() => signal.Wait());
            return retval;
        }
    }
}
