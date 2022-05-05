//-----------------------------------------------------------------------
// <copyright file="UIButtonViewExtensions.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS {
    public static class UIButtonViewExtensions {


        public static void SetTitleForAllStates(this UIButton button, string title) {
            button.SetTitle(title, UIControlState.Application);
            button.SetTitle(title, UIControlState.Disabled);
            button.SetTitle(title, UIControlState.Highlighted);
            button.SetTitle(title, UIControlState.Normal);
            button.SetTitle(title, UIControlState.Reserved);
            button.SetTitle(title, UIControlState.Selected);
        }


    }
}

