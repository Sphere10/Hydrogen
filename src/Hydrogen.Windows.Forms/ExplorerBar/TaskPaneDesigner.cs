using System.Windows.Forms.Design;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A custom designer used by TaskPanes to remove unwanted 
/// properties from the Property window in the designer
/// </summary>
internal class TaskPaneDesigner : ScrollableControlDesigner {
	/// <summary>
	/// Initializes a new instance of the TaskPaneDesigner class
	/// </summary>
	public TaskPaneDesigner()
		: base() {

	}


	/// <summary>
	/// Adjusts the set of properties the component exposes through 
	/// a TypeDescriptor
	/// </summary>
	/// <param name="properties">An IDictionary containing the properties 
	/// for the class of the component</param>
	protected override void PreFilterProperties(System.Collections.IDictionary properties) {
		base.PreFilterProperties(properties);

		properties.Remove("BackColor");
		properties.Remove("BackgroundImage");
		properties.Remove("Cursor");
		properties.Remove("ForeColor");
	}
}
