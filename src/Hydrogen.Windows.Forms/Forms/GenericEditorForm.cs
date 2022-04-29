//-----------------------------------------------------------------------
// <copyright file="GenericEditorForm.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sphere10.Framework.Windows.Forms {
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
}
