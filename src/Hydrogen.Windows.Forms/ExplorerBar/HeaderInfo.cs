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
/// A class that contains system defined settings for an Expando's 
/// header section
/// </summary>
public class HeaderInfo : IDisposable {

	#region Class Data

	/// <summary>
	/// The Font used to draw the text on the title bar
	/// </summary>
	private Font titleFont;

	/// <summary>
	/// The Margin around the header
	/// </summary>
	private int margin;

	/// <summary>
	/// The Image used as the title bar's background for a special Expando
	/// </summary>
	private Image specialBackImage;

	/// <summary>
	/// The Image used as the title bar's background for a normal Expando
	/// </summary>
	private Image normalBackImage;

	/// <summary>
	///  The width of the Image used as the title bar's background
	/// </summary>
	private int backImageWidth;

	/// <summary>
	/// The height of the Image used as the title bar's background
	/// </summary>
	private int backImageHeight;

	/// <summary>
	/// The Color of the text on the title bar for a special Expando
	/// </summary>
	private Color specialTitle;

	/// <summary>
	/// The Color of the text on the title bar for a normal Expando
	/// </summary>
	private Color normalTitle;

	/// <summary>
	/// The Color of the text on the title bar for a special Expando 
	/// when highlighted
	/// </summary>
	private Color specialTitleHot;

	/// <summary>
	/// The Color of the text on the title bar for a normal Expando 
	/// when highlighted
	/// </summary>
	private Color normalTitleHot;

	/// <summary>
	/// The alignment of the text on the title bar for a special Expando
	/// </summary>
	private ContentAlignment specialAlignment;

	/// <summary>
	/// The alignment of the text on the title bar for a normal Expando
	/// </summary>
	private ContentAlignment normalAlignment;

	/// <summary>
	/// The amount of space between the border and items along 
	/// each edge of the title bar for a special Expando
	/// </summary>
	private PaddingEx specialPadding;

	/// <summary>
	/// The amount of space between the border and items along 
	/// each edge of the title bar for a normal Expando
	/// </summary>
	private PaddingEx normalPadding;

	/// <summary>
	/// The width of the Border along each edge of the title bar 
	/// for a special Expando
	/// </summary>
	private Border specialBorder;

	/// <summary>
	/// The width of the Border along each edge of the title bar 
	/// for a normal Expando
	/// </summary>
	private Border normalBorder;

	/// <summary>
	/// The Color of the title bar's Border for a special Expando
	/// </summary>
	private Color specialBorderColor;

	/// <summary>
	/// The Color of the title bar's Border for a normal Expando
	/// </summary>
	private Color normalBorderColor;

	/// <summary>
	/// The Color of the title bar's background for a special Expando
	/// </summary>
	private Color specialBackColor;

	/// <summary>
	/// The Color of the title bar's background for a normal Expando
	/// </summary>
	private Color normalBackColor;

	/// <summary>
	/// The Image that is used as a collapse arrow on the title bar 
	/// for a special Expando
	/// </summary>
	private Image specialArrowUp;

	/// <summary>
	/// The Image that is used as a collapse arrow on the title bar 
	/// for a special Expando when highlighted
	/// </summary>
	private Image specialArrowUpHot;

	/// <summary>
	/// The Image that is used as an expand arrow on the title bar 
	/// for a special Expando
	/// </summary>
	private Image specialArrowDown;

	/// <summary>
	/// The Image that is used as an expand arrow on the title bar 
	/// for a special Expando when highlighted
	/// </summary>
	private Image specialArrowDownHot;

	/// <summary>
	/// The Image that is used as a collapse arrow on the title bar 
	/// for a normal Expando
	/// </summary>
	private Image normalArrowUp;

	/// <summary>
	/// The Image that is used as a collapse arrow on the title bar 
	/// for a normal Expando when highlighted
	/// </summary>
	private Image normalArrowUpHot;

	/// <summary>
	/// The Image that is used as an expand arrow on the title bar 
	/// for a normal Expando
	/// </summary>
	private Image normalArrowDown;

	/// <summary>
	/// The Image that is used as an expand arrow on the title bar 
	/// for a normal Expando when highlighted
	/// </summary>
	private Image normalArrowDownHot;

	/// <summary>
	/// Specifies whether the title bar should use a gradient fill
	/// </summary>
	private bool useTitleGradient;

	/// <summary>
	/// The start Color of a title bar's gradient fill for a special 
	/// Expando
	/// </summary>
	private Color specialGradientStartColor;

	/// <summary>
	/// The end Color of a title bar's gradient fill for a special 
	/// Expando
	/// </summary>
	private Color specialGradientEndColor;

	/// <summary>
	/// The start Color of a title bar's gradient fill for a normal 
	/// Expando
	/// </summary>
	private Color normalGradientStartColor;

	/// <summary>
	/// The end Color of a title bar's gradient fill for a normal 
	/// Expando
	/// </summary>
	private Color normalGradientEndColor;

	/// <summary>
	/// How far along the title bar the gradient starts
	/// </summary>
	private float gradientOffset;

	/// <summary>
	/// The radius of the corners on the title bar
	/// </summary>
	private int titleRadius;

	/// <summary>
	/// The Expando that the HeaderInfo belongs to
	/// </summary>
	private Expando owner;

	/// <summary>
	/// 
	/// </summary>
	private bool rightToLeft;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the HeaderInfo class with default settings
	/// </summary>
	public HeaderInfo() {
		// work out the default font name for the user's os.
		// this ignores other fonts that may be specified - need 
		// to change parser to get font names
		if (Environment.OSVersion.Version.Major >= 5) {
			// Win2k, XP, Server 2003
			this.titleFont = new Font("Tahoma", 8.25f, FontStyle.Bold);
		} else {
			// Win9x, ME, NT
			this.titleFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
		}

		this.margin = 15;

		// set title colors and alignment
		this.specialTitle = Color.Transparent;
		this.specialTitleHot = Color.Transparent;

		this.normalTitle = Color.Transparent;
		this.normalTitleHot = Color.Transparent;

		this.specialAlignment = ContentAlignment.MiddleLeft;
		this.normalAlignment = ContentAlignment.MiddleLeft;

		// set padding values
		this.specialPadding = new PaddingEx(10, 0, 1, 0);
		this.normalPadding = new PaddingEx(10, 0, 1, 0);

		// set border values
		this.specialBorder = new Border(2, 2, 2, 0);
		this.specialBorderColor = Color.Transparent;

		this.normalBorder = new Border(2, 2, 2, 0);
		this.normalBorderColor = Color.Transparent;

		this.specialBackColor = Color.Transparent;
		this.normalBackColor = Color.Transparent;

		// set background image values
		this.specialBackImage = null;
		this.normalBackImage = null;

		this.backImageWidth = -1;
		this.backImageHeight = -1;

		// set arrow values
		this.specialArrowUp = null;
		this.specialArrowUpHot = null;
		this.specialArrowDown = null;
		this.specialArrowDownHot = null;

		this.normalArrowUp = null;
		this.normalArrowUpHot = null;
		this.normalArrowDown = null;
		this.normalArrowDownHot = null;

		this.useTitleGradient = false;
		this.specialGradientStartColor = Color.White;
		this.specialGradientEndColor = SystemColors.Highlight;
		this.normalGradientStartColor = Color.White;
		this.normalGradientEndColor = SystemColors.Highlight;
		this.gradientOffset = 0.5f;
		this.titleRadius = 5;

		this.owner = null;
		this.rightToLeft = false;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Forces the use of default values
	/// </summary>
	public void SetDefaultValues() {
		// work out the default font name for the user's os
		if (Environment.OSVersion.Version.Major >= 5) {
			// Win2k, XP, Server 2003
			this.titleFont = new Font("Tahoma", 8.25f, FontStyle.Bold);
		} else {
			// Win9x, ME, NT
			this.titleFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
		}

		this.margin = 15;

		// set title colors and alignment
		this.specialTitle = SystemColors.HighlightText;
		this.specialTitleHot = SystemColors.HighlightText;

		this.normalTitle = SystemColors.ControlText;
		this.normalTitleHot = SystemColors.ControlText;

		this.specialAlignment = ContentAlignment.MiddleLeft;
		this.normalAlignment = ContentAlignment.MiddleLeft;

		// set padding values
		this.specialPadding.Left = 10;
		this.specialPadding.Top = 0;
		this.specialPadding.Right = 1;
		this.specialPadding.Bottom = 0;

		this.normalPadding.Left = 10;
		this.normalPadding.Top = 0;
		this.normalPadding.Right = 1;
		this.normalPadding.Bottom = 0;

		// set border values
		this.specialBorder.Left = 2;
		this.specialBorder.Top = 2;
		this.specialBorder.Right = 2;
		this.specialBorder.Bottom = 0;

		this.specialBorderColor = SystemColors.Highlight;
		this.specialBackColor = SystemColors.Highlight;

		this.normalBorder.Left = 2;
		this.normalBorder.Top = 2;
		this.normalBorder.Right = 2;
		this.normalBorder.Bottom = 0;

		this.normalBorderColor = SystemColors.Control;
		this.normalBackColor = SystemColors.Control;

		// set background image values
		this.specialBackImage = null;
		this.normalBackImage = null;

		this.backImageWidth = 186;
		this.backImageHeight = 25;

		// set arrow values
		this.specialArrowUp = null;
		this.specialArrowUpHot = null;
		this.specialArrowDown = null;
		this.specialArrowDownHot = null;

		this.normalArrowUp = null;
		this.normalArrowUpHot = null;
		this.normalArrowDown = null;
		this.normalArrowDownHot = null;

		this.useTitleGradient = false;
		this.specialGradientStartColor = Color.White;
		this.specialGradientEndColor = SystemColors.Highlight;
		this.normalGradientStartColor = Color.White;
		this.normalGradientEndColor = SystemColors.Highlight;
		this.gradientOffset = 0.5f;
		this.titleRadius = 2;

		this.rightToLeft = false;
	}


	/// <summary>
	/// Forces the use of default empty values
	/// </summary>
	public void SetDefaultEmptyValues() {
		// work out the default font name for the user's os
		this.titleFont = null;

		this.margin = 15;

		// set title colors and alignment
		this.specialTitle = Color.Empty;
		this.specialTitleHot = Color.Empty;

		this.normalTitle = Color.Empty;
		this.normalTitleHot = Color.Empty;

		this.specialAlignment = ContentAlignment.MiddleLeft;
		this.normalAlignment = ContentAlignment.MiddleLeft;

		// set padding values
		this.specialPadding = PaddingEx.Empty;
		this.normalPadding = PaddingEx.Empty;

		// set border values
		this.specialBorder = Border.Empty;
		this.specialBorderColor = Color.Empty;
		this.specialBackColor = Color.Empty;

		this.normalBorder = Border.Empty;
		this.normalBorderColor = Color.Empty;
		this.normalBackColor = Color.Empty;

		// set background image values
		this.specialBackImage = null;
		this.normalBackImage = null;

		this.backImageWidth = 186;
		this.backImageHeight = 25;

		// set arrow values
		this.specialArrowUp = null;
		this.specialArrowUpHot = null;
		this.specialArrowDown = null;
		this.specialArrowDownHot = null;

		this.normalArrowUp = null;
		this.normalArrowUpHot = null;
		this.normalArrowDown = null;
		this.normalArrowDownHot = null;

		this.useTitleGradient = false;
		this.specialGradientStartColor = Color.Empty;
		this.specialGradientEndColor = Color.Empty;
		this.normalGradientStartColor = Color.Empty;
		this.normalGradientEndColor = Color.Empty;
		this.gradientOffset = 0.5f;
		this.titleRadius = 2;

		this.rightToLeft = false;
	}


	/// <summary>
	/// Releases all resources used by the HeaderInfo
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


		if (this.specialArrowUp != null) {
			this.specialArrowUp.Dispose();
			this.specialArrowUp = null;
		}

		if (this.specialArrowUpHot != null) {
			this.specialArrowUpHot.Dispose();
			this.specialArrowUpHot = null;
		}

		if (this.specialArrowDown != null) {
			this.specialArrowDown.Dispose();
			this.specialArrowDown = null;
		}

		if (this.specialArrowDownHot != null) {
			this.specialArrowDownHot.Dispose();
			this.specialArrowDownHot = null;
		}

		if (this.normalArrowUp != null) {
			this.normalArrowUp.Dispose();
			this.normalArrowUp = null;
		}

		if (this.normalArrowUpHot != null) {
			this.normalArrowUpHot.Dispose();
			this.normalArrowUpHot = null;
		}

		if (this.normalArrowDown != null) {
			this.normalArrowDown.Dispose();
			this.normalArrowDown = null;
		}

		if (this.normalArrowDownHot != null) {
			this.normalArrowDownHot.Dispose();
			this.normalArrowDownHot = null;
		}
	}

	#endregion


	#region Properties

	#region Border

	/// <summary>
	/// Gets or sets the border value for a special header
	/// </summary>
	[Description("The width of the border along each side of a special Expando's Title Bar")]
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
	/// Gets or sets the border color for a special header
	/// </summary>
	[Description("The border color for a special Expandos titlebar")]
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
	/// Gets or sets the background Color for a special header
	/// </summary>
	[Description("The background Color for a special Expandos titlebar")]
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
	/// Gets or sets the border value for a normal header
	/// </summary>
	[Description("The width of the border along each side of a normal Expando's Title Bar")]
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
	/// Gets or sets the border color for a normal header
	/// </summary>
	[Description("The border color for a normal Expandos titlebar")]
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


	/// <summary>
	/// Gets or sets the background Color for a normal header
	/// </summary>
	[Description("The background Color for a normal Expandos titlebar")]
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

	#endregion

	#region Fonts

	/// <summary>
	/// Gets the Font used to render the header's text
	/// </summary>
	[DefaultValue(null),
	 Description("The Font used to render the titlebar's text")]
	public Font TitleFont {
		get { return this.titleFont; }

		set {
			if (this.titleFont != value) {
				this.titleFont = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets the name of the font used to render the header's text. 
	/// </summary>
	protected internal string FontName {
		get { return this.TitleFont.Name; }

		set { this.TitleFont = new Font(value, this.TitleFont.SizeInPoints, this.TitleFont.Style); }
	}


	/// <summary>
	/// Gets or sets the size of the font used to render the header's text. 
	/// </summary>
	protected internal float FontSize {
		get { return this.TitleFont.SizeInPoints; }

		set { this.TitleFont = new Font(this.TitleFont.Name, value, this.TitleFont.Style); }
	}


	/// <summary>
	/// Gets or sets the weight of the font used to render the header's text. 
	/// </summary>
	protected internal FontStyle FontWeight {
		get { return this.TitleFont.Style; }

		set {
			value |= this.TitleFont.Style;

			this.TitleFont = new Font(this.TitleFont.Name, this.TitleFont.SizeInPoints, value);
		}
	}


	/// <summary>
	/// Gets or sets the style of the Font used to render the header's text. 
	/// </summary>
	protected internal FontStyle FontStyle {
		get { return this.TitleFont.Style; }

		set {
			value |= this.TitleFont.Style;

			this.TitleFont = new Font(this.TitleFont.Name, this.TitleFont.SizeInPoints, value);
		}
	}

	#endregion

	#region Images

	/// <summary>
	/// Gets or sets the background image for a special header
	/// </summary>
	[DefaultValue(null),
	 Description("The background image for a special titlebar")]
	public Image SpecialBackImage {
		get { return this.specialBackImage; }

		set {
			if (this.specialBackImage != value) {
				this.specialBackImage = value;

				if (value != null) {
					this.backImageWidth = value.Width;
					this.backImageHeight = value.Height;
				}

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets the background image for a normal header
	/// </summary>
	[DefaultValue(null),
	 Description("The background image for a normal titlebar")]
	public Image NormalBackImage {
		get { return this.normalBackImage; }

		set {
			if (this.normalBackImage != value) {
				this.normalBackImage = value;

				if (value != null) {
					this.backImageWidth = value.Width;
					this.backImageHeight = value.Height;
				}

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets the width of the header's background image
	/// </summary>
	protected internal int BackImageWidth {
		get {
			if (this.backImageWidth == -1) {
				return 186;
			}

			return this.backImageWidth;
		}

		set { this.backImageWidth = value; }
	}


	/// <summary>
	/// Gets or sets the height of the header's background image
	/// </summary>
	protected internal int BackImageHeight {
		get {
			if (this.backImageHeight < 23) {
				return 23;
			}

			return this.backImageHeight;
		}

		set { this.backImageHeight = value; }
	}


	/// <summary>
	/// Gets or sets a special header's collapse arrow image in it's normal state
	/// </summary>
	[DefaultValue(null),
	 Description("A special Expando's collapse arrow image in it's normal state")]
	public Image SpecialArrowUp {
		get { return this.specialArrowUp; }

		set {
			if (this.specialArrowUp != value) {
				this.specialArrowUp = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets a special header's collapse arrow image in it's highlighted state
	/// </summary>
	[DefaultValue(null),
	 Description("A special Expando's collapse arrow image in it's highlighted state")]
	public Image SpecialArrowUpHot {
		get { return this.specialArrowUpHot; }

		set {
			if (this.specialArrowUpHot != value) {
				this.specialArrowUpHot = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets a special header's expand arrow image in it's normal state
	/// </summary>
	[DefaultValue(null),
	 Description("A special Expando's expand arrow image in it's normal state")]
	public Image SpecialArrowDown {
		get { return this.specialArrowDown; }

		set {
			if (this.specialArrowDown != value) {
				this.specialArrowDown = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets a special header's expend arrow image in it's highlighted state
	/// </summary>
	[DefaultValue(null),
	 Description("A special Expando's expand arrow image in it's highlighted state")]
	public Image SpecialArrowDownHot {
		get { return this.specialArrowDownHot; }

		set {
			if (this.specialArrowDownHot != value) {
				this.specialArrowDownHot = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets a normal header's collapse arrow image in it's normal state
	/// </summary>
	[DefaultValue(null),
	 Description("A normal Expando's collapse arrow image in it's normal state")]
	public Image NormalArrowUp {
		get { return this.normalArrowUp; }

		set {
			if (this.normalArrowUp != value) {
				this.normalArrowUp = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets a normal header's collapse arrow image in it's highlighted state
	/// </summary>
	[DefaultValue(null),
	 Description("A normal Expando's collapse arrow image in it's highlighted state")]
	public Image NormalArrowUpHot {
		get { return this.normalArrowUpHot; }

		set {
			if (this.normalArrowUpHot != value) {
				this.normalArrowUpHot = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets a normal header's expand arrow image in it's normal state
	/// </summary>
	[DefaultValue(null),
	 Description("A normal Expando's expand arrow image in it's normal state")]
	public Image NormalArrowDown {
		get { return this.normalArrowDown; }

		set {
			if (this.normalArrowDown != value) {
				this.normalArrowDown = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets a normal header's expand arrow image in it's highlighted state
	/// </summary>
	[DefaultValue(null),
	 Description("A normal Expando's expand arrow image in it's highlighted state")]
	public Image NormalArrowDownHot {
		get { return this.normalArrowDownHot; }

		set {
			if (this.normalArrowDownHot != value) {
				this.normalArrowDownHot = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Sets the arrow images for use when theming is not supported
	/// </summary>
	internal void SetUnthemedArrowImages() {
		// get the arrow images resource
		System.Reflection.Assembly myAssembly;
		myAssembly = this.GetType().Assembly;

		// set the arrow images
		this.specialArrowDown = Resources.SPECIALGROUPEXPAND;
		this.specialArrowDownHot = Resources.SPECIALGROUPEXPANDHOT;
		this.specialArrowUp = Resources.SPECIALGROUPCOLLAPSE;
		this.specialArrowUpHot = Resources.SPECIALGROUPCOLLAPSEHOT;

		this.normalArrowDown = Resources.NORMALGROUPEXPAND;
		this.normalArrowDownHot = Resources.NORMALGROUPEXPANDHOT;
		this.normalArrowUp = Resources.NORMALGROUPCOLLAPSE;
		this.normalArrowUpHot = Resources.NORMALGROUPCOLLAPSEHOT;
	}

	#endregion

	#region Margin

	/// <summary>
	/// Gets or sets the margin around the header
	/// </summary>
	[DefaultValue(15),
	 Description("The margin around the titlebar")]
	public int Margin {
		get { return this.margin; }

		set {
			if (this.margin != value) {
				this.margin = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}

	#endregion

	#region Padding

	/// <summary>
	/// Gets or sets the padding for a special header
	/// </summary>
	[Description("The amount of space between the border and items along each side of a special Expandos Title Bar")]
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
	/// Gets or sets the padding for a normal header
	/// </summary>
	[Description("The amount of space between the border and items along each side of a normal Expandos Title Bar")]
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

	#region Title

	/// <summary>
	/// Gets or sets the color of the text displayed in a special 
	/// header in it's normal state
	/// </summary>
	[Description("The color of the text displayed in a special Expandos titlebar in it's normal state")]
	public Color SpecialTitleColor {
		get { return this.specialTitle; }

		set {
			if (this.specialTitle != value) {
				this.specialTitle = value;

				// set the SpecialTitleHotColor as well just in case
				// it isn't/wasn't set during UIFILE parsing
				if (this.SpecialTitleHotColor == Color.Transparent) {
					this.SpecialTitleHotColor = value;
				}

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the SpecialTitleColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the SpecialTitleColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSpecialTitleColor() {
		return this.SpecialTitleColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the color of the text displayed in a special 
	/// header in it's highlighted state
	/// </summary>
	[Description("The color of the text displayed in a special Expandos titlebar in it's highlighted state")]
	public Color SpecialTitleHotColor {
		get { return this.specialTitleHot; }

		set {
			if (this.specialTitleHot != value) {
				this.specialTitleHot = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the SpecialTitleHotColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the SpecialTitleHotColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSpecialTitleHotColor() {
		return this.SpecialTitleHotColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the color of the text displayed in a normal 
	/// header in it's normal state
	/// </summary>
	[Description("The color of the text displayed in a normal Expandos titlebar in it's normal state")]
	public Color NormalTitleColor {
		get { return this.normalTitle; }

		set {
			if (this.normalTitle != value) {
				this.normalTitle = value;

				// set the NormalTitleHotColor as well just in case
				// it isn't/wasn't set during UIFILE parsing
				if (this.NormalTitleHotColor == Color.Transparent) {
					this.NormalTitleHotColor = value;
				}

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the NormalTitleColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the NormalTitleColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeNormalTitleColor() {
		return this.NormalTitleColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the color of the text displayed in a normal 
	/// header in it's highlighted state
	/// </summary>
	[Description("The color of the text displayed in a normal Expandos titlebar in it's highlighted state")]
	public Color NormalTitleHotColor {
		get { return this.normalTitleHot; }

		set {
			if (this.normalTitleHot != value) {
				this.normalTitleHot = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the NormalTitleHotColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the NormalTitleHotColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeNormalTitleHotColor() {
		return this.NormalTitleHotColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the alignment of the text displayed in a special header
	/// </summary>
	[DefaultValue(ContentAlignment.MiddleLeft),
	 Description("The alignment of the text displayed in a special Expandos titlebar")]
	public ContentAlignment SpecialAlignment {
		get { return this.specialAlignment; }

		set {
			if (!Enum.IsDefined(typeof(ContentAlignment), value)) {
				throw new InvalidEnumArgumentException("value", (int)value, typeof(ContentAlignment));
			}

			if (this.specialAlignment != value) {
				this.specialAlignment = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets the alignment of the text displayed in a normal header
	/// </summary>
	[DefaultValue(ContentAlignment.MiddleLeft),
	 Description("The alignment of the text displayed in a normal Expandos titlebar")]
	public ContentAlignment NormalAlignment {
		get { return this.normalAlignment; }

		set {
			if (!Enum.IsDefined(typeof(ContentAlignment), value)) {
				throw new InvalidEnumArgumentException("value", (int)value, typeof(ContentAlignment));
			}

			if (this.normalAlignment != value) {
				this.normalAlignment = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets whether the header's background should use a gradient fill
	/// </summary>
	[DefaultValue(false),
	 Description("")]
	public bool TitleGradient {
		get { return this.useTitleGradient; }

		set {
			if (this.useTitleGradient != value) {
				this.useTitleGradient = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets the start Color of a header's gradient fill for a special 
	/// Expando
	/// </summary>
	[Description("")]
	public Color SpecialGradientStartColor {
		get { return this.specialGradientStartColor; }

		set {
			if (this.specialGradientStartColor != value) {
				this.specialGradientStartColor = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the SpecialGradientStartColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the SpecialGradientStartColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSpecialGradientStartColor() {
		return this.SpecialGradientStartColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the end Color of a header's gradient fill for a special 
	/// Expando
	/// </summary>
	[Description("")]
	public Color SpecialGradientEndColor {
		get { return this.specialGradientEndColor; }

		set {
			if (this.specialGradientEndColor != value) {
				this.specialGradientEndColor = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the SpecialGradientEndColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the SpecialGradientEndColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSpecialGradientEndColor() {
		return this.SpecialGradientEndColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the start Color of a header's gradient fill for a normal 
	/// Expando
	/// </summary>
	[Description("")]
	public Color NormalGradientStartColor {
		get { return this.normalGradientStartColor; }

		set {
			if (this.normalGradientStartColor != value) {
				this.normalGradientStartColor = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the NormalGradientStartColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the NormalGradientStartColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeNormalGradientStartColor() {
		return this.NormalGradientStartColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets the end Color of a header's gradient fill for a normal 
	/// Expando
	/// </summary>
	[Description("")]
	public Color NormalGradientEndColor {
		get { return this.normalGradientEndColor; }

		set {
			if (this.normalGradientEndColor != value) {
				this.normalGradientEndColor = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the NormalGradientEndColor property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the NormalGradientEndColor property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeNormalGradientEndColor() {
		return this.NormalGradientEndColor != Color.Empty;
	}


	/// <summary>
	/// Gets or sets how far along the header the gradient starts
	/// </summary>
	[DefaultValue(0.5f),
	 Description("")]
	public float GradientOffset {
		get { return this.gradientOffset; }

		set {
			if (value < 0) {
				value = 0f;
			} else if (value > 1) {
				value = 1f;
			}

			if (this.gradientOffset != value) {
				this.gradientOffset = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}


	/// <summary>
	///Gets or sets the radius of the corners on the header
	/// </summary>
	[DefaultValue(2),
	 Description("")]
	public int TitleRadius {
		get { return this.titleRadius; }

		set {
			if (value < 0) {
				value = 0;
			} else if (value > this.BackImageHeight) {
				value = this.BackImageHeight;
			}

			if (this.titleRadius != value) {
				this.titleRadius = value;

				if (this.Expando != null) {
					this.Expando.FireCustomSettingsChanged(EventArgs.Empty);
				}
			}
		}
	}

	#endregion

	#region Expando

	/// <summary>
	/// Gets or sets the Expando the HeaderInfo belongs to
	/// </summary>
	protected internal Expando Expando {
		get { return this.owner; }

		set { this.owner = value; }
	}


	/// <summary>
	/// 
	/// </summary>
	internal bool RightToLeft {
		get { return this.rightToLeft; }

		set { this.rightToLeft = value; }
	}

	#endregion

	#endregion

}
