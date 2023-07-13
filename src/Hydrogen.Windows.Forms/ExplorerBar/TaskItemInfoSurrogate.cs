// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.Serialization;
using System.Drawing;
using System.Security.Permissions;
using System.Reflection;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that is serialized instead of a TaskItemInfo (as 
/// TaskItemInfos contain objects that cause serialization problems)
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable()]
public class TaskItemInfoSurrogate : ISerializable {

	#region Class Data

	/// <summary>
	/// See TaskItemInfo.Padding.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public PaddingEx Padding;

	/// <summary>
	/// See TaskItemInfo.Margin.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public Margin Margin;

	/// <summary>
	/// See TaskItemInfo.LinkColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string LinkNormal;

	/// <summary>
	/// See TaskItemInfo.HotLinkColor.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public string LinkHot;

	/// <summary>
	/// See TaskItemInfo.FontDecoration.  This member is not 
	/// intended to be used directly from your code.
	/// </summary>
	public FontStyle FontDecoration;

	/// <summary>
	/// Version number of the surrogate.  This member is not intended 
	/// to be used directly from your code.
	/// </summary>
	public int Version = 3300;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the TaskItemInfoSurrogate class with default settings
	/// </summary>
	public TaskItemInfoSurrogate() {
		this.Padding = PaddingEx.Empty;
		this.Margin = Margin.Empty;

		this.LinkNormal = Tools.Drawing.ConvertColorToString(Color.Empty);
		this.LinkHot = Tools.Drawing.ConvertColorToString(Color.Empty);

		this.FontDecoration = FontStyle.Regular;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Populates the TaskItemInfoSurrogate with data that is to be 
	/// serialized from the specified TaskItemInfo
	/// </summary>
	/// <param name="taskItemInfo">The TaskItemInfo that contains the data 
	/// to be serialized</param>
	public void Load(TaskItemInfo taskItemInfo) {
		this.Padding = taskItemInfo.Padding;
		this.Margin = taskItemInfo.Margin;

		this.LinkNormal = Tools.Drawing.ConvertColorToString(taskItemInfo.LinkColor);
		this.LinkHot = Tools.Drawing.ConvertColorToString(taskItemInfo.HotLinkColor);

		this.FontDecoration = taskItemInfo.FontDecoration;
	}


	/// <summary>
	/// Returns a TaskItemInfo that contains the deserialized TaskItemInfoSurrogate data
	/// </summary>
	/// <returns>A TaskItemInfo that contains the deserialized TaskItemInfoSurrogate data</returns>
	public TaskItemInfo Save() {
		TaskItemInfo taskItemInfo = new TaskItemInfo();

		taskItemInfo.Padding = this.Padding;
		taskItemInfo.Margin = this.Margin;

		taskItemInfo.LinkColor = Tools.Drawing.ConvertStringToColor(this.LinkNormal);
		taskItemInfo.HotLinkColor = Tools.Drawing.ConvertStringToColor(this.LinkHot);

		taskItemInfo.FontDecoration = this.FontDecoration;

		return taskItemInfo;
	}


	/// <summary>
	/// Populates a SerializationInfo with the data needed to serialize the TaskItemInfoSurrogate
	/// </summary>
	/// <param name="info">The SerializationInfo to populate with data</param>
	/// <param name="context">The destination for this serialization</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		info.AddValue("Version", this.Version);

		info.AddValue("Padding", this.Padding);
		info.AddValue("Margin", this.Margin);

		info.AddValue("LinkNormal", this.LinkNormal);
		info.AddValue("LinkHot", this.LinkHot);

		info.AddValue("FontDecoration", this.FontDecoration);
	}


	/// <summary>
	/// Initializes a new instance of the TaskItemInfoSurrogate class using the information 
	/// in the SerializationInfo
	/// </summary>
	/// <param name="info">The information to populate the TaskItemInfoSurrogate</param>
	/// <param name="context">The source from which the TaskItemInfoSurrogate is deserialized</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	protected TaskItemInfoSurrogate(SerializationInfo info, StreamingContext context)
		: base() {
		int version = info.GetInt32("Version");

		this.Padding = (PaddingEx)info.GetValue("Padding", typeof(PaddingEx));
		this.Margin = (Margin)info.GetValue("Margin", typeof(Margin));

		this.LinkNormal = info.GetString("LinkNormal");
		this.LinkHot = info.GetString("LinkHot");

		this.FontDecoration = (FontStyle)info.GetValue("FontDecoration", typeof(FontStyle));
	}

	#endregion

}
