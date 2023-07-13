// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Globalization;
using System.IO;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class RegionToolTestScreen : ApplicationScreen {
	private readonly TextWriter _outputTextWriter;

	public RegionToolTestScreen() {
		InitializeComponent();
		_outputTextWriter = new TextBoxWriter(_outputTextBox);
	}

	private void button1_Click(object sender, EventArgs e) {
		foreach (var culture in CultureInfo.GetCultures(CultureTypes.FrameworkCultures)) {
			if (culture.IsNeutralCulture)
				_outputTextWriter.WriteLine("NETRUAL {0}: {1}", culture.EnglishName, culture.TwoLetterISOLanguageName);
			else
				try {
					var region = new RegionInfo(culture.LCID);
					_outputTextWriter.WriteLine("{0}: {1} - {2} - {3}", region.DisplayName, region.Name, region.TwoLetterISORegionName, region.ThreeLetterISORegionName);

				} catch (Exception error) {
					_outputTextWriter.WriteLine("Error: {0}", error.ToDisplayString());
				}
		}

	}
}
