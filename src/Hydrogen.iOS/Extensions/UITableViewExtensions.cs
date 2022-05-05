//-----------------------------------------------------------------------
// <copyright file="UITableViewExtensions.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS {

    public static class UITableViewExtensions {

        public static void DeselectAllRows(this UITableView tableView, bool animated) {
            foreach (var item in tableView.IndexPathsForSelectedRows) {
                tableView.DeselectRow(item, animated);
            }
        }

    }
}

