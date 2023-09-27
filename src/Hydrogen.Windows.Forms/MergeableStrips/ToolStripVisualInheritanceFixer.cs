// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// This component allows you to modify a base forms ToolStrip by merging a (hidden) tool strip on the sub-form. 
/// It is a work-around for Microsoft's intentionally added limitation (as they could not figure out this simple work-around).
/// </summary>
public class ToolStripVisualInheritanceFixer : Component {
	private ToolStrip _toolStrip;
	private ToolStrip _inheritedToolStrip;
	private EventHandler _visibilityChangedHandler;
	private LayoutEventHandler _layoutHandler;
	private const string ClonedTag = "__$$Cloned";

	public ToolStripVisualInheritanceFixer()
		: this(null) {
	}

	ToolStripVisualInheritanceFixer(IContainer container) {
		if (container != null) {
			container.Add(this);
		}
		_toolStrip = null;
		_inheritedToolStrip = null;
		_visibilityChangedHandler = new EventHandler(_toolStrip_VisibleChanged);
		_layoutHandler = new LayoutEventHandler(_toolStrip_Layout);
		InitializeComponent();
	}

	/// <summary>
	/// The base form's tool strip. Set base forms ToolStrip's modifer to 'protected' if you cannot see find it here
	/// </summary>
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Description("The base form's tool strip. If you can't find it, make sure the base forms ToolStrip has it's modifer to 'protected' (or greater)")]
	[Category("Behavior")]
	public ToolStrip InheritedToolStrip {
		get { return _inheritedToolStrip; }
		set { _inheritedToolStrip = value; }
	}

	/// <summary>
	/// The tool strip on this form to be merged with the inherited tool strip.
	/// </summary>
	[Description("The tool strip on this form to be merged with the inherited tool strip")]
	[Category("Behavior")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public ToolStrip ToolStrip {
		get { return _toolStrip; }
		set {
			if (_toolStrip != null) {
				_toolStrip.VisibleChanged -= _visibilityChangedHandler;
				_toolStrip.Layout -= _layoutHandler;
				_toolStrip.Visible = true;
			}
			_toolStrip = value;
			if (_toolStrip != null) {
				_toolStrip.VisibleChanged += _visibilityChangedHandler;
				_toolStrip.Layout += _layoutHandler;
				_toolStrip.Visible = false;
			}
		}
	}

	private bool CanMerge {
		get { return ToolStrip != null && InheritedToolStrip != null; }
	}

	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// At runtime this merges ToolStrip with InheritedToolStrip. At design-time, only a clone of ToolStrip is merged (to prefect designer serialization problems)
	/// </summary>
	private void MergeWithInheritedToolStrip() {
		ToolStrip sourceMenuStrip;
		if (!IsDesignMode()) {
			sourceMenuStrip = ToolStrip;
		} else {
			List<ToolStripItem> lastMergedItems = new List<ToolStripItem>();
			foreach (ToolStripItem item in InheritedToolStrip.Items) {
				if (item.Tag is string && (string)item.Tag == ClonedTag) {
					lastMergedItems.Add(item);
				}
			}
			foreach (ToolStripItem item in lastMergedItems) {
				InheritedToolStrip.Items.Remove(item);
			}
			sourceMenuStrip = BasicClone(ToolStrip);
		}

		// .NET already provided the functionality to merge tool strips, so we just use that!
		ToolStripManager.Merge(sourceMenuStrip, InheritedToolStrip);
	}

	/// <summary>
	/// This just clones a toolstrip and it's items. It is written for our purposes here. For general 
	/// use setting of its state should be done similar to the below method.
	/// </summary>
	/// <param name="original">ToolStrip be be cloned</param>
	/// <returns>The cloned ToolStrip</returns>
	private static ToolStrip BasicClone(ToolStrip original) {
		ToolStrip clone = new ToolStrip();
		foreach (ToolStripItem item in original.Items) {
			ToolStripItem clonedItem = Clone(item);
			if (clonedItem != null) {
				clone.Items.Add(clonedItem);
			}
		}
		return clone;
	}

	/// <summary>
	/// Clones a ToolStripItem using reflection.
	/// </summary>
	/// <param name="original">The original item to be cloned.</param>
	/// <returns>Cloned tool strip item (or null if original has no default constructor)</returns>
	private static ToolStripItem Clone(ToolStripItem original) {
		ToolStripItem clone = null;
		if (original is ICloneable) {
			clone = (ToolStripItem)((ICloneable)original).Clone();
		} else {
			if (original.GetType().GetConstructor(new Type[0]) != null) {
				clone = (ToolStripItem)Activator.CreateInstance(original.GetType());
				FieldInfo[] fis = original.GetType().GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fi in fis) {
					object fieldValue = fi.GetValue(original);
					if (fi.FieldType.Namespace != original.GetType().Namespace)
						fi.SetValue(clone, fieldValue);
				}
				PropertyInfo[] pis = original.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (PropertyInfo pi in pis) {
					if (pi.Name == "Owner" || pi.Name == "OwnerItem") {
						// We ignore these properties as they cause problems
						continue;
					}
					if (pi.CanWrite) {
						pi.SetValue(clone, pi.GetValue(original, null), null);
					}
				}
				clone.Tag = ClonedTag;
				clone.Visible = true;
			}
		}
		return clone;
	}

	/// <summary> 
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing) {
		if (disposing && (components != null)) {
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent() {
		components = new System.ComponentModel.Container();
	}


	private bool IsDesignMode() {
		// .NET framework doesn't provide proper support for design mode checking, so we do it here manually
		return System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv";
	}

	#region Handlers

	void _toolStrip_Layout(object sender, LayoutEventArgs e) {
		if (!IsDesignMode() && CanMerge) {
			MergeWithInheritedToolStrip();
		}
	}

	void _toolStrip_VisibleChanged(object sender, EventArgs e) {
		if (InheritedToolStrip != null && IsDesignMode() && !ToolStrip.Visible && CanMerge) {
			MergeWithInheritedToolStrip();
		}
	}

	#endregion

}
