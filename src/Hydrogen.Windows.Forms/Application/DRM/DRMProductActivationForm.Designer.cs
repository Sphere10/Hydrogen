// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {

    partial class DRMProductActivationForm {
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
			this._cancelButton = new System.Windows.Forms.Button();
			this._activateButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this._licenseKeyTextBox = new System.Windows.Forms.TextBox();
			this._applicationBanner = new Hydrogen.Windows.Forms.ApplicationBanner();
			this._loadingCircle = new Hydrogen.Windows.Forms.LoadingCircle();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(449, 209);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(88, 27);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "&Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _activateButton
			// 
			this._activateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._activateButton.Location = new System.Drawing.Point(355, 209);
			this._activateButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._activateButton.Name = "_activateButton";
			this._activateButton.Size = new System.Drawing.Size(88, 27);
			this._activateButton.TabIndex = 2;
			this._activateButton.Text = "&Activate";
			this._activateButton.UseVisualStyleBackColor = true;
			this._activateButton.Click += new System.EventHandler(this._activateButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._licenseKeyTextBox);
			this.groupBox1.Location = new System.Drawing.Point(14, 118);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox1.Size = new System.Drawing.Size(523, 76);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Enter your product key";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 32);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Product Key";
			// 
			// _licenseKeyTextBox
			// 
			this._licenseKeyTextBox.Location = new System.Drawing.Point(112, 29);
			this._licenseKeyTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._licenseKeyTextBox.Name = "_licenseKeyTextBox";
			this._licenseKeyTextBox.Size = new System.Drawing.Size(403, 23);
			this._licenseKeyTextBox.TabIndex = 0;
			// 
			// _applicationBanner
			// 
			this._applicationBanner.CompanyName = "{CompanyName}";
			this._applicationBanner.FromColor = System.Drawing.Color.RoyalBlue;
			this._applicationBanner.Location = new System.Drawing.Point(0, 0);
			this._applicationBanner.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._applicationBanner.Name = "_applicationBanner";
			this._applicationBanner.Size = new System.Drawing.Size(551, 111);
			this._applicationBanner.TabIndex = 5;
			this._applicationBanner.Title = "{ProductName}";
			this._applicationBanner.ToColor = System.Drawing.Color.LightBlue;
			this._applicationBanner.Version = "Version {ProductVersion}";
			// 
			// _loadingCircle
			// 
			this._loadingCircle.Active = false;
			this._loadingCircle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._loadingCircle.BackColor = System.Drawing.Color.Transparent;
			this._loadingCircle.Color = System.Drawing.Color.DarkGray;
			this._loadingCircle.HideStopControl = null;
			this._loadingCircle.InnerCircleRadius = 8;
			this._loadingCircle.Location = new System.Drawing.Point(305, 201);
			this._loadingCircle.Name = "_loadingCircle";
			this._loadingCircle.NumberSpoke = 10;
			this._loadingCircle.OuterCircleRadius = 10;
			this._loadingCircle.RotationSpeed = 100;
			this._loadingCircle.Size = new System.Drawing.Size(43, 46);
			this._loadingCircle.SpokeThickness = 4;
			this._loadingCircle.TabIndex = 4;
			this._loadingCircle.Text = "loadingCircle1";
			this._loadingCircle.Visible = false;
			// 
			// DRMProductActivationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(551, 249);
			this.ControlBox = false;
			this.Controls.Add(this._applicationBanner);
			this.Controls.Add(this._loadingCircle);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this._activateButton);
			this.Controls.Add(this._cancelButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DRMProductActivationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Product Activation";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _activateButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox _licenseKeyTextBox;
        private System.Windows.Forms.Label label1;
		private ApplicationBanner _applicationBanner;
		private LoadingCircle _loadingCircle;
	}
}
