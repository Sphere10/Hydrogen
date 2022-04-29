//-----------------------------------------------------------------------
// <copyright file="RegionToolTestForm.cs" company="Sphere 10 Software">
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework;
using Sphere10.Framework.Windows.Forms;

namespace Sphere10.FrameworkTester.WinForms {
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
					_outputTextWriter.WriteLine("{0}: {1} - {2} - {3}", region.DisplayName, region.Name,  region.TwoLetterISORegionName, region.ThreeLetterISORegionName);

				} catch (Exception error) {
					_outputTextWriter.WriteLine("Error: {0}", error.ToDisplayString());
				}
			}

		}
	}

}

