using System;

namespace VelocityNET.Presentation.Node {

	public class TitleAttribute : Attribute {
		public TitleAttribute(string title) {
			Title = title;
		}

		public string Title { get; private set; }
	}

}
