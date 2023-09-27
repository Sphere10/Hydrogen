// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {

    partial class WhoAreYouControl {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this._homeUserRadioButton = new System.Windows.Forms.RadioButton();
            this._smallBusinessRadioButton = new System.Windows.Forms.RadioButton();
            this._mediumBusinessRadioButton = new System.Windows.Forms.RadioButton();
            this._corporationRadioButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // _homeUserRadioButton
            // 
            this._homeUserRadioButton.AutoSize = true;
            this._homeUserRadioButton.Checked = true;
            this._homeUserRadioButton.Location = new System.Drawing.Point(3, 3);
            this._homeUserRadioButton.Name = "_homeUserRadioButton";
            this._homeUserRadioButton.Size = new System.Drawing.Size(78, 17);
            this._homeUserRadioButton.TabIndex = 1;
            this._homeUserRadioButton.TabStop = true;
            this._homeUserRadioButton.Text = "Home User";
            this._homeUserRadioButton.UseVisualStyleBackColor = true;
            // 
            // _smallBusinessRadioButton
            // 
            this._smallBusinessRadioButton.AutoSize = true;
            this._smallBusinessRadioButton.Location = new System.Drawing.Point(3, 26);
            this._smallBusinessRadioButton.Name = "_smallBusinessRadioButton";
            this._smallBusinessRadioButton.Size = new System.Drawing.Size(178, 17);
            this._smallBusinessRadioButton.TabIndex = 2;
            this._smallBusinessRadioButton.Text = "Small Business (2 - 5 employees)";
            this._smallBusinessRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mediumBusinessRadioButton
            // 
            this._mediumBusinessRadioButton.AutoSize = true;
            this._mediumBusinessRadioButton.Location = new System.Drawing.Point(3, 49);
            this._mediumBusinessRadioButton.Name = "_mediumBusinessRadioButton";
            this._mediumBusinessRadioButton.Size = new System.Drawing.Size(196, 17);
            this._mediumBusinessRadioButton.TabIndex = 3;
            this._mediumBusinessRadioButton.Text = "Medium Business (6 - 20 employees)";
            this._mediumBusinessRadioButton.UseVisualStyleBackColor = true;
            // 
            // _corporationRadioButton
            // 
            this._corporationRadioButton.AutoSize = true;
            this._corporationRadioButton.Location = new System.Drawing.Point(3, 72);
            this._corporationRadioButton.Name = "_corporationRadioButton";
            this._corporationRadioButton.Size = new System.Drawing.Size(159, 17);
            this._corporationRadioButton.TabIndex = 4;
            this._corporationRadioButton.Text = "Large Business (21+ employees)";
            this._corporationRadioButton.UseVisualStyleBackColor = true;
            // 
            // WhoAreYouControl
            // 
            this.Controls.Add(this._corporationRadioButton);
            this.Controls.Add(this._mediumBusinessRadioButton);
            this.Controls.Add(this._smallBusinessRadioButton);
            this.Controls.Add(this._homeUserRadioButton);
            this.Name = "WhoAreYouControl";
            this.Size = new System.Drawing.Size(197, 92);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton _homeUserRadioButton;
        private System.Windows.Forms.RadioButton _smallBusinessRadioButton;
        private System.Windows.Forms.RadioButton _mediumBusinessRadioButton;
        private System.Windows.Forms.RadioButton _corporationRadioButton;
    }
}
