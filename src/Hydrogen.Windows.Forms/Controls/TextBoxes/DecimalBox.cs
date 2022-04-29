//-----------------------------------------------------------------------
// <copyright file="DecimalBox.cs" company="Sphere 10 Software">
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
	public class DecimalBox : NumericBoxBase<decimal> {

		protected override bool CanParse(string text) {
			decimal buf;
			return decimal.TryParse(
				this.Text,
				NumberStyles.Any,
				CultureInfo.CurrentUICulture,
				out buf);
		}

		protected override decimal? DoParse(string text) {
			decimal buf;
			if (decimal.TryParse(
				this.Text,
				NumberStyles.Any,
				CultureInfo.CurrentUICulture,
				out buf)) {
				return buf;
			}
			return null;
		}

		protected override string ToText(decimal value) {
			return value.ToString(this.FormatString);
		}

	}
}
