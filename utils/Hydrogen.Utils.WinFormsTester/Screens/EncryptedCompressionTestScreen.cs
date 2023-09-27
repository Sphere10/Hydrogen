// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class EncryptedCompressionTestScreen : ApplicationScreen {
	private readonly TextBoxWriter _output;
	public EncryptedCompressionTestScreen() {
		InitializeComponent();
		_output = new TextBoxWriter(textBox1);
	}

	private void _testButton_Click(object sender, EventArgs e) {
		try {
			if (File.Exists(fileSelectorControl1.Path)) {
				Test(fileSelectorControl1.Path);
			}
		} catch (Exception error) {
			_output.WriteLine(error.ToDiagnosticString());
		}
	}


	void Test(string filename) {
		var tempFile = Path.GetTempFileName();
		Tools.FileSystem.CompressFile(filename, tempFile, _passwordTextBox.Text);
		var tempFile2 = Path.GetTempFileName();
		Tools.FileSystem.DecompressFile(tempFile, tempFile2, _passwordTextBox.Text);
		var compressedLength = new FileInfo(tempFile).Length;
		var originalLength = new FileInfo(filename).Length;
		var ratio = (float)compressedLength / (float)originalLength;
		_output.WriteLine("Ratio {0:P}, original {1}, compressed {2}, uncompresed correctly = {3}", ratio, originalLength, compressedLength, File.ReadAllBytes(filename).SequenceEqual(File.ReadAllBytes(tempFile2)));
		File.Delete(tempFile);
		File.Delete(tempFile2);
	}


}
