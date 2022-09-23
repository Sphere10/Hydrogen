//-----------------------------------------------------------------------
// <copyright file="NagMessageControl.Designer.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.Forms {

    partial class ProductExpirationDetailsControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this._expirationNoticeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _expirationNoticeLabel
            // 
            this._expirationNoticeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._expirationNoticeLabel.Location = new System.Drawing.Point(0, 0);
            this._expirationNoticeLabel.Name = "_expirationNoticeLabel";
            this._expirationNoticeLabel.Size = new System.Drawing.Size(237, 13);
            this._expirationNoticeLabel.TabIndex = 6;
            this._expirationNoticeLabel.Text = "Your software will expire on ...";
            // 
            // ProductExpirationDetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._expirationNoticeLabel);
            this.Name = "ProductExpirationDetailsControl";
            this.Size = new System.Drawing.Size(237, 13);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _expirationNoticeLabel;
    }
}
