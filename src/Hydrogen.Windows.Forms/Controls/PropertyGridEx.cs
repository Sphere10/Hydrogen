// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class PropertyGridEx : PropertyGrid {
	private bool _readonly;
	public PropertyGridEx() {
//		this.ToolbarVisible = false; // categories need to be always visible
	}

	[Category("Behavior")]
	[Description("Sets the grid to readonly mode")]
	[DefaultValue(false)]
	public bool Readonly {
		get => _readonly;
		set { _readonly = value; }
	}

	protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e) {
		if (e.NewSelection.GridItemType == GridItemType.Property) {

			this.SelectedGridItem = null;
			//if (e.NewSelection.Parent != null && e.NewSelection.Parent.GridItemType == GridItemType.Category) {
			//	this.SelectedGridItem = e.NewSelection.Parent;
			//	return;
			//}
		}
	}
}
