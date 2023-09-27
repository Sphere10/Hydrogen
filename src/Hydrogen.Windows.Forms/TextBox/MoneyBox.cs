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
public class MoneyBox : NumericBoxBase<decimal> {

	private const string DEFAULT_FORMATSTRING = "C";

	public MoneyBox() {
		this.FormatString = DEFAULT_FORMATSTRING;
	}

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

	[DefaultValue(DEFAULT_FORMATSTRING)]
	[Category("Appearance")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public override string FormatString {
		get { return base.FormatString; }
		set { base.FormatString = value; }
	}


}
