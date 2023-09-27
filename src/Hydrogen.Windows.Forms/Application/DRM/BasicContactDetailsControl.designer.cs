// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {

    partial class BasicContactDetailsControl {
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
            this._emailButton = new System.Windows.Forms.RadioButton();
            this._anonymousButton = new System.Windows.Forms.RadioButton();
            this._emailTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _emailButton
            // 
            this._emailButton.AutoSize = true;
            this._emailButton.Location = new System.Drawing.Point(0, 24);
            this._emailButton.Name = "_emailButton";
            this._emailButton.Size = new System.Drawing.Size(50, 17);
            this._emailButton.TabIndex = 1;
            this._emailButton.Text = "Email";
            this._emailButton.UseVisualStyleBackColor = true;
            this._emailButton.CheckedChanged += new System.EventHandler(this._emailButton_CheckedChanged);
            // 
            // _anonymousButton
            // 
            this._anonymousButton.AutoSize = true;
            this._anonymousButton.Checked = true;
            this._anonymousButton.Location = new System.Drawing.Point(0, 0);
            this._anonymousButton.Name = "_anonymousButton";
            this._anonymousButton.Size = new System.Drawing.Size(80, 17);
            this._anonymousButton.TabIndex = 0;
            this._anonymousButton.TabStop = true;
            this._anonymousButton.Text = "Anonymous";
            this._anonymousButton.UseVisualStyleBackColor = true;
            // 
            // _emailTextBox
            // 
            this._emailTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._emailTextBox.Enabled = false;
            this._emailTextBox.Location = new System.Drawing.Point(56, 21);
            this._emailTextBox.Name = "_emailTextBox";
            this._emailTextBox.Size = new System.Drawing.Size(386, 20);
            this._emailTextBox.TabIndex = 2;
            // 
            // BasicContactDetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._emailButton);
            this.Controls.Add(this._anonymousButton);
            this.Controls.Add(this._emailTextBox);
            this.Name = "BasicContactDetailsControl";
            this.Size = new System.Drawing.Size(442, 48);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton _emailButton;
        private System.Windows.Forms.RadioButton _anonymousButton;
        private System.Windows.Forms.TextBox _emailTextBox;
    }
}
