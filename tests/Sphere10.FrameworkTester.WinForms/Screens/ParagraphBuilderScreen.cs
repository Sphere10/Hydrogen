//-----------------------------------------------------------------------
// <copyright file="ParagraphBuilderForm.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework;
using Sphere10.Framework.Windows.Forms;

namespace Sphere10.FrameworkTester.WinForms {
	public partial class ParagraphBuilderScreen : ApplicationScreen {
		public ParagraphBuilderScreen() {
			InitializeComponent();
			Builder = new ParagraphBuilder();
		}

		public ParagraphBuilder Builder { get; set; }

		private void _appendSentenceButton_Click(object sender, EventArgs e) {
			Builder.AppendSentence(_sentenceTextBox.Text);
			RefreshParagraphTextBox();
		}

		private void _appendBreakButton_Click(object sender, EventArgs e) {
			Builder.AppendParagraphBreak();
			RefreshParagraphTextBox();
		}

		private void RefreshParagraphTextBox() {
			_paragraphTextBox.Text = Builder.ToString();
		}
	}
}
