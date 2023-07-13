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
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Reflection;


namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that is serialized instead of a TaskItem (as 
/// TaskItems contain objects that cause serialization problems)
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable()]
public class TaskItemSurrogate : ISerializable {

	#region Class Data

	/// <summary>
	/// See TaskItem.Name.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string Name;

	/// <summary>
	/// See TaskItem.Size.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public Size Size;

	/// <summary>
	/// See TaskItem.Location.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public Point Location;

	/// <summary>
	/// See TaskItem.BackColor.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string BackColor;

	/// <summary>
	/// See TaskItem.CustomSettings.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public TaskItemInfoSurrogate CustomSettings;

	/// <summary>
	/// See TaskItem.Text.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string Text;

	/// <summary>
	/// See TaskItem.ShowFocusCues.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool ShowFocusCues;

	/// <summary>
	/// See TaskItem.Image.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	[XmlElementAttribute("TaskItemImage", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] Image;

	/// <summary>
	/// See TaskItem.Enabled.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool Enabled;

	/// <summary>
	/// See TaskItem.Visible.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool Visible;

	/// <summary>
	/// See TaskItem.Anchor.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public AnchorStyles Anchor;

	/// <summary>
	/// See TaskItem.Dock.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public DockStyle Dock;

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
	public FontStyle FontDecoration;


	/// <summary>
	/// See Control.Tag.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	[XmlElementAttribute("Tag", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] Tag;

	/// <summary>
	/// Version number of the surrogate.  This member is not intended 
	/// to be used directly from your code.
	/// </summary>
	public int Version = 3300;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the TaskItemSurrogate class with default settings
	/// </summary>
	public TaskItemSurrogate() {
		this.Name = null;

		this.Size = Size.Empty;
		this.Location = Point.Empty;

		this.BackColor = Tools.Drawing.ConvertColorToString(Color.Empty);

		this.CustomSettings = null;

		this.Text = null;
		this.ShowFocusCues = false;
		this.Image = new byte[0];

		this.Enabled = true;
		this.Visible = true;

		this.Anchor = AnchorStyles.None;
		this.Dock = DockStyle.None;

		this.FontName = null;
		this.FontSize = 8.25f;
		this.FontDecoration = FontStyle.Regular;

		this.Tag = new byte[0];
	}

	#endregion


	#region Methods

	/// <summary>
	/// Populates the TaskItemSurrogate with data that is to be 
	/// serialized from the specified TaskItem
	/// </summary>
	/// <param name="taskItem">The TaskItem that contains the data 
	/// to be serialized</param>
	public void Load(TaskItem taskItem) {
		this.Name = taskItem.Name;
		this.Size = taskItem.Size;
		this.Location = taskItem.Location;

		this.BackColor = Tools.Drawing.ConvertColorToString(taskItem.BackColor);

		this.CustomSettings = new TaskItemInfoSurrogate();
		this.CustomSettings.Load(taskItem.CustomSettings);

		this.Text = taskItem.Text;
		this.ShowFocusCues = taskItem.ShowFocusCues;
		this.Image = taskItem.Image.ToByteArray();

		this.Enabled = taskItem.Enabled;
		this.Visible = taskItem.Visible;

		this.Anchor = taskItem.Anchor;
		this.Dock = taskItem.Dock;

		this.FontName = taskItem.Font.FontFamily.Name;
		this.FontSize = taskItem.Font.SizeInPoints;
		this.FontDecoration = taskItem.Font.Style;

		this.Tag = Tools.Object.SerializeToByteArray(taskItem.Tag);
	}


	/// <summary>
	/// Returns a TaskItem that contains the deserialized TaskItemSurrogate data
	/// </summary>
	/// <returns>A TaskItem that contains the deserialized TaskItemSurrogate data</returns>
	public TaskItem Save() {
		TaskItem taskItem = new TaskItem();

		taskItem.Name = this.Name;
		taskItem.Size = this.Size;
		taskItem.Location = this.Location;

		taskItem.BackColor = Tools.Drawing.ConvertStringToColor(this.BackColor);

		taskItem.CustomSettings = this.CustomSettings.Save();
		taskItem.CustomSettings.TaskItem = taskItem;

		taskItem.Text = this.Text;
		taskItem.ShowFocusCues = this.ShowFocusCues;
		taskItem.Image = this.Image.ToImage();

		taskItem.Enabled = this.Enabled;
		taskItem.Visible = this.Visible;

		taskItem.Anchor = this.Anchor;
		taskItem.Dock = this.Dock;

		taskItem.Font = new Font(this.FontName, this.FontSize, this.FontDecoration);

		taskItem.Tag = this.Tag.ToImage();

		return taskItem;
	}


	/// <summary>
	/// Populates a SerializationInfo with the data needed to serialize the TaskItemSurrogate
	/// </summary>
	/// <param name="info">The SerializationInfo to populate with data</param>
	/// <param name="context">The destination for this serialization</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		info.AddValue("Version", this.Version);

		info.AddValue("Name", this.Name);
		info.AddValue("Size", this.Size);
		info.AddValue("Location", this.Location);

		info.AddValue("BackColor", this.BackColor);

		info.AddValue("CustomSettings", this.CustomSettings);

		info.AddValue("Text", this.Text);
		info.AddValue("ShowFocusCues", this.ShowFocusCues);
		info.AddValue("Image", this.Image);

		info.AddValue("Enabled", this.Enabled);
		info.AddValue("Visible", this.Visible);

		info.AddValue("Anchor", this.Anchor);
		info.AddValue("Dock", this.Dock);

		info.AddValue("FontName", this.FontName);
		info.AddValue("FontSize", this.FontSize);
		info.AddValue("FontDecoration", this.FontDecoration);

		info.AddValue("Tag", this.Tag);
	}


	/// <summary>
	/// Initializes a new instance of the TaskItemSurrogate class using the information 
	/// in the SerializationInfo
	/// </summary>
	/// <param name="info">The information to populate the TaskItemSurrogate</param>
	/// <param name="context">The source from which the TaskItemSurrogate is deserialized</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	protected TaskItemSurrogate(SerializationInfo info, StreamingContext context)
		: base() {

		this.Name = info.GetString("Name");
		this.Size = (Size)info.GetValue("Size", typeof(Size));
		this.Location = (Point)info.GetValue("Location", typeof(Point));

		this.BackColor = info.GetString("BackColor");

		this.CustomSettings = (TaskItemInfoSurrogate)info.GetValue("CustomSettings", typeof(TaskItemInfoSurrogate));

		this.Text = info.GetString("Text");
		this.ShowFocusCues = info.GetBoolean("ShowFocusCues");
		this.Image = (byte[])info.GetValue("Image", typeof(byte[]));

		this.Enabled = info.GetBoolean("Enabled");
		this.Visible = info.GetBoolean("Visible");

		this.Anchor = (AnchorStyles)info.GetValue("Anchor", typeof(AnchorStyles));
		this.Dock = (DockStyle)info.GetValue("Dock", typeof(DockStyle));

		this.FontName = info.GetString("FontName");
		this.FontSize = info.GetSingle("FontSize");
		this.FontDecoration = (FontStyle)info.GetValue("FontDecoration", typeof(FontStyle));


		this.Tag = (byte[])info.GetValue("Tag", typeof(byte[]));
	}

	#endregion

}
