// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace DevAge.Drawing.VisualElements;

public interface IButton : IBackground {
	ButtonStyle Style { get; set; }
}


[Serializable]
public abstract class ButtonBase : BackgroundBase, IButton {

	#region Constuctor

	/// <summary>
	/// Default constructor
	/// </summary>
	public ButtonBase() {
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public ButtonBase(ButtonBase other)
		: base(other) {
		Style = other.Style;
	}

	#endregion

	#region Properties

	private ButtonStyle mControlDrawStyle = ButtonStyle.Normal;

	public virtual ButtonStyle Style {
		get { return mControlDrawStyle; }
		set { mControlDrawStyle = value; }
	}

	protected virtual bool ShouldSerializeStyle() {
		return Style != ButtonStyle.Normal;
	}

	#endregion

}
