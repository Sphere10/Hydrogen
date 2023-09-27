// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// This component allows you to modify a base forms ToolStrip. 
/// Microsoft has intentionally added this limitation due to the complexities it could not overcome. This component simply works around
/// those limitations by allowing you to merge a (hidden) tool strip on the sub form with the readonly inherited base form.
/// </summary>
public class MergeableMenuStrip : MenuStrip {
	private ToolStripVisualInheritanceFixer _fixer;

	public MergeableMenuStrip() {
		_fixer = new ToolStripVisualInheritanceFixer();
		_fixer.ToolStrip = this;
	}

	protected override void InitLayout() {
		base.InitLayout();
		Visible = false;
	}

	/// <summary>
	/// The base form's menu strip.
	/// </summary>
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Description("Set this to the base forms MenuStrip. If you can't find it, make sure it's modifer is set to to 'protected' or greater.")]
	[Category("Behavior")]
	public MenuStrip InheritedToolStrip {
		get { return _fixer.InheritedToolStrip as MenuStrip; }
		set { _fixer.InheritedToolStrip = (value as MenuStrip); }
	}

}
