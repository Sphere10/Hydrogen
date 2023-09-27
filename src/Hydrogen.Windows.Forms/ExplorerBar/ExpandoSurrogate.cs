// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Permissions;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Reflection;


namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that is serialized instead of an Expando (as 
/// Expandos contain objects that cause serialization problems)
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable()]
public class ExpandoSurrogate : ISerializable {

	#region Class Data

	/// <summary>
	/// See Expando.Name.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string Name;

	/// <summary>
	/// See Expando.Text.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string Text;

	/// <summary>
	/// See Expando.Size.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public Size Size;

	/// <summary>
	/// See Expando.Location.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public Point Location;

	/// <summary>
	/// See Expando.BackColor.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public string BackColor;

	/// <summary>
	/// See Expando.ExpandedHeight.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public int ExpandedHeight;

	/// <summary>
	/// See Expando.CustomSettings.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public ExpandoInfoSurrogate CustomSettings;

	/// <summary>
	/// See Expando.CustomHeaderSettings.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public HeaderInfoSurrogate CustomHeaderSettings;

	/// <summary>
	/// See Expando.Animate.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool Animate;

	/// <summary>
	/// See Expando.ShowFocusCues.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool ShowFocusCues;

	/// <summary>
	/// See Expando.Collapsed.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool Collapsed;

	/// <summary>
	/// See Expando.CanCollapse.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool CanCollapse;

	/// <summary>
	/// See Expando.SpecialGroup.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool SpecialGroup;

	/// <summary>
	/// See Expando.TitleImage.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	[XmlElement("TitleImage", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] TitleImage;

	/// <summary>
	/// See Expando.Watermark.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	[XmlElement("Watermark", typeof(Byte[]), DataType = "base64Binary")]
	public byte[] Watermark;

	/// <summary>
	/// See Expando.Enabled.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool Enabled;

	/// <summary>
	/// See Expando.Visible.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool Visible;

	/// <summary>
	/// See Expando.AutoLayout.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public bool AutoLayout;

	/// <summary>
	/// See Expando.Anchor.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	public AnchorStyles Anchor;

	/// <summary>
	/// See Expando.Dock.  This member is not intended to be used 
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
	/// See Expando.Items.  This member is not intended to be used 
	/// directly from your code.
	/// </summary>
	[XmlArray("Items"), XmlArrayItem("TaskItemSurrogate", typeof(TaskItemSurrogate))]
	public ArrayList Items;

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
	/// Initializes a new instance of the ExpandoSurrogate class with default settings
	/// </summary>
	public ExpandoSurrogate() {
		this.Name = null;
		this.Text = null;
		this.Size = Size.Empty;
		this.Location = Point.Empty;

		this.BackColor = Tools.Drawing.ConvertColorToString(SystemColors.Control);
		this.ExpandedHeight = -1;

		this.CustomSettings = null;
		this.CustomHeaderSettings = null;

		this.Animate = false;
		this.ShowFocusCues = false;
		this.Collapsed = false;
		this.CanCollapse = true;
		this.SpecialGroup = false;

		this.TitleImage = new byte[0];
		this.Watermark = new byte[0];

		this.Enabled = true;
		this.Visible = true;
		this.AutoLayout = false;

		this.Anchor = AnchorStyles.None;
		this.Dock = DockStyle.None;

		this.FontName = "Tahoma";
		this.FontSize = 8.25f;
		this.FontDecoration = FontStyle.Regular;

		this.Items = new ArrayList();

		this.Tag = new byte[0];
	}

	#endregion


	#region Methods

	/// <summary>
	/// Populates the ExpandoSurrogate with data that is to be 
	/// serialized from the specified Expando
	/// </summary>
	/// <param name="expando">The Expando that contains the data 
	/// to be serialized</param>
	public void Load(Expando expando) {
		this.Name = expando.Name;
		this.Text = expando.Text;
		this.Size = expando.Size;
		this.Location = expando.Location;

		this.BackColor = Tools.Drawing.ConvertColorToString(expando.BackColor);
		this.ExpandedHeight = expando.ExpandedHeight;

		this.CustomSettings = new ExpandoInfoSurrogate();
		this.CustomSettings.Load(expando.CustomSettings);
		this.CustomHeaderSettings = new HeaderInfoSurrogate();
		this.CustomHeaderSettings.Load(expando.CustomHeaderSettings);

		this.Animate = expando.Animate;
		this.ShowFocusCues = expando.ShowFocusCues;
		this.Collapsed = expando.Collapsed;
		this.CanCollapse = expando.CanCollapse;
		this.SpecialGroup = expando.SpecialGroup;

		this.TitleImage = expando.TitleImage.ToByteArray();
		this.Watermark = expando.Watermark.ToByteArray();

		this.Enabled = expando.Enabled;
		this.Visible = expando.Visible;
		this.AutoLayout = expando.AutoLayout;

		this.Anchor = expando.Anchor;
		this.Dock = expando.Dock;

		this.FontName = expando.Font.FontFamily.Name;
		this.FontSize = expando.Font.SizeInPoints;
		this.FontDecoration = expando.Font.Style;

		this.Tag = Tools.Object.SerializeToByteArray(expando);

		for (int i = 0; i < expando.Items.Count; i++) {
			if (expando.Items[i] is TaskItem) {
				TaskItemSurrogate tis = new TaskItemSurrogate();

				tis.Load((TaskItem)expando.Items[i]);

				this.Items.Add(tis);
			}
		}
	}


	/// <summary>
	/// Returns an Expando that contains the deserialized ExpandoSurrogate data
	/// </summary>
	/// <returns>An Expando that contains the deserialized ExpandoSurrogate data</returns>
	public Expando Save() {
		Expando expando = new Expando();
		((ISupportInitialize)expando).BeginInit();
		expando.SuspendLayout();

		expando.Name = this.Name;
		expando.Text = this.Text;
		expando.Size = this.Size;
		expando.Location = this.Location;

		expando.BackColor = Tools.Drawing.ConvertStringToColor(this.BackColor);
		expando.ExpandedHeight = this.ExpandedHeight;

		expando.CustomSettings = this.CustomSettings.Save();
		expando.CustomSettings.Expando = expando;
		expando.CustomHeaderSettings = this.CustomHeaderSettings.Save();
		expando.CustomHeaderSettings.Expando = expando;

		expando.TitleImage = this.TitleImage.ToImage();
		expando.Watermark = this.Watermark.ToImage();

		expando.Animate = this.Animate;
		expando.ShowFocusCues = this.ShowFocusCues;
		expando.Collapsed = this.Collapsed;
		expando.CanCollapse = this.CanCollapse;
		expando.SpecialGroup = this.SpecialGroup;

		expando.Enabled = this.Enabled;
		expando.Visible = this.Visible;
		expando.AutoLayout = this.AutoLayout;

		expando.Anchor = this.Anchor;
		expando.Dock = this.Dock;

		expando.Font = new Font(this.FontName, this.FontSize, this.FontDecoration);

		expando.Tag = Tools.Object.SerializeToByteArray(Tag);

		foreach (Object o in this.Items) {
			TaskItem ti = ((TaskItemSurrogate)o).Save();

			expando.Items.Add(ti);
		}

		((ISupportInitialize)expando).EndInit();
		expando.ResumeLayout(false);

		return expando;
	}


	/// <summary>
	/// Populates a SerializationInfo with the data needed to serialize the ExpandoSurrogate
	/// </summary>
	/// <param name="info">The SerializationInfo to populate with data</param>
	/// <param name="context">The destination for this serialization</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		info.AddValue("Version", this.Version);

		info.AddValue("Name", this.Name);
		info.AddValue("Text", this.Text);
		info.AddValue("Size", this.Size);
		info.AddValue("Location", this.Location);

		info.AddValue("BackColor", this.BackColor);
		info.AddValue("ExpandedHeight", this.ExpandedHeight);

		info.AddValue("CustomSettings", this.CustomSettings);
		info.AddValue("CustomHeaderSettings", this.CustomHeaderSettings);

		info.AddValue("Animate", this.Animate);
		info.AddValue("ShowFocusCues", this.ShowFocusCues);
		info.AddValue("Collapsed", this.Collapsed);
		info.AddValue("CanCollapse", this.CanCollapse);
		info.AddValue("SpecialGroup", this.SpecialGroup);

		info.AddValue("TitleImage", this.TitleImage);
		info.AddValue("Watermark", this.Watermark);

		info.AddValue("Enabled", this.Enabled);
		info.AddValue("Visible", this.Visible);
		info.AddValue("AutoLayout", this.AutoLayout);

		info.AddValue("Anchor", this.Anchor);
		info.AddValue("Dock", this.Dock);

		info.AddValue("FontName", this.FontName);
		info.AddValue("FontSize", this.FontSize);
		info.AddValue("FontDecoration", this.FontDecoration);

		info.AddValue("Tag", this.Tag);

		info.AddValue("Items", this.Items);
	}


	/// <summary>
	/// Initializes a new instance of the ExpandoSurrogate class using the information 
	/// in the SerializationInfo
	/// </summary>
	/// <param name="info">The information to populate the ExpandoSurrogate</param>
	/// <param name="context">The source from which the ExpandoSurrogate is deserialized</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	protected ExpandoSurrogate(SerializationInfo info, StreamingContext context)
		: base() {
		int version = info.GetInt32("Version");

		this.Name = info.GetString("Name");
		this.Text = info.GetString("Text");
		this.Size = (Size)info.GetValue("Size", typeof(Size));
		this.Location = (Point)info.GetValue("Location", typeof(Point));

		this.BackColor = info.GetString("BackColor");
		this.ExpandedHeight = info.GetInt32("ExpandedHeight");

		this.CustomSettings = (ExpandoInfoSurrogate)info.GetValue("CustomSettings", typeof(ExpandoInfoSurrogate));
		this.CustomHeaderSettings = (HeaderInfoSurrogate)info.GetValue("CustomHeaderSettings", typeof(HeaderInfoSurrogate));

		this.Animate = info.GetBoolean("Animate");
		this.ShowFocusCues = info.GetBoolean("ShowFocusCues");
		this.Collapsed = info.GetBoolean("Collapsed");
		this.CanCollapse = info.GetBoolean("CanCollapse");
		this.SpecialGroup = info.GetBoolean("SpecialGroup");

		this.TitleImage = (byte[])info.GetValue("TitleImage", typeof(byte[]));
		this.Watermark = (byte[])info.GetValue("Watermark", typeof(byte[]));

		this.Enabled = info.GetBoolean("Enabled");
		this.Visible = info.GetBoolean("Visible");
		this.AutoLayout = info.GetBoolean("AutoLayout");

		this.Anchor = (AnchorStyles)info.GetValue("Anchor", typeof(AnchorStyles));
		this.Dock = (DockStyle)info.GetValue("Dock", typeof(DockStyle));

		this.FontName = info.GetString("FontName");
		this.FontSize = info.GetSingle("FontSize");
		this.FontDecoration = (FontStyle)info.GetValue("FontDecoration", typeof(FontStyle));

		this.Tag = (byte[])info.GetValue("Tag", typeof(byte[]));

		this.Items = (ArrayList)info.GetValue("Items", typeof(ArrayList));
	}

	#endregion

}
