using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		set {
			_readonly = value;
		}
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
