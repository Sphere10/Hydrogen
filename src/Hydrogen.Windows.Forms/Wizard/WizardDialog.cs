//-----------------------------------------------------------------------
// <copyright file="WizardDialog.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevAge.Windows.Forms;
using Hydrogen;

namespace Hydrogen.Windows.Forms {
    public partial class WizardDialog<T> : FormEx {
        
        public WizardDialog() {
	        this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();
            Closing = false;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IWizard<T> WizardManager { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal new bool Closing { get; private set; }

        public async Task SetContent(WizardScreen<T> screen) {
            if (_contentPanel.Controls.Count > 0) {
                _contentPanel.RemoveAllControls();                
            }
            screen.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(screen);
        }

        public new void Close() {
            throw new MethodAccessException("Call CloseDialog");
        }

        public void CloseDialog() {
            Closing = true;
            base.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            if (!Closing) {
                var closeValidation = WizardManager.CancelRequested();
                e.Cancel = closeValidation.Failure;
                if (e.Cancel) {
	                DialogEx.Show(this, SystemIconType.Error, closeValidation.ErrorMessages.ToParagraphCase(true), "Error");
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
        }


        private async void _previousButton_Click(object sender, EventArgs e) {
            try {
                using (loadingCircle1.BeginAnimationScope(this)) {
                    await WizardManager.Previous();
                }
            } catch (Exception error) {
                ExceptionDialog.Show(this, error);
            }

        }

        private async void _nextButton_Click(object sender, EventArgs e) {
            try {
                using (loadingCircle1.BeginAnimationScope(this)) {
                    await WizardManager.Next();
                }
            } catch (Exception error) {
                ExceptionDialog.Show(this, error);
            }

        }
    }
}
