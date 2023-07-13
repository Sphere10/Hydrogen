// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Security.Permissions;
using System.Collections;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Reflection;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that is serialized instead of a TaskPane (as 
/// TaskPanes contain objects that cause serialization problems)
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable()]
[XmlRoot("TaskPaneSurrogate", Namespace = "", IsNullable = false)]
public class TaskPaneSurrogate : ISerializable {

	#region Class Data

	/// <summary>
	/// See TaskPane.Name.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string Name;

	/// <summary>
	/// See TaskPane.Size.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public Size Size;

	/// <summary>
	/// See TaskPane.Location.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public Point Location;

	/// <summary>
	/// See TaskPane.BackColor.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string BackColor;

	/// <summary>
	/// See TaskPane.CustomSettings.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public TaskPaneInfoSurrogate CustomSettings;

	/// <summary>
	/// See TaskPane.AutoScroll.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool AutoScroll;

	/// <summary>
	/// See TaskPane.AutoScrollMargin.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public Size AutoScrollMargin;

	/// <summary>
	/// See TaskPane.Enabled.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool Enabled;

	/// <summary>
	/// See TaskPane.Visible.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool Visible;

	/// <summary>
	/// See TaskPane.Anchor.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public AnchorStyles Anchor;

	/// <summary>
	/// See TaskPane.Dock.  This member is not intended to be used 
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
	/// See TaskPane.Expandos.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	[XmlArray("Expandos"), XmlArrayItem("ExpandoSurrogate", typeof(ExpandoSurrogate))]
	public ArrayList Expandos;

	/// <summary>
	/// See Control.Tag.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	[XmlElementAttribute("Tag", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] Tag;

	/// <summary>
	/// See TaskPane.AllowExpandoDragging.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool AllowExpandoDragging;

	/// <summary>
	/// See TaskPane.ExpandoDropIndicatorColor.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string ExpandoDropIndicatorColor;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the TaskPaneSurrogate class with default settings
	/// </summary>
	public TaskPaneSurrogate() {
		this.Name = null;

		this.Size = Size.Empty;
		this.Location = Point.Empty;

		this.BackColor = Tools.Drawing.ConvertColorToString(SystemColors.Control);

		this.CustomSettings = null;

		this.AutoScroll = false;
		this.AutoScrollMargin = Size.Empty;

		this.Enabled = true;
		this.Visible = true;

		this.Anchor = AnchorStyles.None;
		this.Dock = DockStyle.None;

		this.FontName = "Tahoma";
		this.FontSize = 8.25f;
		this.FontDecoration = FontStyle.Regular;

		this.Tag = new byte[0];

		this.AllowExpandoDragging = false;
		this.ExpandoDropIndicatorColor = Tools.Drawing.ConvertColorToString(Color.Red);

		this.Expandos = new ArrayList();
	}

	#endregion


	#region Methods

	/// <summary>
	/// Populates the TaskPaneSurrogate with data that is to be 
	/// serialized from the specified TaskPane
	/// </summary>
	/// <param name="taskPane">The TaskPane that contains the data 
	/// to be serialized</param>
	public void Load(TaskPane taskPane) {
		this.Name = taskPane.Name;
		this.Size = taskPane.Size;
		this.Location = taskPane.Location;

		this.BackColor = Tools.Drawing.ConvertColorToString(taskPane.BackColor);

		this.CustomSettings = new TaskPaneInfoSurrogate();
		this.CustomSettings.Load(taskPane.CustomSettings);

		this.AutoScroll = taskPane.AutoScroll;
		this.AutoScrollMargin = taskPane.AutoScrollMargin;

		this.Enabled = taskPane.Enabled;
		this.Visible = taskPane.Visible;

		this.Anchor = taskPane.Anchor;
		this.Dock = taskPane.Dock;

		this.FontName = taskPane.Font.FontFamily.Name;
		this.FontSize = taskPane.Font.SizeInPoints;
		this.FontDecoration = taskPane.Font.Style;

		this.AllowExpandoDragging = taskPane.AllowExpandoDragging;
		this.ExpandoDropIndicatorColor = Tools.Drawing.ConvertColorToString(taskPane.ExpandoDropIndicatorColor);

		this.Tag = Tools.Object.SerializeToByteArray(taskPane.Tag);

		foreach (Expando expando in taskPane.Expandos) {
			ExpandoSurrogate es = new ExpandoSurrogate();

			es.Load(expando);

			this.Expandos.Add(es);
		}
	}


	/// <summary>
	/// Returns a TaskPane that contains the deserialized TaskPaneSurrogate data
	/// </summary>
	/// <returns>A TaskPane that contains the deserialized TaskPaneSurrogate data</returns>
	public TaskPane Save() {
		TaskPane taskPane = new TaskPane();
		((ISupportInitialize)taskPane).BeginInit();
		taskPane.SuspendLayout();

		taskPane.Name = this.Name;
		taskPane.Size = this.Size;
		taskPane.Location = this.Location;

		taskPane.BackColor = Tools.Drawing.ConvertStringToColor(this.BackColor);

		taskPane.CustomSettings = this.CustomSettings.Save();
		taskPane.CustomSettings.TaskPane = taskPane;

		taskPane.AutoScroll = this.AutoScroll;
		taskPane.AutoScrollMargin = this.AutoScrollMargin;

		taskPane.Enabled = this.Enabled;
		taskPane.Visible = this.Visible;

		taskPane.Anchor = this.Anchor;
		taskPane.Dock = this.Dock;

		taskPane.Font = new Font(this.FontName, this.FontSize, this.FontDecoration);

		taskPane.Tag = Tools.Object.SerializeToByteArray(this.Tag);

		taskPane.AllowExpandoDragging = this.AllowExpandoDragging;
		taskPane.ExpandoDropIndicatorColor = Tools.Drawing.ConvertStringToColor(this.ExpandoDropIndicatorColor);

		foreach (Object o in this.Expandos) {
			Expando e = ((ExpandoSurrogate)o).Save();

			taskPane.Expandos.Add(e);
		}

		((ISupportInitialize)taskPane).EndInit();
		taskPane.ResumeLayout(false);

		return taskPane;
	}


	/// <summary>
	/// Populates a SerializationInfo with the data needed to serialize the TaskPaneSurrogate
	/// </summary>
	/// <param name="info">The SerializationInfo to populate with data</param>
	/// <param name="context">The destination for this serialization</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context) {

		info.AddValue("Name", this.Name);
		info.AddValue("Size", this.Size);
		info.AddValue("Location", this.Location);

		info.AddValue("BackColor", this.BackColor);

		info.AddValue("CustomSettings", this.CustomSettings);

		info.AddValue("AutoScroll", this.AutoScroll);
		info.AddValue("AutoScrollMargin", this.AutoScrollMargin);

		info.AddValue("Enabled", this.Enabled);
		info.AddValue("Visible", this.Visible);

		info.AddValue("Anchor", this.Anchor);
		info.AddValue("Dock", this.Dock);

		info.AddValue("FontName", this.FontName);
		info.AddValue("FontSize", this.FontSize);
		info.AddValue("FontDecoration", this.FontDecoration);

		info.AddValue("AllowExpandoDragging", this.AllowExpandoDragging);
		info.AddValue("ExpandoDropIndicatorColor", this.ExpandoDropIndicatorColor);

		info.AddValue("Tag", this.Tag);

		info.AddValue("Expandos", this.Expandos);
	}


	/// <summary>
	/// Initializes a new instance of the TaskPaneSurrogate class using the information 
	/// in the SerializationInfo
	/// </summary>
	/// <param name="info">The information to populate the TaskPaneSurrogate</param>
	/// <param name="context">The source from which the TaskPaneSurrogate is deserialized</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	protected TaskPaneSurrogate(SerializationInfo info, StreamingContext context)
		: base() {
		int version = info.GetInt32("Version");

		this.Name = info.GetString("Name");
		this.Size = (Size)info.GetValue("Size", typeof(Size));
		this.Location = (Point)info.GetValue("Location", typeof(Point));

		this.BackColor = info.GetString("BackColor");

		this.CustomSettings = (TaskPaneInfoSurrogate)info.GetValue("CustomSettings", typeof(TaskPaneInfoSurrogate));

		this.AutoScroll = info.GetBoolean("AutoScroll");
		this.AutoScrollMargin = (Size)info.GetValue("AutoScrollMargin", typeof(Size));

		this.Enabled = info.GetBoolean("Enabled");
		this.Visible = info.GetBoolean("Visible");

		this.Anchor = (AnchorStyles)info.GetValue("Anchor", typeof(AnchorStyles));
		this.Dock = (DockStyle)info.GetValue("Dock", typeof(DockStyle));

		this.FontName = info.GetString("FontName");
		this.FontSize = info.GetSingle("FontSize");
		this.FontDecoration = (FontStyle)info.GetValue("FontDecoration", typeof(FontStyle));

		if (version >= 3300) {
			this.AllowExpandoDragging = info.GetBoolean("AllowExpandoDragging");
			this.ExpandoDropIndicatorColor = info.GetString("ExpandoDropIndicatorColor");
		}

		this.Tag = (byte[])info.GetValue("Tag", typeof(byte[]));

		this.Expandos = (ArrayList)info.GetValue("Expandos", typeof(ArrayList));
	}

	#endregion

}
