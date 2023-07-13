// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Windows.Forms;
using Hydrogen.FastReflection;


namespace SourceGrid.Cells.Editors;

/// <summary>
/// Editor for a ComboBox (using DevAgeComboBox control)
/// </summary>
[System.ComponentModel.ToolboxItem(false)]
public class DropDownList : EditorControlBase {
	private readonly string _displayMember;

	#region Constructor

	/// <summary>
	/// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
	/// </summary>
	/// <param name="p_Type">The type of this model</param>
	public DropDownList(Type p_Type, string displayMember = null) : base(p_Type) {
		_displayMember = displayMember;
	}

	/// <summary>
	/// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
	/// </summary>
	/// <param name="p_Type">The type of this model</param>
	/// <param name="p_StandardValues"></param>
	/// <param name="p_StandardValueExclusive">True to not allow custom value, only the values specified in the standardvalues collection are allowed.</param>
	public DropDownList(Type p_Type, ICollection p_StandardValues, bool p_StandardValueExclusive)
		: base(p_Type) {
		StandardValues = p_StandardValues;
		StandardValuesExclusive = p_StandardValueExclusive;
	}

	#endregion

	#region Edit Control

	internal override void InternalStartEdit(CellContext cellContext) {
		base.InternalStartEdit(cellContext);
		SendKeys.Flush();
		SendKeys.Send("{F4}");
	}


	/// <summary>
	/// Create the editor control
	/// </summary>
	/// <returns></returns>
	protected override Control CreateControl() {
		DevAge.Windows.Forms.DevAgeComboBox editor = new DevAge.Windows.Forms.DevAgeComboBox();
		editor.DropDownStyle = ComboBoxStyle.DropDownList;
		//editor.FlatStyle = FlatStyle.System;
		editor.Validator = this;

		//NOTE: I have changed a little the ArrangeLinkedControls to support ComboBox control
		editor.SelectedIndexChanged += (o, e) => {
			// Lose the focus
			Control.Enabled = false;
			Control.Enabled = true;
		};
		return editor;
	}

	/// <summary>
	/// Gets the control used for editing the cell.
	/// </summary>
	public new DevAge.Windows.Forms.DevAgeComboBox Control {
		get { return (DevAge.Windows.Forms.DevAgeComboBox)base.Control; }
	}

	#endregion

	/// <summary>
	/// Set the specified value in the current editor control.
	/// </summary>
	/// <param name="editValue"></param>
	public override void SetEditValue(object editValue) {
		if (editValue is string && IsStringConversionSupported() &&
		    Control.DropDownStyle == ComboBoxStyle.DropDown) {
			Control.SelectedIndex = -1;
			Control.Text = (string)editValue;
			Control.SelectionLength = 0;
			if (Control.Text != null)
				Control.SelectionStart = Control.Text.Length;
			else
				Control.SelectionStart = 0;
		} else {
			Control.SelectedIndex = -1;
			Control.Value = editValue;
			Control.SelectAll();
		}
	}


	protected override void OnConvertingValueToDisplayString(DevAge.ComponentModel.ConvertingObjectEventArgs e) {
		if (!string.IsNullOrEmpty(_displayMember) && e.Value != null) {
			e.Value = e.Value.GetType().GetProperty(_displayMember).FastGetValue(e.Value);
		} else {
			base.OnConvertingValueToDisplayString(e);
		}
	}


	/// <summary>
	/// Returns the value inserted with the current editor control
	/// </summary>
	/// <returns></returns>
	public override object GetEditedValue() {
		return Control.Value;
	}

	protected override void OnSendCharToEditor(char key) {
		if (Control.DropDownStyle == ComboBoxStyle.DropDown) {
			Control.Text = key.ToString();
			if (Control.Text != null)
				Control.SelectionStart = Control.Text.Length;
		}
	}

	public override bool IsStringConversionSupported() {
		return false;
	}


}
