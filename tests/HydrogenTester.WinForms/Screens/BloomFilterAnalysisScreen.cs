//-----------------------------------------------------------------------
// <copyright file="TextAreaTests.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Windows.Forms;
using Sphere10.Framework;
using Sphere10.Framework.Data;
using Sphere10.Framework.Windows;
using Sphere10.Framework.Windows.Forms;
using Tuple = System.Tuple;

namespace Sphere10.FrameworkTester.WinForms {
	public partial class BloomFilterAnalysisScreen : ApplicationScreen {
		private readonly TextWriter _writer;
		
		public BloomFilterAnalysisScreen() {
			InitializeComponent();
			_writer = new TextBoxWriter(_standardTextBox);
		}
		
		private void _metricsTableButton_Click(object sender, EventArgs e) {
			_writer.WriteLine("Bloom Filter Metrics Table");
			_writer.WriteLine("Expected Items\tBloom Mask Size\tTarget Error\tFilter Size\tExclusion Filter Size");
			foreach (ulong expectedItems in new[] {1024L, 1048576L, 1073741824L, 1099511627776L}) {
				foreach (var maskSize in new[] {3,4,5}) {
					foreach (var targetError in new[] { 0.01M, 0.02M, 0.03M, 0.05M, 0.25M }) {
						var filterLengthBytes = BloomFilterMath.EstimateBloomFilterLength(targetError, (long)expectedItems, maskSize) / 8;
						var exclusionFilterLengthBytes = BloomFilterMath.EstimateBloomFilterLength(targetError, (long)expectedItems, maskSize) * 8;
						_writer.WriteLine($"{expectedItems}\t{maskSize}\t{targetError}\t{Tools.Memory.GetBytesReadable(filterLengthBytes)}\t{Tools.Memory.GetBytesReadable(exclusionFilterLengthBytes)}");
					}
				}
			}
		}
	}
}
