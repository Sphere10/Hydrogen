// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.ComponentModel;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that contains system defined settings for TaskItems
/// </summary>
public class TaskItemInfo {

	#region Class Data

	/// <summary>
	/// The amount of space around the text along each side of 
	/// the TaskItem
	/// </summary>
	private PaddingEx padding;

	/// <summary>
	/// The amount of space between individual TaskItems 
	/// along each side of the TaskItem
	/// </summary>
	private Margin margin;

	/// <summary>
	/// The Color of the text displayed in the TaskItem
	/// </summary>
	private Color linkNormal;

	/// <summary>
	/// The Color of the text displayed in the TaskItem when 
	/// highlighted
	/// </summary>
	private Color linkHot;

	/// <summary>
	/// The decoration to be used on the text while in a highlighted state
	/// </summary>
	private FontStyle fontDecoration;

	/// <summary>
	/// The TaskItem that owns this TaskItemInfo
	/// </summary>
	private TaskItem owner;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the TaskLinkInfo class with default settings
	/// </summary>
	public TaskItemInfo() {
		// set padding values
		this.padding = new PaddingEx(6, 0, 4, 0);

		// set margin values
		this.margin = new Margin(0, 4, 0, 0);

		// set text values
		this.linkNormal = SystemColors.ControlText;
		this.linkHot = SystemColors.ControlText;

		this.fontDecoration = FontStyle.Underline;

		this.owner = null;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Forces the use of default values
	/// </summary>
	public void SetDefaultValues() {
		// set padding values
		this.padding.Left = 6;
		this.padding.Top = 0;
		this.padding.Right = 4;
		this.padding.Bottom = 0;

		// set margin values
		this.margin.Left = 0;
		this.margin.Top = 4;
		this.margin.Right = 0;
		this.margin.Bottom = 0;

		// set text values
		this.linkNormal = SystemColors.ControlText;
		this.linkHot = SystemColors.HotTrack;

		this.fontDecoration = FontStyle.Underline;
	}


	/// <summary>
	/// Forces the use of default empty values
	/// </summary>
	public void SetDefaultEmptyValues() {
		this.padding = PaddingEx.Empty;
		this.margin = Margin.Empty;
		this.linkNormal = Color.Empty;
		this.linkHot = Color.Empty;
		this.fontDecoration = FontStyle.Underline;
	}

	#endregion


	#region Properties

	#region Margin

	/// <summary>
	/// Gets or sets the amount of space between individual TaskItems 
	/// along each side of the TaskItem
	/// </summary>
	[Description("The amount of space between individual TaskItems along each side of the TaskItem")]
	public Margin Margin {
		get { return this.margin; }

		set {
			if (this.margin != value) {
				this.margin = value;

				if (this.TaskItem != null) {
					this.TaskItem.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the Margin property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the Margin property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeMargin() {
		return this.Margin != Margin.Empty;
	}

	#endregion

	#region Padding

	/// <summary>
	/// Gets or sets the amount of space around the text along each 
	/// side of the TaskItem
	/// </summary>
	[Description("The amount of space around the text along each side of the TaskItem")]
	public PaddingEx Padding {
		get { return this.padding; }

		set {
			if (this.padding != value) {
				this.padding = value;

				if (this.TaskItem != null) {
					this.TaskItem.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the Padding property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the Padding property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializePadding() {
		return this.Padding != PaddingEx.Empty;
	}

	#endregion

	#region Text

	/// <summary>
	/// Gets or sets the foreground color of a normal link
	/// </summary>
	[Description("The foreground color of a normal link")]
	public Color LinkColor {
		get { return this.linkNormal; }

		set {
			if (this.linkNormal != value) {
				this.linkNormal = value;

				if (this.TaskItem != null) {
					this.TaskItem.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the LinkColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the LinkColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeLinkColor() {
		return this.LinkColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the foreground color of a highlighted link
	/// </summary>
	[Description("The foreground color of a highlighted link")]
	public Color HotLinkColor {
		get { return this.linkHot; }

		set {
			if (this.linkHot != value) {
				this.linkHot = value;

				if (this.TaskItem != null) {
					this.TaskItem.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the HotLinkColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the HotLinkColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeHotLinkColor() {
		return this.HotLinkColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the font decoration of a link
	/// </summary>
	[DefaultValue(FontStyle.Underline),
	 Description("")]
	public FontStyle FontDecoration {
		get { return this.fontDecoration; }

		set {
			if (!Enum.IsDefined(typeof(FontStyle), value)) {
				throw new InvalidEnumArgumentException("value", (int)value, typeof(FontStyle));
			}

			if (this.fontDecoration != value) {
				this.fontDecoration = value;

				if (this.TaskItem != null) {
					this.TaskItem.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}

	#endregion

	#region TaskItem

	/// <summary>
	/// Gets or sets the TaskItem the TaskItemInfo belongs to
	/// </summary>
	protected internal TaskItem TaskItem {
		get { return this.owner; }

		set { this.owner = value; }
	}

	#endregion

	#endregion

}
