//-----------------------------------------------------------------------
// <copyright file="BasicContactDetailsControl.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sphere10.Framework.Windows.Forms
{
    public partial class BasicContactDetailsControl : UserControl {
        public BasicContactDetailsControl() {
            InitializeComponent();
        }

        private void EnableDisableControls() {
            _emailTextBox.Enabled = _emailButton.Checked;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContactIsAnonymous {
            get {
                return _anonymousButton.Checked;
            }
            set {
                _anonymousButton.Checked = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ContactEmail {
            get {
                if (ContactIsAnonymous) {
                    return "Anonymous";
                } else {
                    return _emailTextBox.Text;
                }
            }
            set {
                _emailTextBox.Text = value;
            }
        }

        private void _emailButton_CheckedChanged(object sender, EventArgs e) {
            EnableDisableControls();
        }



    }
}
