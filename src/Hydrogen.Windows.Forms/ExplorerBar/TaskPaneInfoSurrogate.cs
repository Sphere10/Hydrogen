// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Reflection;


namespace Hydrogen.Windows.Forms;

#region TaskPaneInfoSurrogate

/// <summary>
/// A class that is serialized instead of a TaskPaneInfo (as 
/// TaskPaneInfos contain objects that cause serialization problems)
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable()]
public class TaskPaneInfoSurrogate : ISerializable {

	#region Class Data

	/// <summary>
	/// See TaskPaneInfo.GradientStartColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string GradientStartColor;

	/// <summary>
	/// See TaskPaneInfo.GradientEndColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string GradientEndColor;

	/// <summary>
	/// See TaskPaneInfo.GradientDirection.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public LinearGradientMode GradientDirection;

	/// <summary>
	/// See TaskPaneInfo.Padding.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public PaddingEx Padding;

	/// <summary>
	/// See TaskPaneInfo.BackImage.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("BackImage", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] BackImage;

	/// <summary>
	/// See TaskPaneInfo.StretchMode.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public ImageStretchMode StretchMode;

	/// <summary>
	/// See TaskPaneInfo.Watermark.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	[XmlElementAttribute("Watermark", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] Watermark;

	/// <summary>
	/// See TaskPaneInfo.WatermarkAlignment.  This member is not 
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
	/// Initializes a new instance of the TaskPaneInfoSurrogate class with default settings
	/// </summary>
	public TaskPaneInfoSurrogate() {
		this.GradientStartColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.GradientEndColor = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.GradientDirection = LinearGradientMode.Vertical;

		this.Padding = PaddingEx.Empty;

		this.BackImage = new byte[0];
		this.StretchMode = ImageStretchMode.Normal;

		this.Watermark = new byte[0];
		this.WatermarkAlignment = ContentAlignment.BottomCenter;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Populates the TaskPaneInfoSurrogate with data that is to be 
	/// serialized from the specified TaskPaneInfo
	/// </summary>
	/// <param name="taskPaneInfo">The TaskPaneInfo that contains the data 
	/// to be serialized</param>
	public void Load(TaskPaneInfo taskPaneInfo) {
		this.GradientStartColor = Tools.Drawing.ConvertColorToString(taskPaneInfo.GradientStartColor);
		this.GradientEndColor = Tools.Drawing.ConvertColorToString(taskPaneInfo.GradientEndColor);
		this.GradientDirection = taskPaneInfo.GradientDirection;

		this.Padding = taskPaneInfo.Padding;

		this.BackImage = taskPaneInfo.BackImage.ToByteArray();
		this.StretchMode = taskPaneInfo.StretchMode;

		this.Watermark = taskPaneInfo.Watermark.ToByteArray();
		this.WatermarkAlignment = taskPaneInfo.WatermarkAlignment;
	}


	/// <summary>
	/// Returns a TaskPaneInfo that contains the deserialized TaskPaneInfoSurrogate data
	/// </summary>
	/// <returns>A TaskPaneInfo that contains the deserialized TaskPaneInfoSurrogate data</returns>
	public TaskPaneInfo Save() {
		TaskPaneInfo taskPaneInfo = new TaskPaneInfo();

		taskPaneInfo.GradientStartColor = Tools.Drawing.ConvertStringToColor(this.GradientStartColor);
		taskPaneInfo.GradientEndColor = Tools.Drawing.ConvertStringToColor(this.GradientEndColor);
		taskPaneInfo.GradientDirection = this.GradientDirection;

		taskPaneInfo.Padding = this.Padding;

		taskPaneInfo.BackImage = this.BackImage.ToImage();
		taskPaneInfo.StretchMode = this.StretchMode;

		taskPaneInfo.Watermark = this.Watermark.ToImage();
		taskPaneInfo.WatermarkAlignment = this.WatermarkAlignment;

		return taskPaneInfo;
	}


	/// <summary>
	/// Populates a SerializationInfo with the data needed to serialize the TaskPaneInfoSurrogate
	/// </summary>
	/// <param name="info">The SerializationInfo to populate with data</param>
	/// <param name="context">The destination for this serialization</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		info.AddValue("Version", this.Version);

		info.AddValue("GradientStartColor", this.GradientStartColor);
		info.AddValue("GradientEndColor", this.GradientEndColor);
		info.AddValue("GradientDirection", this.GradientDirection);

		info.AddValue("Padding", this.Padding);

		info.AddValue("BackImage", this.BackImage);
		info.AddValue("StretchMode", this.StretchMode);

		info.AddValue("Watermark", this.Watermark);
		info.AddValue("WatermarkAlignment", this.WatermarkAlignment);
	}


	/// <summary>
	/// Initializes a new instance of the TaskPaneInfoSurrogate class using the information 
	/// in the SerializationInfo
	/// </summary>
	/// <param name="info">The information to populate the TaskPaneInfoSurrogate</param>
	/// <param name="context">The source from which the TaskPaneInfoSurrogate is deserialized</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	protected TaskPaneInfoSurrogate(SerializationInfo info, StreamingContext context)
		: base() {
		int version = info.GetInt32("Version");

		this.GradientStartColor = info.GetString("GradientStartColor");
		this.GradientEndColor = info.GetString("GradientEndColor");
		this.GradientDirection = (LinearGradientMode)info.GetValue("GradientDirection", typeof(LinearGradientMode));

		this.Padding = (PaddingEx)info.GetValue("Padding", typeof(PaddingEx));

		this.BackImage = (byte[])info.GetValue("BackImage", typeof(byte[]));
		this.StretchMode = (ImageStretchMode)info.GetValue("StretchMode", typeof(ImageStretchMode));

		this.Watermark = (byte[])info.GetValue("Watermark", typeof(byte[]));
		this.WatermarkAlignment = (ContentAlignment)info.GetValue("WatermarkAlignment", typeof(ContentAlignment));
	}

	#endregion

}

#endregion
