//-----------------------------------------------------------------------
// <copyright file="UITableViewControllerWithBugFix.cs" company="Sphere 10 Software">
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
	public class UITableViewControllerWithBugFix : UITableViewController {
		public UITableViewControllerWithBugFix(UITableViewStyle withStyle) : base(withStyle) {
		}
			
		public override void ViewWillLayoutSubviews() {
			if (TableView.ContentInset.Top != 0.0f)
				TableView.ContentInset = UIEdgeInsets.Zero;
			if (TableView.ScrollIndicatorInsets.Top != 0.0f)
				TableView.ScrollIndicatorInsets = UIEdgeInsets.Zero;

			base.ViewWillLayoutSubviews();
		}
	}
}

