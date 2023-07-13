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
/// A class that is serialized instead of an ExpandoInfo (as 
/// ExpandoInfos contain objects that cause serialization problems)
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable()]
public class ExpandoInfoSurrogate : ISerializable {

	#region Class Data

	/// <summary>
	/// See ExpandoInfo.SpecialBackColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string SpecialBackColor;

	/// <summary>
	/// See ExpandoInfo.NormalBackColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string NormalBackColor;

	/// <summary>
	/// See ExpandoInfo.SpecialBorder.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public Border SpecialBorder;

	/// <summary>
	/// See ExpandoInfo.NormalBorder.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public Border NormalBorder;

	/// <summary>
	/// See ExpandoInfo.SpecialBorderColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string SpecialBorderColor;

	/// <summary>
	/// See ExpandoInfo.NormalBorderColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string NormalBorderColor;

	/// <summary>
	/// See ExpandoInfo.SpecialPadding.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public PaddingEx SpecialPadding;

	/// <summary>
	/// See ExpandoInfo.NormalPadding.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public PaddingEx NormalPadding;

	/// <summary>
	/// See ExpandoInfo.SpecialBackImage.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("SpecialBackImage", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] SpecialBackImage;

	/// <summary>
	/// See ExpandoInfo.NormalBackImage.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("NormalBackImage", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] NormalBackImage;

	/// <summary>
	/// See ExpandoInfo.WatermarkAlignment.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public ContentAlignment WatermarkAlignment;

	/// <summary>
	/// Version number of the surrogate.  This member is not intended 
	/// to be used directly from your code.
	/// </summary>
	public int Version = 3300;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the ExpandoInfoSurrogate class with default settings
	/// </summary>
	public ExpandoInfoSurrogate() {
		this.SpecialBackColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.NormalBackColor = Tools.Drawing.ConvertColorToString(Color.Empty);

		this.SpecialBorder = Border.Empty;
		this.NormalBorder = Border.Empty;

		this.SpecialBorderColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.NormalBorderColor = Tools.Drawing.ConvertColorToString(Color.Empty);

		this.SpecialPadding = PaddingEx.Empty;
		this.NormalPadding = PaddingEx.Empty;

		this.SpecialBackImage = new byte[0];
		this.NormalBackImage = new byte[0];

		this.WatermarkAlignment = ContentAlignment.BottomRight;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Populates the ExpandoInfoSurrogate with data that is to be 
	/// serialized from the specified ExpandoInfo
	/// </summary>
	/// <param name="expandoInfo">The ExpandoInfo that contains the data 
	/// to be serialized</param>
	public void Load(ExpandoInfo expandoInfo) {
		this.SpecialBackColor = Tools.Drawing.ConvertColorToString(expandoInfo.SpecialBackColor);
		this.NormalBackColor = Tools.Drawing.ConvertColorToString(expandoInfo.NormalBackColor);

		this.SpecialBorder = expandoInfo.SpecialBorder;
		this.NormalBorder = expandoInfo.NormalBorder;

		this.SpecialBorderColor = Tools.Drawing.ConvertColorToString(expandoInfo.SpecialBorderColor);
		this.NormalBorderColor = Tools.Drawing.ConvertColorToString(expandoInfo.NormalBorderColor);

		this.SpecialPadding = expandoInfo.SpecialPadding;
		this.NormalPadding = expandoInfo.NormalPadding;

		this.SpecialBackImage = expandoInfo.SpecialBackImage.ToByteArray();
		this.NormalBackImage = expandoInfo.NormalBackImage.ToByteArray();

		this.WatermarkAlignment = expandoInfo.WatermarkAlignment;
	}


	/// <summary>
	/// Returns an ExpandoInfo that contains the deserialized ExpandoInfoSurrogate data
	/// </summary>
	/// <returns>An ExpandoInfo that contains the deserialized ExpandoInfoSurrogate data</returns>
	public ExpandoInfo Save() {
		ExpandoInfo expandoInfo = new ExpandoInfo();

		expandoInfo.SpecialBackColor = Tools.Drawing.ConvertStringToColor(this.SpecialBackColor);
		expandoInfo.NormalBackColor = Tools.Drawing.ConvertStringToColor(this.NormalBackColor);

		expandoInfo.SpecialBorder = this.SpecialBorder;
		expandoInfo.NormalBorder = this.NormalBorder;

		expandoInfo.SpecialBorderColor = Tools.Drawing.ConvertStringToColor(this.SpecialBorderColor);
		expandoInfo.NormalBorderColor = Tools.Drawing.ConvertStringToColor(this.NormalBorderColor);

		expandoInfo.SpecialPadding = this.SpecialPadding;
		expandoInfo.NormalPadding = this.NormalPadding;

		expandoInfo.SpecialBackImage = this.SpecialBackImage.ToImage();
		expandoInfo.NormalBackImage = this.NormalBackImage.ToImage();

		expandoInfo.WatermarkAlignment = this.WatermarkAlignment;

		return expandoInfo;
	}


	/// <summary>
	/// Populates a SerializationInfo with the data needed to serialize the ExpandoInfoSurrogate
	/// </summary>
	/// <param name="info">The SerializationInfo to populate with data</param>
	/// <param name="context">The destination for this serialization</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		info.AddValue("Version", this.Version);

		info.AddValue("SpecialBackColor", this.SpecialBackColor);
		info.AddValue("NormalBackColor", this.NormalBackColor);

		info.AddValue("SpecialBorder", this.SpecialBorder);
		info.AddValue("NormalBorder", this.NormalBorder);

		info.AddValue("SpecialBorderColor", this.SpecialBorderColor);
		info.AddValue("NormalBorderColor", this.NormalBorderColor);

		info.AddValue("SpecialPadding", this.SpecialPadding);
		info.AddValue("NormalPadding", this.NormalPadding);

		info.AddValue("SpecialBackImage", this.SpecialBackImage);
		info.AddValue("NormalBackImage", this.NormalBackImage);

		info.AddValue("WatermarkAlignment", this.WatermarkAlignment);
	}


	/// <summary>
	/// Initializes a new instance of the ExpandoInfoSurrogate class using the information 
	/// in the SerializationInfo
	/// </summary>
	/// <param name="info">The information to populate the ExpandoInfoSurrogate</param>
	/// <param name="context">The source from which the ExpandoInfoSurrogate is deserialized</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	protected ExpandoInfoSurrogate(SerializationInfo info, StreamingContext context)
		: base() {
		int version = info.GetInt32("Version");

		this.SpecialBackColor = info.GetString("SpecialBackColor");
		this.NormalBackColor = info.GetString("NormalBackColor");

		this.SpecialBorder = (Border)info.GetValue("SpecialBorder", typeof(Border));
		this.NormalBorder = (Border)info.GetValue("NormalBorder", typeof(Border));

		this.SpecialBorderColor = info.GetString("SpecialBorderColor");
		this.NormalBorderColor = info.GetString("NormalBorderColor");

		this.SpecialPadding = (PaddingEx)info.GetValue("SpecialPadding", typeof(PaddingEx));
		this.NormalPadding = (PaddingEx)info.GetValue("NormalPadding", typeof(PaddingEx));

		this.SpecialBackImage = (byte[])info.GetValue("SpecialBackImage", typeof(byte[]));
		this.NormalBackImage = (byte[])info.GetValue("NormalBackImage", typeof(byte[]));

		this.WatermarkAlignment = (ContentAlignment)info.GetValue("WatermarkAlignment", typeof(ContentAlignment));
	}

	#endregion

}
