// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class CompressionTestScreen : ApplicationScreen {
	private TextBoxWriter _output;
	public CompressionTestScreen() {
		InitializeComponent();
		_output = new TextBoxWriter(textBox1);
	}

	private void _testButton_Click(object sender, EventArgs e) {
		if (File.Exists(fileSelectorControl1.Path)) {
			LibTest(fileSelectorControl1.Path);
			ManualTest1(fileSelectorControl1.Path);
			for (int i = 0; i < 24; i++) {
				ManualTest2(fileSelectorControl1.Path, (int)Math.Pow(2, i));
			}

		}
	}


	void LibTest(string filename) {
		var tempFile = Path.GetTempFileName();
		Tools.FileSystem.CompressFile(filename, tempFile);
		var tempFile2 = Path.GetTempFileName();
		Tools.FileSystem.DecompressFile(tempFile, tempFile2);

		var compressedLength = new FileInfo(tempFile).Length;
		var originalLength = new FileInfo(filename).Length;
		var ratio = (float)compressedLength / (float)originalLength;
		_output.WriteLine("Sphere 10 Library test - ratio {0:P}, original {1}, compressed {2}, uncompresed correctly = {3}", ratio, originalLength, compressedLength, File.ReadAllBytes(filename).SequenceEqual(File.ReadAllBytes(tempFile2)));
		File.Delete(tempFile);
		File.Delete(tempFile2);
	}


	void ManualTest1(string filename) {
		var tempFile = Path.GetTempFileName();
		using (var fileStream = File.Open(tempFile, FileMode.OpenOrCreate)) {
			using (var stream = new GZipStream(fileStream, CompressionMode.Compress)) {
				// Write to the `stream` here and the result will be compressed
				var readBytes = File.ReadAllBytes(filename);
				stream.Write(readBytes, 0, readBytes.Length);
			}
		}
		var compressedLength = new FileInfo(tempFile).Length;
		var originalLength = new FileInfo(filename).Length;
		var ratio = (float)compressedLength / (float)originalLength;
		_output.WriteLine("Manual Test - ratio {0:P}, original {1}, compressed {2}, write blocksize = full", ratio, originalLength, compressedLength);
		File.Delete(tempFile);
	}

	void ManualTest2(string filename, int blockSize) {
		var tempFile = Path.GetTempFileName();
		using (var fileStream = File.Open(tempFile, FileMode.OpenOrCreate)) {
			using (var stream = new GZipStream(fileStream, CompressionMode.Compress)) {
				using (var readStream = File.OpenRead(filename)) {
					Tools.Streams.RouteStream(readStream, stream, blockSize, false, false);
				}
			}
		}

		var compressedLength = new FileInfo(tempFile).Length;
		var originalLength = new FileInfo(filename).Length;
		var ratio = (float)compressedLength / (float)originalLength;
		_output.WriteLine("Manual Test - ratio {0:P}, original {1}, compressed {2}, write blocksize = {3}", ratio, originalLength, compressedLength, blockSize);
		File.Delete(tempFile);
	}


}
