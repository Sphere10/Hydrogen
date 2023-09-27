// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class BloomFilterAnalysisScreen : ApplicationScreen {
	private readonly TextWriter _writer;

	public BloomFilterAnalysisScreen() {
		InitializeComponent();
		_writer = new TextBoxWriter(_standardTextBox);
	}

	private void _metricsTableButton_Click(object sender, EventArgs e) {
		_writer.WriteLine("Bloom Filter Metrics Table");
		_writer.WriteLine("Expected Items\tBloom Mask Size\tTarget Error\tFilter Size\tExclusion Filter Size");
		foreach (ulong expectedItems in new[] { 1024L, 1048576L, 1073741824L, 1099511627776L }) {
			foreach (var maskSize in new[] { 3, 4, 5 }) {
				foreach (var targetError in new[] { 0.01M, 0.02M, 0.03M, 0.05M, 0.25M }) {
					var filterLengthBytes = BloomFilterMath.EstimateBloomFilterLength(targetError, (long)expectedItems, maskSize) / 8;
					var exclusionFilterLengthBytes = BloomFilterMath.EstimateBloomFilterLength(targetError, (long)expectedItems, maskSize) * 8;
					_writer.WriteLine($"{expectedItems}\t{maskSize}\t{targetError}\t{Tools.Memory.GetBytesReadable(filterLengthBytes)}\t{Tools.Memory.GetBytesReadable(exclusionFilterLengthBytes)}");
				}
			}
		}
	}
}
