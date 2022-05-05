using Hydrogen;
using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI {

	public class TextViewWriter : BaseTextWriter {
		private readonly TextView _textBox;

		public TextViewWriter(TextView textBox) {
			_textBox = textBox;
		}

		protected override void InternalWrite(string value) {
			_textBox.Text += value;
			_textBox.SetNeedsDisplay();
		}
	}

}
