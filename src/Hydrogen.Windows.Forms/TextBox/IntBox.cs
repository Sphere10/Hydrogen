// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using System.Globalization;

namespace Hydrogen.Windows.Forms;

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
