//-----------------------------------------------------------------------
// <copyright file="IntBox.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;

namespace Sphere10.Framework.Windows.Forms {

	[ToolboxItem(true)]
	public class IntBox : NumericBoxBase<int> {

		protected override bool CanParse(string text) {
			int buf;
			return int.TryParse(
				this.Text,
				NumberStyles.Any,
				CultureInfo.CurrentUICulture,
				out buf);
		}

		protected override int? DoParse(string text) {
			int buf;
			if (int.TryParse(
				this.Text,
				NumberStyles.Any,
				CultureInfo.CurrentUICulture,
				out buf)) {
				return buf;
			}
			return null;
		}

		protected override string ToText(int value) {
			return value.ToString(this.FormatString);
		}

	}
}
