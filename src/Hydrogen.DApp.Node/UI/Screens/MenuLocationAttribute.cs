using System;

namespace Hydrogen.DApp.Node.UI {

	public class MenuLocationAttribute : Attribute {
		
		public MenuLocationAttribute(AppMenu menu, string name, int preferredIndex = -1) {
			Menu = menu;
			Name = name;
			PreferredIndex = preferredIndex;
		}

		public AppMenu Menu { get; private set; }

		public string Name { get; private set; }

		public int? PreferredIndex { get; private set; }
	}

}
