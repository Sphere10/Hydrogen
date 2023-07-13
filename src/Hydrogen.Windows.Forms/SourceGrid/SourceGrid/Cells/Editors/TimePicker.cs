// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Windows.Forms;


namespace SourceGrid.Cells.Editors;

/// <summary>
/// Create an Editor that use a DateTimePicker as control for time editing.
/// </summary>
[System.ComponentModel.ToolboxItem(false)]
public class TimePicker : DateTimePicker {
	/// <summary>
	/// Constructor
	/// </summary>
	public TimePicker() : this("T", new string[] { "T" }) {
	}

	/// <summary>
	/// Constructor
	/// </summary>
	public TimePicker(System.String toStringFormat, string[] p_ParseFormats) {
		DevAge.ComponentModel.Converter.DateTimeTypeConverter timeConverter = new DevAge.ComponentModel.Converter.DateTimeTypeConverter(toStringFormat, p_ParseFormats);
		TypeConverter = timeConverter;
	}

	#region Edit Control

	/// <summary>
	/// Create the editor control
	/// </summary>
	/// <returns></returns>
	protected override Control CreateControl() {
		System.Windows.Forms.DateTimePicker dtPicker = new System.Windows.Forms.DateTimePicker();
		dtPicker.Format = DateTimePickerFormat.Time;
		dtPicker.ShowUpDown = true;
		return dtPicker;
	}

	/// <summary>
	/// Gets the control used for editing the cell.
	/// </summary>
	public new System.Windows.Forms.DateTimePicker Control {
		get { return (System.Windows.Forms.DateTimePicker)base.Control; }
	}

	#endregion

}
