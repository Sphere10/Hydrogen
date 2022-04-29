//-----------------------------------------------------------------------
// <copyright file="ProductActivationForm.Designer.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Windows.Forms {

    partial class ProductActivationForm {
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
			this.applicationBanner1 = new Sphere10.Framework.Windows.Forms.ApplicationBanner();
			this._cancelButton = new System.Windows.Forms.Button();
			this._activateButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this._licenseKeyTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// applicationBanner1
			// 
			this.applicationBanner1.EnableStateChangeEvent = false;
			this.applicationBanner1.Text = "Product Activation";
			this.applicationBanner1.Dock = System.Windows.Forms.DockStyle.Top;
			this.applicationBanner1.Location = new System.Drawing.Point(0, 0);
			this.applicationBanner1.Name = "applicationBanner1";
			this.applicationBanner1.Size = new System.Drawing.Size(472, 96);
			this.applicationBanner1.TabIndex = 0;
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(385, 181);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "&Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _activateButton
			// 
			this._activateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._activateButton.Location = new System.Drawing.Point(304, 181);
			this._activateButton.Name = "_activateButton";
			this._activateButton.Size = new System.Drawing.Size(75, 23);
			this._activateButton.TabIndex = 2;
			this._activateButton.Text = "&Activate";
			this._activateButton.UseVisualStyleBackColor = true;
			this._activateButton.Click += new System.EventHandler(this._activateButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._licenseKeyTextBox);
			this.groupBox1.Location = new System.Drawing.Point(12, 102);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(448, 66);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Enter your product key";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Product Key";
			// 
			// _licenseKeyTextBox
			// 
			this._licenseKeyTextBox.Location = new System.Drawing.Point(96, 25);
			this._licenseKeyTextBox.Name = "_licenseKeyTextBox";
			this._licenseKeyTextBox.Size = new System.Drawing.Size(346, 20);
			this._licenseKeyTextBox.TabIndex = 0;
			// 
			// ProductActivationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(472, 216);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this._activateButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this.applicationBanner1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProductActivationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Product Activation";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private ApplicationBanner applicationBanner1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _activateButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox _licenseKeyTextBox;
        private System.Windows.Forms.Label label1;
    }
}
