using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class TabControlStateEventProvider : ContainerControlStateEventProviderBase<TabControl> {

	protected override IEnumerable<Control> GetChildControls(TabControl control) 
		=> base.GetChildControls(control).Union(control.TabPages.Cast<TabPage>());

}
