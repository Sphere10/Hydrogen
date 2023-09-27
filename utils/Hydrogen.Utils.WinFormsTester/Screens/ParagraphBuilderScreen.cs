// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

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
