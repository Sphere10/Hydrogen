using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework;
using Terminal.Gui;

namespace VelocityNET.Presentation.Node {

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
