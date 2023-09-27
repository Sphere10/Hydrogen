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
using System.Drawing.Drawing2D;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that contains system defined settings for TaskPanes
/// </summary>
public class TaskPaneInfo : IDisposable {

	#region Class Data

	/// <summary>
	/// The starting Color for the TaskPane's background gradient
	/// </summary>
	private Color gradientStartColor;

	/// <summary>
	/// The ending Color for the TaskPane's background gradient
	/// </summary>
	private Color gradientEndColor;

	/// <summary>
	/// The direction of the TaskPane's gradient background
	/// </summary>
	private LinearGradientMode direction;

	/// <summary>
	/// The amount of space between the Border and Expandos along 
	/// each edge of the TaskPane
	/// </summary>
	private PaddingEx padding;

	/// <summary>
	/// The Image that is used as the TaskPane's background
	/// </summary>
	private Image backImage;

	/// <summary>
	/// Specified how the TaskPane's background Image is drawn
	/// </summary>
	private ImageStretchMode stretchMode;

	/// <summary>
	/// The Image that is used as a watermark
	/// </summary>
	private Image watermark;

	/// <summary>
	/// The alignment of the Image used as a watermark
	/// </summary>
	private ContentAlignment watermarkAlignment;

	/// <summary>
	/// The TaskPane that owns the TaskPaneInfo
	/// </summary>
	private TaskPane owner;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the TaskPaneInfo class with default settings
	/// </summary>
	public TaskPaneInfo() {
		// set background values
		this.gradientStartColor = Color.Transparent;
		this.gradientEndColor = Color.Transparent;
		this.direction = LinearGradientMode.Vertical;

		// set padding values
		this.padding = new PaddingEx(12, 12, 12, 12);

		// images
		this.backImage = null;
		this.stretchMode = ImageStretchMode.Tile;

		this.watermark = null;
		this.watermarkAlignment = ContentAlignment.BottomCenter;

		this.owner = null;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Forces the use of default values
	/// </summary>
	public void SetDefaultValues() {
		// set background values
		this.gradientStartColor = SystemColors.Window;
		this.gradientEndColor = SystemColors.Window;
		this.direction = LinearGradientMode.Vertical;

		// set padding values
		this.padding.Left = 12;
		this.padding.Top = 12;
		this.padding.Right = 12;
		this.padding.Bottom = 12;

		// images
		this.backImage = null;
		this.stretchMode = ImageStretchMode.Tile;
		this.watermark = null;
		this.watermarkAlignment = ContentAlignment.BottomCenter;
	}


	/// <summary>
	/// Forces the use of default empty values
	/// </summary>
	public void SetDefaultEmptyValues() {
		// set background values
		this.gradientStartColor = Color.Empty;
		this.gradientEndColor = Color.Empty;
		this.direction = LinearGradientMode.Vertical;

		// set padding values
		this.padding.Left = 0;
		this.padding.Top = 0;
		this.padding.Right = 0;
		this.padding.Bottom = 0;

		// images
		this.backImage = null;
		this.stretchMode = ImageStretchMode.Tile;
		this.watermark = null;
		this.watermarkAlignment = ContentAlignment.BottomCenter;
	}


	/// <summary>
	/// Releases all resources used by the TaskPaneInfo
	/// </summary>
	public void Dispose() {
		if (this.backImage != null) {
			this.backImage.Dispose();
			this.backImage = null;
		}

		if (this.watermark != null) {
			this.watermark.Dispose();
			this.watermark = null;
		}
	}

	#endregion


	#region Properties

	#region Background

	/// <summary>
	/// Gets or sets the TaskPane's first gradient background color
	/// </summary>
	[Description("The TaskPane's first gradient background color")]
	public Color GradientStartColor {
		get { return this.gradientStartColor; }

		set {
			if (this.gradientStartColor != value) {
				this.gradientStartColor = value;

				if (this.TaskPane != null) {
					this.TaskPane.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the GradientStartColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the GradientStartColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeGradientStartColor() {
		return this.GradientStartColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the TaskPane's second gradient background color
	/// </summary>
	[Description("The TaskPane's second gradient background color")]
	public Color GradientEndColor {
		get { return this.gradientEndColor; }

		set {
			if (this.gradientEndColor != value) {
				this.gradientEndColor = value;

				if (this.TaskPane != null) {
					this.TaskPane.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the GradientEndColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the GradientEndColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeGradientEndColor() {
		return this.GradientEndColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the direction of the TaskPane's gradient
	/// </summary>
	[DefaultValue(LinearGradientMode.Vertical),
	 Description("The direction of the TaskPane's background gradient")]
	public LinearGradientMode GradientDirection {
		get { return this.direction; }

		set {
			if (!Enum.IsDefined(typeof(LinearGradientMode), value)) {
				throw new InvalidEnumArgumentException("value", (int)value, typeof(LinearGradientMode));
			}

			if (this.direction != value) {
				this.direction = value;

				if (this.TaskPane != null) {
					this.TaskPane.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}

	#endregion

	#region Images

	/// <summary>
	/// Gets or sets the Image that is used as the TaskPane's background
	/// </summary>
	[DefaultValue(null),
	 Description("The Image that is used as the TaskPane's background")]
	public Image BackImage {
		get { return this.backImage; }

		set {
			if (this.backImage != value) {
				this.backImage = value;

				if (this.TaskPane != null) {
					this.TaskPane.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets how the TaskPane's background Image is drawn
	/// </summary>
	[Browsable(false),
	 DefaultValue(ImageStretchMode.Tile),
	 Description("Specifies how the TaskPane's background Image is drawn")]
	public ImageStretchMode StretchMode {
		get { return this.stretchMode; }

		set {
			if (!Enum.IsDefined(typeof(ImageStretchMode), value)) {
				throw new InvalidEnumArgumentException("value", (int)value, typeof(ImageStretchMode));
			}

			if (this.stretchMode != value) {
				this.stretchMode = value;

				if (this.TaskPane != null) {
					this.TaskPane.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets the Image that is used as the TaskPane's watermark
	/// </summary>
	[DefaultValue(null),
	 Description("The Image that is used as the TaskPane's watermark")]
	public Image Watermark {
		get { return this.watermark; }

		set {
			if (this.watermark != value) {
				this.watermark = value;

				if (this.TaskPane != null) {
					this.TaskPane.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets the alignment of the Image that is used as the 
	/// TaskPane's watermark
	/// </summary>
	[DefaultValue(ContentAlignment.BottomCenter),
	 Description("The alignment of the Image that is used as the TaskPane's watermark")]
	public ContentAlignment WatermarkAlignment {
		get { return this.watermarkAlignment; }

		set {
			if (!Enum.IsDefined(typeof(ContentAlignment), value)) {
				throw new InvalidEnumArgumentException("value", (int)value, typeof(ContentAlignment));
			}

			if (this.watermarkAlignment != value) {
				this.watermarkAlignment = value;

				if (this.TaskPane != null) {
					this.TaskPane.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}

	#endregion

	#region Padding

	/// <summary>
	/// Gets or sets the TaskPane's padding between the border and any items
	/// </summary>
	[Description("The amount of space between the border and the Expando's along each side of the TaskPane")]
	public PaddingEx Padding {
		get {
			var isZero = this.padding.Left;
			var isNotZero = this.padding.Left;
			return this.padding;
		}

		set {
			var isZero = value.Left;
			var isNotZero = value.Left;
			if (this.padding != value) {
				this.padding = value;

				if (this.TaskPane != null) {
					this.TaskPane.FireCustomSettingsChanged(EventArgs.Empty);
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

	#region TaskPane

	/// <summary>
	/// Gets or sets the TaskPane the TaskPaneInfo belongs to
	/// </summary>
	protected internal TaskPane TaskPane {
		get { return this.owner; }

		set { this.owner = value; }
	}

	#endregion

	#endregion


}
