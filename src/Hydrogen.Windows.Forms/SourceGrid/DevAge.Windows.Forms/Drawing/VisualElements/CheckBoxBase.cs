// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace DevAge.Drawing.VisualElements;

public interface ICheckBox : IVisualElement {
	ControlDrawStyle Style { get; set; }

	CheckBoxState CheckBoxState { get; set; }
}


[Serializable]
public abstract class CheckBoxBase : VisualElementBase, ICheckBox {

	#region Constuctor

	/// <summary>
	/// Default constructor
	/// </summary>
	public CheckBoxBase() {
		AnchorArea = new AnchorArea(float.NaN, float.NaN, float.NaN, float.NaN, true, true);
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public CheckBoxBase(CheckBoxBase other)
		: base(other) {
		Style = other.Style;
		CheckBoxState = other.CheckBoxState;
	}

	#endregion

	#region Properties

	private ControlDrawStyle mControlDrawStyle = ControlDrawStyle.Normal;

	public virtual ControlDrawStyle Style {
		get { return mControlDrawStyle; }
		set { mControlDrawStyle = value; }
	}

	protected virtual bool ShouldSerializeStyle() {
		return Style != ControlDrawStyle.Normal;
	}

	private CheckBoxState mCheckBoxState = CheckBoxState.Undefined;

	public virtual CheckBoxState CheckBoxState {
		get { return mCheckBoxState; }
		set { mCheckBoxState = value; }
	}

	protected virtual bool ShouldSerializeCheckBoxState() {
		return CheckBoxState != CheckBoxState.Undefined;
	}

	#endregion

}
