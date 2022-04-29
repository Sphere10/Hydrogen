//-----------------------------------------------------------------------
// <copyright file="StandardNagDialog.Designer.cs" company="Sphere 10 Software">
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

    partial class StandardNagDialog {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StandardNagDialog));
			this._closeButton = new System.Windows.Forms.Button();
			this._enterKeyButton = new System.Windows.Forms.Button();
			this._buyNowLink = new System.Windows.Forms.LinkLabel();
			this.applicationBanner1 = new Sphere10.Framework.Windows.Forms.ApplicationBanner();
			this._nagMessageControl = new Sphere10.Framework.Windows.Forms.ProductExpirationDetailsControl();
			this._shoppingCartPictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this._shoppingCartPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// _closeButton
			// 
			this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._closeButton.Location = new System.Drawing.Point(376, 199);
			this._closeButton.Name = "_closeButton";
			this._closeButton.Size = new System.Drawing.Size(114, 23);
			this._closeButton.TabIndex = 6;
			this._closeButton.Text = "&Close";
			this._closeButton.UseVisualStyleBackColor = true;
			this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
			// 
			// _enterKeyButton
			// 
			this._enterKeyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._enterKeyButton.Location = new System.Drawing.Point(256, 199);
			this._enterKeyButton.Name = "_enterKeyButton";
			this._enterKeyButton.Size = new System.Drawing.Size(114, 23);
			this._enterKeyButton.TabIndex = 7;
			this._enterKeyButton.Text = "Enter Product Key";
			this._enterKeyButton.UseVisualStyleBackColor = true;
			this._enterKeyButton.Click += new System.EventHandler(this._enterKeyButton_Click);
			// 
			// _buyNowLink
			// 
			this._buyNowLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buyNowLink.AutoSize = true;
			this._buyNowLink.Location = new System.Drawing.Point(49, 204);
			this._buyNowLink.Name = "_buyNowLink";
			this._buyNowLink.Size = new System.Drawing.Size(184, 13);
			this._buyNowLink.TabIndex = 8;
			this._buyNowLink.TabStop = true;
			this._buyNowLink.Text = "Click here to purchase the full version";
			this._buyNowLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._buyNowLink_LinkClicked);
			// 
			// applicationBanner1
			// 
			this.applicationBanner1.EnableStateChangeEvent = false;
			this.applicationBanner1.Dock = System.Windows.Forms.DockStyle.Top;
			this.applicationBanner1.FromColor = System.Drawing.Color.RoyalBlue;
			this.applicationBanner1.Location = new System.Drawing.Point(0, 0);
			this.applicationBanner1.Name = "applicationBanner1";
			this.applicationBanner1.Size = new System.Drawing.Size(502, 96);
			this.applicationBanner1.TabIndex = 0;
			this.applicationBanner1.Title = "{ProductName}";
			this.applicationBanner1.ToColor = System.Drawing.Color.LightBlue;
			this.applicationBanner1.Version = "Version {ProductVersion}";
			// 
			// _nagMessageControl
			// 
			this._nagMessageControl.EnableStateChangeEvent = false;
			this._nagMessageControl.Location = new System.Drawing.Point(25, 121);
			this._nagMessageControl.Name = "_nagMessageControl";
			this._nagMessageControl.Size = new System.Drawing.Size(465, 72);
			this._nagMessageControl.TabIndex = 9;
			// 
			// _shoppingCartPictureBox
			// 
			this._shoppingCartPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("_shoppingCartPictureBox.Image")));
			this._shoppingCartPictureBox.Location = new System.Drawing.Point(25, 201);
			this._shoppingCartPictureBox.Margin = new System.Windows.Forms.Padding(0);
			this._shoppingCartPictureBox.Name = "_shoppingCartPictureBox";
			this._shoppingCartPictureBox.Size = new System.Drawing.Size(21, 21);
			this._shoppingCartPictureBox.TabIndex = 10;
			this._shoppingCartPictureBox.TabStop = false;
			// 
			// NagForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 234);
			this.Controls.Add(this._shoppingCartPictureBox);
			this.Controls.Add(this._nagMessageControl);
			this.Controls.Add(this._buyNowLink);
			this.Controls.Add(this._enterKeyButton);
			this.Controls.Add(this._closeButton);
			this.Controls.Add(this.applicationBanner1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NagForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Evaluation Version Notice";
			((System.ComponentModel.ISupportInitialize)(this._shoppingCartPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		protected ApplicationBanner applicationBanner1;
		protected System.Windows.Forms.Button _closeButton;
		protected System.Windows.Forms.Button _enterKeyButton;
		protected System.Windows.Forms.LinkLabel _buyNowLink;
		protected ProductExpirationDetailsControl _nagMessageControl;
		protected System.Windows.Forms.PictureBox _shoppingCartPictureBox;

	}
}
