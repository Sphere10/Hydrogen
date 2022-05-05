//-----------------------------------------------------------------------
// <copyright file="StandardTableViewDelegate.cs" company="Sphere 10 Software">
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
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Hydrogen {
	public class StandardTableViewDelegate : NSTableViewDelegate {

		public StandardTableViewDelegate() {

		}

		public override bool ShouldSelectRow(NSTableView tableView, int row) {
			return true;
		}

		public override bool ShouldSelectTableColumn(NSTableView tableView, NSTableColumn tableColumn) {
			return false;
		}

		public override bool ShouldEditTableColumn(NSTableView tableView, NSTableColumn tableColumn, int row) {
			return false;
		}


			
	}

}

