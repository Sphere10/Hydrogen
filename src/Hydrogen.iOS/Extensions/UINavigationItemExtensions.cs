//-----------------------------------------------------------------------
// <copyright file="UINavigationItemExtensions.cs" company="Sphere 10 Software">
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
using Hydrogen;
using System.Linq;

namespace Hydrogen.iOS {
    public static class UINavigationItemExtensions {


        public static void AddLeftItem(this UINavigationItem navigationItem, UIBarButtonItem item, bool skipIfAlreadyExists = true) {
            if (navigationItem.LeftBarButtonItem == null)
                navigationItem.LeftBarButtonItem = item;
            else
                navigationItem.LeftBarButtonItems =
                    skipIfAlreadyExists ?
                    navigationItem.LeftBarButtonItems.Union(item).ToArray() :  
                    Tools.Array.ConcatArrays(navigationItem.LeftBarButtonItems, item);
        }


    }
}

