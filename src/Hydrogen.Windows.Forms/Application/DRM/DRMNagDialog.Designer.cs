// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms
{

	partial class DRMNagDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(DRMNagDialog));
			_closeButton = new System.Windows.Forms.Button();
			_enterKeyButton = new System.Windows.Forms.Button();
			_buyNowLink = new System.Windows.Forms.LinkLabel();
			_expirationControl = new ProductExpirationDetailsControl();
			_shoppingCartPictureBox = new System.Windows.Forms.PictureBox();
			applicationBanner1 = new ApplicationBanner();
			((System.ComponentModel.ISupportInitialize)_shoppingCartPictureBox).BeginInit();
			SuspendLayout();
			// 
			// _closeButton
			// 
			_closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			_closeButton.Location = new System.Drawing.Point(439, 230);
			_closeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_closeButton.Name = "_closeButton";
			_closeButton.Size = new System.Drawing.Size(133, 27);
			_closeButton.TabIndex = 6;
			_closeButton.Text = "&Close";
			_closeButton.UseVisualStyleBackColor = true;
			_closeButton.Click += _closeButton_Click;
			// 
			// _enterKeyButton
			// 
			_enterKeyButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			_enterKeyButton.Location = new System.Drawing.Point(299, 230);
			_enterKeyButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_enterKeyButton.Name = "_enterKeyButton";
			_enterKeyButton.Size = new System.Drawing.Size(133, 27);
			_enterKeyButton.TabIndex = 7;
			_enterKeyButton.Text = "Enter Product Key";
			_enterKeyButton.UseVisualStyleBackColor = true;
			_enterKeyButton.Click += _enterKeyButton_Click;
			// 
			// _buyNowLink
			// 
			_buyNowLink.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_buyNowLink.AutoSize = true;
			_buyNowLink.Location = new System.Drawing.Point(57, 235);
			_buyNowLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			_buyNowLink.Name = "_buyNowLink";
			_buyNowLink.Size = new System.Drawing.Size(205, 15);
			_buyNowLink.TabIndex = 8;
			_buyNowLink.TabStop = true;
			_buyNowLink.Text = "Click here to purchase the full version";
			_buyNowLink.LinkClicked += _buyNowLink_LinkClicked;
			// 
			// _expirationControl
			// 
			_expirationControl.EnableStateChangeEvent = false;
			_expirationControl.Location = new System.Drawing.Point(29, 140);
			_expirationControl.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			_expirationControl.Name = "_expirationControl";
			_expirationControl.Size = new System.Drawing.Size(542, 83);
			_expirationControl.TabIndex = 9;
			// 
			// _shoppingCartPictureBox
			// 
			_shoppingCartPictureBox.Image = (System.Drawing.Image)resources.GetObject("_shoppingCartPictureBox.Image");
			_shoppingCartPictureBox.Location = new System.Drawing.Point(33, 232);
			_shoppingCartPictureBox.Margin = new System.Windows.Forms.Padding(0);
			_shoppingCartPictureBox.Name = "_shoppingCartPictureBox";
			_shoppingCartPictureBox.Size = new System.Drawing.Size(20, 20);
			_shoppingCartPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			_shoppingCartPictureBox.TabIndex = 10;
			_shoppingCartPictureBox.TabStop = false;
			// 
			// applicationBanner1
			// 
			applicationBanner1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			applicationBanner1.CompanyName = "{CompanyName}";
			applicationBanner1.FromColor = System.Drawing.Color.RoyalBlue;
			applicationBanner1.Location = new System.Drawing.Point(0, 0);
			applicationBanner1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			applicationBanner1.MaximumSize = new System.Drawing.Size(9999, 111);
			applicationBanner1.Name = "applicationBanner1";
			applicationBanner1.Size = new System.Drawing.Size(586, 111);
			applicationBanner1.TabIndex = 11;
			applicationBanner1.Title = "{ProductName}";
			applicationBanner1.ToColor = System.Drawing.Color.LightBlue;
			applicationBanner1.Version = "Version {ProductVersion}";
			// 
			// DRMNagDialog
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(586, 270);
			Controls.Add(applicationBanner1);
			Controls.Add(_shoppingCartPictureBox);
			Controls.Add(_expirationControl);
			Controls.Add(_buyNowLink);
			Controls.Add(_enterKeyButton);
			Controls.Add(_closeButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "DRMNagDialog";
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Evaluation Version Notice";
			((System.ComponentModel.ISupportInitialize)_shoppingCartPictureBox).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion


		protected System.Windows.Forms.Button _closeButton;
		protected System.Windows.Forms.Button _enterKeyButton;
		protected System.Windows.Forms.LinkLabel _buyNowLink;
		protected ProductExpirationDetailsControl _expirationControl;
		protected System.Windows.Forms.PictureBox _shoppingCartPictureBox;
		private ApplicationBanner applicationBanner1;
	}
}
