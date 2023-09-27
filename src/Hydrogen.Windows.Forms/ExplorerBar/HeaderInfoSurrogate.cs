// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Drawing;
using System.Security.Permissions;
using System.Reflection;


namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that is serialized instead of a HeaderInfo (as 
/// HeaderInfos contain objects that cause serialization problems)
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable()]
public class HeaderInfoSurrogate : ISerializable {

	#region Class Data

	/// <summary>
	/// See Font.Name.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string FontName;

	/// <summary>
	/// See Font.Size.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public float FontSize;

	/// <summary>
	/// See Font.Style.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public FontStyle FontStyle;

	/// <summary>
	/// See HeaderInfo.Margin.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public int Margin;

	/// <summary>
	/// See HeaderInfo.SpecialBackImage.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("SpecialBackImage", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] SpecialBackImage;

	/// <summary>
	/// See HeaderInfo.NormalBackImage.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("NormalBackImage", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] NormalBackImage;

	/// <summary>
	/// See HeaderInfo.SpecialTitle.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string SpecialTitle;

	/// <summary>
	/// See HeaderInfo.NormalTitle.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string NormalTitle;

	/// <summary>
	/// See HeaderInfo.SpecialTitleHot.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string SpecialTitleHot;

	/// <summary>
	/// See HeaderInfo.NormalTitleHot.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string NormalTitleHot;

	/// <summary>
	/// See HeaderInfo.SpecialAlignment.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public ContentAlignment SpecialAlignment;

	/// <summary>
	/// See HeaderInfo.NormalAlignment.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public ContentAlignment NormalAlignment;

	/// <summary>
	/// See HeaderInfo.SpecialPadding.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public PaddingEx SpecialPadding;

	/// <summary>
	/// See HeaderInfo.NormalPadding.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public PaddingEx NormalPadding;

	/// <summary>
	/// See HeaderInfo.SpecialBorder.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public Border SpecialBorder;

	/// <summary>
	/// See HeaderInfo.NormalBorder.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public Border NormalBorder;

	/// <summary>
	/// See HeaderInfo.SpecialBorderColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string SpecialBorderColor;

	/// <summary>
	/// See HeaderInfo.NormalBorderColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string NormalBorderColor;

	/// <summary>
	/// See HeaderInfo.SpecialBackColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string SpecialBackColor;

	/// <summary>
	/// See HeaderInfo.NormalBackColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string NormalBackColor;

	/// <summary>
	/// See HeaderInfo.SpecialArrowUp.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("SpecialArrowUp", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] SpecialArrowUp;

	/// <summary>
	/// See HeaderInfo.SpecialArrowUpHot.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("SpecialArrowUpHot", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] SpecialArrowUpHot;

	/// <summary>
	/// See HeaderInfo.SpecialArrowDown.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("SpecialArrowDown", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] SpecialArrowDown;

	/// <summary>
	/// See HeaderInfo.SpecialArrowDownHot.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("SpecialArrowDownHot", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] SpecialArrowDownHot;

	/// <summary>
	/// See HeaderInfo.NormalArrowUp.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("NormalArrowUp", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] NormalArrowUp;

	/// <summary>
	/// See HeaderInfo.NormalArrowUpHot.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("NormalArrowUpHot", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] NormalArrowUpHot;

	/// <summary>
	/// See HeaderInfo.NormalArrowDown.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("NormalArrowDown", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] NormalArrowDown;

	/// <summary>
	/// See HeaderInfo.NormalArrowDownHot.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("NormalArrowDownHot", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] NormalArrowDownHot;

	/// <summary>
	/// See HeaderInfo.TitleGradient.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public bool TitleGradient;

	/// <summary>
	/// See HeaderInfo.SpecialGradientStartColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string SpecialGradientStartColor;

	/// <summary>
	/// See HeaderInfo.SpecialGradientEndColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string SpecialGradientEndColor;

	/// <summary>
	/// See HeaderInfo.NormalGradientStartColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string NormalGradientStartColor;

	/// <summary>
	/// See HeaderInfo.NormalGradientEndColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string NormalGradientEndColor;

	/// <summary>
	/// See HeaderInfo.GradientOffset.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public float GradientOffset;

	/// <summary>
	/// See HeaderInfo.TitleRadius.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public int TitleRadius;

	/// <summary>
	/// Version number of the surrogate.  This member is not intended 
	/// to be used directly from your code.
	/// </summary>
	public int Version = 3300;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the HeaderInfoSurrogate class with default settings
	/// </summary>
	public HeaderInfoSurrogate() {
		this.FontName = null;
		this.FontSize = 8.25f;
		this.FontStyle = FontStyle.Regular;
		this.Margin = 15;

		this.SpecialBackImage = new byte[0];
		this.NormalBackImage = new byte[0];

		this.SpecialTitle = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.NormalTitle = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.SpecialTitleHot = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.NormalTitleHot = Tools.Drawing.ConvertColorToString(Color.Empty);

		this.SpecialAlignment = ContentAlignment.MiddleLeft;
		this.NormalAlignment = ContentAlignment.MiddleLeft;

		this.SpecialPadding = PaddingEx.Empty;
		this.NormalPadding = PaddingEx.Empty;

		this.SpecialBorder = Border.Empty;
		this.NormalBorder = Border.Empty;
		this.SpecialBorderColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.NormalBorderColor = Tools.Drawing.ConvertColorToString(Color.Empty);

		this.SpecialBackColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.NormalBackColor = Tools.Drawing.ConvertColorToString(Color.Empty);

		this.SpecialArrowUp = new byte[0];
		this.SpecialArrowUpHot = new byte[0];
		this.SpecialArrowDown = new byte[0];
		this.SpecialArrowDownHot = new byte[0];
		this.NormalArrowUp = new byte[0];
		this.NormalArrowUpHot = new byte[0];
		this.NormalArrowDown = new byte[0];
		this.NormalArrowDownHot = new byte[0];

		this.TitleGradient = false;
		this.SpecialGradientStartColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.SpecialGradientEndColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.NormalGradientStartColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.NormalGradientEndColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.GradientOffset = 0.5f;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Populates the HeaderInfoSurrogate with data that is to be 
	/// serialized from the specified HeaderInfo
	/// </summary>
	/// <param name="headerInfo">The HeaderInfo that contains the data 
	/// to be serialized</param>
	public void Load(HeaderInfo headerInfo) {
		if (headerInfo.TitleFont != null) {
			this.FontName = headerInfo.TitleFont.Name;
			this.FontSize = headerInfo.TitleFont.SizeInPoints;
			this.FontStyle = headerInfo.TitleFont.Style;
		}

		this.Margin = headerInfo.Margin;

		this.SpecialBackImage = headerInfo.SpecialBackImage.ToByteArray();
		this.NormalBackImage = headerInfo.NormalBackImage.ToByteArray();

		this.SpecialTitle = Tools.Drawing.ConvertColorToString(headerInfo.SpecialTitleColor);
		this.NormalTitle = Tools.Drawing.ConvertColorToString(headerInfo.NormalTitleColor);
		this.SpecialTitleHot = Tools.Drawing.ConvertColorToString(headerInfo.SpecialTitleHotColor);
		this.NormalTitleHot = Tools.Drawing.ConvertColorToString(headerInfo.NormalTitleHotColor);

		this.SpecialAlignment = headerInfo.SpecialAlignment;
		this.NormalAlignment = headerInfo.NormalAlignment;

		this.SpecialPadding = headerInfo.SpecialPadding;
		this.NormalPadding = headerInfo.NormalPadding;

		this.SpecialBorder = headerInfo.SpecialBorder;
		this.NormalBorder = headerInfo.NormalBorder;
		this.SpecialBorderColor = Tools.Drawing.ConvertColorToString(headerInfo.SpecialBorderColor);
		this.NormalBorderColor = Tools.Drawing.ConvertColorToString(headerInfo.NormalBorderColor);

		this.SpecialBackColor = Tools.Drawing.ConvertColorToString(headerInfo.SpecialBackColor);
		this.NormalBackColor = Tools.Drawing.ConvertColorToString(headerInfo.NormalBackColor);

		this.SpecialArrowUp = headerInfo.SpecialArrowUp.ToByteArray();
		this.SpecialArrowUpHot = headerInfo.SpecialArrowUpHot.ToByteArray();
		this.SpecialArrowDown = headerInfo.SpecialArrowDown.ToByteArray();
		this.SpecialArrowDownHot = headerInfo.SpecialArrowDownHot.ToByteArray();
		this.NormalArrowUp = headerInfo.NormalArrowUp.ToByteArray();
		this.NormalArrowUpHot = headerInfo.NormalArrowUpHot.ToByteArray();
		this.NormalArrowDown = headerInfo.NormalArrowDown.ToByteArray();
		this.NormalArrowDownHot = headerInfo.NormalArrowDownHot.ToByteArray();

		this.TitleGradient = headerInfo.TitleGradient;
		this.SpecialGradientStartColor = Tools.Drawing.ConvertColorToString(headerInfo.SpecialGradientStartColor);
		this.SpecialGradientEndColor = Tools.Drawing.ConvertColorToString(headerInfo.SpecialGradientEndColor);
		this.NormalGradientStartColor = Tools.Drawing.ConvertColorToString(headerInfo.NormalGradientStartColor);
		this.NormalGradientEndColor = Tools.Drawing.ConvertColorToString(headerInfo.NormalGradientEndColor);
		this.GradientOffset = headerInfo.GradientOffset;
	}


	/// <summary>
	/// Returns a HeaderInfo that contains the deserialized HeaderInfoSurrogate data
	/// </summary>
	/// <returns>A HeaderInfo that contains the deserialized HeaderInfoSurrogate data</returns>
	public HeaderInfo Save() {
		HeaderInfo headerInfo = new HeaderInfo();

		if (this.FontName != null) {
			headerInfo.TitleFont = new Font(this.FontName, this.FontSize, this.FontStyle);
		}

		headerInfo.Margin = this.Margin;

		headerInfo.SpecialBackImage = this.SpecialBackImage.ToImage();
		headerInfo.NormalBackImage = this.NormalBackImage.ToImage();

		headerInfo.SpecialTitleColor = Tools.Drawing.ConvertStringToColor(this.SpecialTitle);
		headerInfo.NormalTitleColor = Tools.Drawing.ConvertStringToColor(this.NormalTitle);
		headerInfo.SpecialTitleHotColor = Tools.Drawing.ConvertStringToColor(this.SpecialTitleHot);
		headerInfo.NormalTitleHotColor = Tools.Drawing.ConvertStringToColor(this.NormalTitleHot);

		headerInfo.SpecialAlignment = this.SpecialAlignment;
		headerInfo.NormalAlignment = this.NormalAlignment;

		headerInfo.SpecialPadding = this.SpecialPadding;
		headerInfo.NormalPadding = this.NormalPadding;

		headerInfo.SpecialBorder = this.SpecialBorder;
		headerInfo.NormalBorder = this.NormalBorder;
		headerInfo.SpecialBorderColor = Tools.Drawing.ConvertStringToColor(this.SpecialBorderColor);
		headerInfo.NormalBorderColor = Tools.Drawing.ConvertStringToColor(this.NormalBorderColor);

		headerInfo.SpecialBackColor = Tools.Drawing.ConvertStringToColor(this.SpecialBackColor);
		headerInfo.NormalBackColor = Tools.Drawing.ConvertStringToColor(this.NormalBackColor);

		headerInfo.SpecialArrowUp = this.SpecialArrowUp.ToImage();
		headerInfo.SpecialArrowUpHot = this.SpecialArrowUpHot.ToImage();
		headerInfo.SpecialArrowDown = this.SpecialArrowDown.ToImage();
		headerInfo.SpecialArrowDownHot = this.SpecialArrowDownHot.ToImage();
		headerInfo.NormalArrowUp = this.NormalArrowUp.ToImage();
		headerInfo.NormalArrowUpHot = this.NormalArrowUpHot.ToImage();
		headerInfo.NormalArrowDown = this.NormalArrowDown.ToImage();
		headerInfo.NormalArrowDownHot = this.NormalArrowDownHot.ToImage();

		headerInfo.TitleGradient = this.TitleGradient;
		headerInfo.SpecialGradientStartColor = Tools.Drawing.ConvertStringToColor(this.SpecialGradientStartColor);
		headerInfo.SpecialGradientEndColor = Tools.Drawing.ConvertStringToColor(this.SpecialGradientEndColor);
		headerInfo.NormalGradientStartColor = Tools.Drawing.ConvertStringToColor(this.NormalGradientStartColor);
		headerInfo.NormalGradientEndColor = Tools.Drawing.ConvertStringToColor(this.NormalGradientEndColor);
		headerInfo.GradientOffset = this.GradientOffset;

		return headerInfo;
	}


	/// <summary>
	/// Populates a SerializationInfo with the data needed to serialize the HeaderInfoSurrogate
	/// </summary>
	/// <param name="info">The SerializationInfo to populate with data</param>
	/// <param name="context">The destination for this serialization</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		info.AddValue("Version", this.Version);

		info.AddValue("FontName", this.FontName);
		info.AddValue("FontSize", this.FontSize);
		info.AddValue("FontStyle", this.FontStyle);

		info.AddValue("Margin", this.Margin);

		info.AddValue("SpecialBackImage", this.SpecialBackImage);
		info.AddValue("NormalBackImage", this.NormalBackImage);

		info.AddValue("SpecialTitle", this.SpecialTitle);
		info.AddValue("NormalTitle", this.NormalTitle);
		info.AddValue("SpecialTitleHot", this.SpecialTitleHot);
		info.AddValue("NormalTitleHot", this.NormalTitleHot);

		info.AddValue("SpecialAlignment", this.SpecialAlignment);
		info.AddValue("NormalAlignment", this.NormalAlignment);

		info.AddValue("SpecialPadding", this.SpecialPadding);
		info.AddValue("NormalPadding", this.NormalPadding);

		info.AddValue("SpecialBorder", this.SpecialBorder);
		info.AddValue("NormalBorder", this.NormalBorder);
		info.AddValue("SpecialBorderColor", this.SpecialBorderColor);
		info.AddValue("NormalBorderColor", this.NormalBorderColor);

		info.AddValue("SpecialBackColor", this.SpecialBackColor);
		info.AddValue("NormalBackColor", this.NormalBackColor);

		info.AddValue("SpecialArrowUp", this.SpecialArrowUp);
		info.AddValue("SpecialArrowUpHot", this.SpecialArrowUpHot);
		info.AddValue("SpecialArrowDown", this.SpecialArrowDown);
		info.AddValue("SpecialArrowDownHot", this.SpecialArrowDownHot);
		info.AddValue("NormalArrowUp", this.NormalArrowUp);
		info.AddValue("NormalArrowUpHot", this.NormalArrowUpHot);
		info.AddValue("NormalArrowDown", this.NormalArrowDown);
		info.AddValue("NormalArrowDownHot", this.NormalArrowDownHot);

		info.AddValue("TitleGradient", this.TitleGradient);
		info.AddValue("SpecialGradientStartColor", this.SpecialGradientStartColor);
		info.AddValue("SpecialGradientEndColor", this.SpecialGradientEndColor);
		info.AddValue("NormalGradientStartColor", this.NormalGradientStartColor);
		info.AddValue("NormalGradientEndColor", this.NormalGradientEndColor);
		info.AddValue("GradientOffset", this.GradientOffset);
	}


	/// <summary>
	/// Initializes a new instance of the HeaderInfoSurrogate class using the information 
	/// in the SerializationInfo
	/// </summary>
	/// <param name="info">The information to populate the HeaderInfoSurrogate</param>
	/// <param name="context">The source from which the HeaderInfoSurrogate is deserialized</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	protected HeaderInfoSurrogate(SerializationInfo info, StreamingContext context)
		: base() {
		int version = info.GetInt32("Version");

		this.FontName = info.GetString("FontName");
		this.FontSize = info.GetSingle("FontSize");
		this.FontStyle = (FontStyle)info.GetValue("FontStyle", typeof(FontStyle));

		this.Margin = info.GetInt32("Margin");

		this.SpecialBackImage = (byte[])info.GetValue("SpecialBackImage", typeof(byte[]));
		this.NormalBackImage = (byte[])info.GetValue("NormalBackImage", typeof(byte[]));

		this.SpecialTitle = info.GetString("SpecialTitle");
		this.NormalTitle = info.GetString("NormalTitle");
		this.SpecialTitleHot = info.GetString("SpecialTitleHot");
		this.NormalTitleHot = info.GetString("NormalTitleHot");

		this.SpecialAlignment = (ContentAlignment)info.GetValue("SpecialAlignment", typeof(ContentAlignment));
		this.NormalAlignment = (ContentAlignment)info.GetValue("NormalAlignment", typeof(ContentAlignment));

		this.SpecialPadding = (PaddingEx)info.GetValue("SpecialPadding", typeof(PaddingEx));
		this.NormalPadding = (PaddingEx)info.GetValue("NormalPadding", typeof(PaddingEx));

		this.SpecialBorder = (Border)info.GetValue("SpecialBorder", typeof(Border));
		this.NormalBorder = (Border)info.GetValue("NormalBorder", typeof(Border));
		this.SpecialBorderColor = info.GetString("SpecialBorderColor");
		this.NormalBorderColor = info.GetString("NormalBorderColor");

		this.SpecialBackColor = info.GetString("SpecialBackColor");
		this.NormalBackColor = info.GetString("NormalBackColor");

		this.SpecialArrowUp = (byte[])info.GetValue("SpecialArrowUp", typeof(byte[]));
		this.SpecialArrowUpHot = (byte[])info.GetValue("SpecialArrowUpHot", typeof(byte[]));
		this.SpecialArrowDown = (byte[])info.GetValue("SpecialArrowDown", typeof(byte[]));
		this.SpecialArrowDownHot = (byte[])info.GetValue("SpecialArrowDownHot", typeof(byte[]));
		this.NormalArrowUp = (byte[])info.GetValue("NormalArrowUp", typeof(byte[]));
		this.NormalArrowUpHot = (byte[])info.GetValue("NormalArrowUpHot", typeof(byte[]));
		this.NormalArrowDown = (byte[])info.GetValue("NormalArrowDown", typeof(byte[]));
		this.NormalArrowDownHot = (byte[])info.GetValue("NormalArrowDownHot", typeof(byte[]));

		this.TitleGradient = info.GetBoolean("TitleGradient");
		this.SpecialGradientStartColor = info.GetString("SpecialGradientStartColor");
		this.SpecialGradientEndColor = info.GetString("SpecialGradientEndColor");
		this.NormalGradientStartColor = info.GetString("NormalGradientStartColor");
		this.NormalGradientEndColor = info.GetString("NormalGradientEndColor");
		this.GradientOffset = info.GetSingle("GradientOffset");
	}

	#endregion

}
