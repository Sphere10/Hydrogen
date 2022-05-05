using System;

namespace Hydrogen.DApp.Node.UI {

	public class TitleAttribute : Attribute {
		public TitleAttribute(string title) {
			Title = title;
		}

		public string Title { get; private set; }
	}

}
