//-----------------------------------------------------------------------
// <copyright file="NSTableColumnExtensions.cs" company="Sphere 10 Software">
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
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Hydrogen {
	public static class NSTableViewColumnExtensions
	{
		public static int GetIndex(this NSTableColumn tableColumn) {
			int index = -1;
			if (tableColumn.TableView != null) {
				var columns = tableColumn.TableView.TableColumns();
				for (int i = 0; i< columns.Length; i++) {
					if (tableColumn == columns[i]) {
						index = i;
						break;
					}
				}
			}
			return index;
		}


	}
}

