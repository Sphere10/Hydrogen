// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class GenericEditorForm : Form {
	public GenericEditorForm() : this(null, false) {

	}

	public GenericEditorForm(object entity, bool readOnly) {
		InitializeComponent();
		if (entity != null) {
			_propertyGrid.SelectedObject = entity;
		}
		_propertyGrid.Enabled = !readOnly;
	}

	public static void ShowForm(object entity, bool readOnly) {
		Form form = new GenericEditorForm(entity, readOnly);
		form.ShowDialog();
	}

	private void _closeButton_Click(object sender, EventArgs e) {
		Close();
	}
}
