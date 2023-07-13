// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.IO;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

// There is an issue dealing with flags that have value '0'. How is such a flag to be interpreted?
public partial class FlagsCheckedBoxListScreen : ApplicationScreen {
	private readonly TextWriter _textWriter;

	public FlagsCheckedBoxListScreen() {
		InitializeComponent();
		_flagsCheckedListBox.EnumType = typeof(Test1Enum);
		var selectedEnum = _flagsCheckedListBox.SelectedEnum;
		_textWriter = new TextBoxWriter(_outputTextBox);
		_textWriter.WriteLine("Initial value: {0}", selectedEnum != null ? ((Test1Enum)selectedEnum).ToString() : string.Empty);
	}

	private void _flagsCheckedListBox_SelectedValueChanged(object sender, EventArgs e) {
		var selectedEnum = _flagsCheckedListBox.SelectedEnum;
		_textWriter.WriteLine("Selected Value Changed: {0}", selectedEnum != null ? ((Test1Enum)selectedEnum).ToString() : string.Empty);
	}

	private void _flagsCheckedListBox_SelectedFlagChanged(FlagsCheckedListBox arg) {
		var selectedEnum = _flagsCheckedListBox.SelectedEnum;
		_textWriter.WriteLine("Selected Flag Changed: {0}", selectedEnum != null ? ((Test1Enum)selectedEnum).ToString() : string.Empty);
	}


	[Flags]
	private enum Test1Enum {
		Zero = 0,
		Flag1 = 1 << 0,
		Flag2 = 1 << 1,
		Flag3 = 1 << 2,
		[Description("Flags 1 & 2")] Flag1And2 = Flag1 | Flag2,
		All = Flag1 | Flag2 | Flag3,
		Default = Flag1 | Flag3,
		//[Description("All (with Zero)")]
		//SuperAll = All | Zero
	}

}
