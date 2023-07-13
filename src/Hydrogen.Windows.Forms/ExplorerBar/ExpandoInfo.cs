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
/// A class that contains system defined settings for Expandos
/// </summary>
public class ExpandoInfo : IDisposable {

	#region Class Data

	/// <summary>
	/// The background Color of an Expando that is a special group
	/// </summary>
	private Color specialBackColor;

	/// <summary>
	/// The background Color of an Expando that is a normal group
	/// </summary>
	private Color normalBackColor;

	/// <summary>
	/// The width of the Border along each edge of an Expando that 
	/// is a special group
	/// </summary>
	private Border specialBorder;

	/// <summary>
	/// The width of the Border along each edge of an Expando that 
	/// is a normal group
	/// </summary>
	private Border normalBorder;

	/// <summary>
	/// The Color of the Border an Expando that is a special group
	/// </summary>
	private Color specialBorderColor;

	/// <summary>
	/// The Color of the Border an Expando that is a normal group
	/// </summary>
	private Color normalBorderColor;

	/// <summary>
	/// The amount of space between the Border and items along 
	/// each edge of an Expando that is a special group
	/// </summary>
	private PaddingEx specialPadding;

	/// <summary>
	/// The amount of space between the Border and items along 
	/// each edge of an Expando that is a normal group
	/// </summary>
	private PaddingEx normalPadding;

	/// <summary>
	/// The alignment of the Image that is to be used as a watermark
	/// </summary>
	private ContentAlignment watermarkAlignment;

	/// <summary>
	/// The background image used for the content area of a special Expando
	/// </summary>
	private Image specialBackImage;

	/// <summary>
	/// The background image used for the content area of a normal Expando
	/// </summary>
	private Image normalBackImage;

	/// <summary>
	/// The Expando that the ExpandoInfo belongs to
	/// </summary>
	private Expando owner;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the ExpandoInfo class with default settings
	/// </summary>
	public ExpandoInfo() {
		// set background color values
		this.specialBackColor = Color.Transparent;
		this.normalBackColor = Color.Transparent;

		// set border values
		this.specialBorder = new Border(1, 0, 1, 1);
		this.specialBorderColor = Color.Transparent;

		this.normalBorder = new Border(1, 0, 1, 1);
		this.normalBorderColor = Color.Transparent;

		// set padding values
		this.specialPadding = new PaddingEx(12, 10, 12, 10);
		this.normalPadding = new PaddingEx(12, 10, 12, 10);

		this.specialBackImage = null;
		this.normalBackImage = null;

		this.watermarkAlignment = ContentAlignment.BottomRight;

		this.owner = null;

	}

	#endregion


	#region Methods

	/// <summary>
	/// Forces the use of default values
	/// </summary>
	public void SetDefaultValues() {
		// set background color values
		this.specialBackColor = SystemColors.Window;
		this.normalBackColor = SystemColors.Window;

		// set border values
		this.specialBorder.Left = 1;
		this.specialBorder.Top = 0;
		this.specialBorder.Right = 1;
		this.specialBorder.Bottom = 1;

		this.specialBorderColor = SystemColors.Highlight;

		this.normalBorder.Left = 1;
		this.normalBorder.Top = 0;
		this.normalBorder.Right = 1;
		this.normalBorder.Bottom = 1;

		this.normalBorderColor = SystemColors.Control;

		// set padding values
		this.specialPadding.Left = 12;
		this.specialPadding.Top = 10;
		this.specialPadding.Right = 12;
		this.specialPadding.Bottom = 10;

		this.normalPadding.Left = 12;
		this.normalPadding.Top = 10;
		this.normalPadding.Right = 12;
		this.normalPadding.Bottom = 10;

		this.specialBackImage = null;
		this.normalBackImage = null;

		this.watermarkAlignment = ContentAlignment.BottomRight;

	}


	/// <summary>
	/// Forces the use of default empty values
	/// </summary>
	public void SetDefaultEmptyValues() {
		// set background color values
		this.specialBackColor = Color.Empty;
		this.normalBackColor = Color.Empty;

		// set border values
		this.specialBorder = Border.Empty;
		this.specialBorderColor = Color.Empty;

		this.normalBorder = Border.Empty;
		this.normalBorderColor = Color.Empty;

		// set padding values
		this.specialPadding = PaddingEx.Empty;
		this.normalPadding = PaddingEx.Empty;

		this.specialBackImage = null;
		this.normalBackImage = null;

		this.watermarkAlignment = ContentAlignment.BottomRight;
	}


	/// <summary>
	/// Releases all resources used by the ExpandoInfo
	/// </summary>
	public void Dispose() {
		if (this.specialBackImage != null) {
			this.specialBackImage.Dispose();
			this.specialBackImage = null;
		}

		if (this.normalBackImage != null) {
			this.normalBackImage.Dispose();
			this.normalBackImage = null;
		}
	}

	#endregion


	#region Properties

	#region Background

	/// <summary>
	/// Gets or sets the background color of a special expando
	/// </summary>
	[Description("The background color of a special Expando")]
	public Color SpecialBackColor {
		get { return this.specialBackColor; }

		set {
			if (this.specialBackColor != value) {
				this.specialBackColor = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the SpecialBackColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the SpecialBackColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSpecialBackColor() {
		return this.SpecialBackColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the background color of a normal expando
	/// </summary>
	[Description("The background color of a normal Expando")]
	public Color NormalBackColor {
		get { return this.normalBackColor; }

		set {
			if (this.normalBackColor != value) {
				this.normalBackColor = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the NormalBackColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the NormalBackColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeNormalBackColor() {
		return this.NormalBackColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the alignment for the expando's background image
	/// </summary>
	[DefaultValue(ContentAlignment.BottomRight),
	 Description("The alignment for the expando's background image")]
	public ContentAlignment WatermarkAlignment {
		get { return this.watermarkAlignment; }

		set {
			if (!Enum.IsDefined(typeof(ContentAlignment), value)) {
				throw new InvalidEnumArgumentException("value", (int)value, typeof(ContentAlignment));
			}

			if (this.watermarkAlignment != value) {
				this.watermarkAlignment = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets a special expando's background image
	/// </summary>
	[DefaultValue(null),
	 Description("")]
	public Image SpecialBackImage {
		get { return this.specialBackImage; }

		set {
			if (this.specialBackImage != value) {
				this.specialBackImage = value;
			}
		}
	}


	/// <summary>
	/// Gets or sets a normal expando's background image
	/// </summary>
	[DefaultValue(null),
	 Description("")]
	public Image NormalBackImage {
		get { return this.normalBackImage; }

		set {
			if (this.normalBackImage != value) {
				this.normalBackImage = value;
			}
		}
	}

	#endregion

	#region Border

	/// <summary>
	/// Gets or sets the border for a special expando
	/// </summary>
	[Description("The width of the Border along each side of a special Expando")]
	public Border SpecialBorder {
		get { return this.specialBorder; }

		set {
			if (this.specialBorder != value) {
				this.specialBorder = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the SpecialBorder property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the SpecialBorder property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSpecialBorder() {
		return this.SpecialBorder != Border.Empty;
	}


	/// <summary>
	/// Gets or sets the border for a normal expando
	/// </summary>
	[Description("The width of the Border along each side of a normal Expando")]
	public Border NormalBorder {
		get { return this.normalBorder; }

		set {
			if (this.normalBorder != value) {
				this.normalBorder = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the NormalBorder property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the NormalBorder property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeNormalBorder() {
		return this.NormalBorder != Border.Empty;
	}


	/// <summary>
	/// Gets or sets the border color for a special expando
	/// </summary>
	[Description("The border color for a special Expando")]
	public Color SpecialBorderColor {
		get { return this.specialBorderColor; }

		set {
			if (this.specialBorderColor != value) {
				this.specialBorderColor = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the SpecialBorderColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the SpecialBorderColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSpecialBorderColor() {
		return this.SpecialBorderColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the border color for a normal expando
	/// </summary>
	[Description("The border color for a normal Expando")]
	public Color NormalBorderColor {
		get { return this.normalBorderColor; }

		set {
			if (this.normalBorderColor != value) {
				this.normalBorderColor = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the NormalBorderColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the NormalBorderColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeNormalBorderColor() {
		return this.NormalBorderColor != Color.Empty;
	}

	#endregion

	#region Padding

	/// <summary>
	/// Gets or sets the padding value for a special expando
	/// </summary>
	[Description("The amount of space between the border and items along each side of a special Expando")]
	public PaddingEx SpecialPadding {
		get { return this.specialPadding; }

		set {
			if (this.specialPadding != value) {
				this.specialPadding = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the SpecialPadding property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the SpecialPadding property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSpecialPadding() {
		return this.SpecialPadding != PaddingEx.Empty;
	}


	/// <summary>
	/// Gets or sets the padding value for a normal expando
	/// </summary>
	[Description("The amount of space between the border and items along each side of a normal Expando")]
	public PaddingEx NormalPadding {
		get { return this.normalPadding; }

		set {
			if (this.normalPadding != value) {
				this.normalPadding = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the NormalPadding property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the NormalPadding property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeNormalPadding() {
		return this.NormalPadding != PaddingEx.Empty;
	}

	#endregion

	#region Expando

	/// <summary>
	/// Gets or sets the Expando that the ExpandoInfo belongs to
	/// </summary>
	protected internal Expando Expando {
		get { return this.owner; }

		set { this.owner = value; }
	}

	#endregion

	#endregion

}
