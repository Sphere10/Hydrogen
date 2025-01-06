// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms;
partial class ProductAboutBox {
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
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
		okButton = new System.Windows.Forms.Button();
		applicationBanner1 = new ApplicationBanner();
		_label5 = new System.Windows.Forms.Label();
		_label6 = new System.Windows.Forms.Label();
		_label7 = new System.Windows.Forms.Label();
		_label8 = new System.Windows.Forms.Label();
		_label9 = new System.Windows.Forms.Label();
		_label10 = new System.Windows.Forms.Label();
		_label11 = new System.Windows.Forms.Label();
		_label12 = new System.Windows.Forms.Label();
		_label1 = new System.Windows.Forms.Label();
		_label2 = new System.Windows.Forms.Label();
		_label3 = new System.Windows.Forms.Label();
		_label4 = new System.Windows.Forms.Label();
		_link1 = new System.Windows.Forms.LinkLabel();
		_companyNumberLabel = new System.Windows.Forms.Label();
		SuspendLayout();
		// 
		// okButton
		// 
		okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		okButton.Location = new System.Drawing.Point(412, 293);
		okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
		okButton.Name = "okButton";
		okButton.Size = new System.Drawing.Size(134, 27);
		okButton.TabIndex = 31;
		okButton.Text = "&OK";
		// 
		// applicationBanner1
		// 
		applicationBanner1.CompanyName = "{CompanyName}";
		applicationBanner1.Dock = System.Windows.Forms.DockStyle.Top;
		applicationBanner1.EnableStateChangeEvent = false;
		applicationBanner1.FromColor = System.Drawing.Color.RoyalBlue;
		applicationBanner1.Location = new System.Drawing.Point(0, 0);
		applicationBanner1.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
		applicationBanner1.MaximumSize = new System.Drawing.Size(9999, 111);
		applicationBanner1.Name = "applicationBanner1";
		applicationBanner1.Size = new System.Drawing.Size(559, 111);
		applicationBanner1.TabIndex = 32;
		applicationBanner1.Title = "{ProductName}";
		applicationBanner1.ToColor = System.Drawing.Color.LightBlue;
		applicationBanner1.Version = "Version {ProductVersion}";
		// 
		// _label5
		// 
		_label5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		_label5.AutoSize = true;
		_label5.Location = new System.Drawing.Point(408, 132);
		_label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label5.Name = "_label5";
		_label5.Size = new System.Drawing.Size(60, 15);
		_label5.TabIndex = 38;
		_label5.Text = "First used:";
		// 
		// _label6
		// 
		_label6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		_label6.AutoSize = true;
		_label6.Location = new System.Drawing.Point(404, 147);
		_label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label6.Name = "_label6";
		_label6.Size = new System.Drawing.Size(61, 15);
		_label6.TabIndex = 39;
		_label6.Text = "Total uses:";
		// 
		// _label7
		// 
		_label7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		_label7.AutoSize = true;
		_label7.Location = new System.Drawing.Point(323, 162);
		_label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label7.Name = "_label7";
		_label7.Size = new System.Drawing.Size(142, 15);
		_label7.TabIndex = 40;
		_label7.Text = "First used by current user:";
		// 
		// _label8
		// 
		_label8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		_label8.AutoSize = true;
		_label8.Location = new System.Drawing.Point(318, 177);
		_label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label8.Name = "_label8";
		_label8.Size = new System.Drawing.Size(143, 15);
		_label8.TabIndex = 41;
		_label8.Text = "Total uses by current user:";
		// 
		// _label9
		// 
		_label9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		_label9.AutoSize = true;
		_label9.Location = new System.Drawing.Point(479, 132);
		_label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label9.Name = "_label9";
		_label9.Size = new System.Drawing.Size(159, 15);
		_label9.TabIndex = 42;
		_label9.Text = "{FirstUsedDateBySystemUTC}";
		// 
		// _label10
		// 
		_label10.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		_label10.AutoSize = true;
		_label10.Location = new System.Drawing.Point(479, 147);
		_label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label10.Name = "_label10";
		_label10.Size = new System.Drawing.Size(147, 15);
		_label10.TabIndex = 43;
		_label10.Text = "{NumberOfUsesBySystem}";
		// 
		// _label11
		// 
		_label11.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		_label11.AutoSize = true;
		_label11.Location = new System.Drawing.Point(479, 162);
		_label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label11.Name = "_label11";
		_label11.Size = new System.Drawing.Size(144, 15);
		_label11.TabIndex = 44;
		_label11.Text = "{FirstUsedDateByUserUTC}";
		// 
		// _label12
		// 
		_label12.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		_label12.AutoSize = true;
		_label12.Location = new System.Drawing.Point(479, 177);
		_label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label12.Name = "_label12";
		_label12.Size = new System.Drawing.Size(132, 15);
		_label12.TabIndex = 45;
		_label12.Text = "{NumberOfUsesByUser}";
		// 
		// _label1
		// 
		_label1.AutoSize = true;
		_label1.Location = new System.Drawing.Point(14, 132);
		_label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label1.Name = "_label1";
		_label1.Size = new System.Drawing.Size(99, 15);
		_label1.TabIndex = 33;
		_label1.Text = "{CompanyName}";
		// 
		// _label2
		// 
		_label2.AutoSize = true;
		_label2.Location = new System.Drawing.Point(14, 162);
		_label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label2.Name = "_label2";
		_label2.Size = new System.Drawing.Size(89, 15);
		_label2.TabIndex = 34;
		_label2.Text = "{ProductName}";
		// 
		// _label3
		// 
		_label3.AutoSize = true;
		_label3.Location = new System.Drawing.Point(14, 177);
		_label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label3.Name = "_label3";
		_label3.Size = new System.Drawing.Size(163, 15);
		_label3.TabIndex = 35;
		_label3.Text = "Version {ProductLongVersion}";
		// 
		// _label4
		// 
		_label4.AutoSize = true;
		_label4.Location = new System.Drawing.Point(14, 192);
		_label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_label4.Name = "_label4";
		_label4.Size = new System.Drawing.Size(103, 15);
		_label4.TabIndex = 36;
		_label4.Text = "{CopyrightNotice}";
		// 
		// _link1
		// 
		_link1.AutoSize = true;
		_link1.Location = new System.Drawing.Point(14, 207);
		_link1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_link1.Name = "_link1";
		_link1.Size = new System.Drawing.Size(82, 15);
		_link1.TabIndex = 37;
		_link1.TabStop = true;
		_link1.Text = "{CompanyUrl}";
		_link1.LinkClicked += _productLink_LinkClicked;
		// 
		// _companyNumberLabel
		// 
		_companyNumberLabel.AutoSize = true;
		_companyNumberLabel.Location = new System.Drawing.Point(14, 147);
		_companyNumberLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		_companyNumberLabel.Name = "_companyNumberLabel";
		_companyNumberLabel.Size = new System.Drawing.Size(111, 15);
		_companyNumberLabel.TabIndex = 48;
		_companyNumberLabel.Text = "{CompanyNumber}";
		// 
		// ProductAboutBox
		// 
		AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		ClientSize = new System.Drawing.Size(559, 333);
		Controls.Add(_companyNumberLabel);
		Controls.Add(_label12);
		Controls.Add(_label11);
		Controls.Add(_label10);
		Controls.Add(_label9);
		Controls.Add(_label8);
		Controls.Add(_label7);
		Controls.Add(_label6);
		Controls.Add(_label5);
		Controls.Add(_link1);
		Controls.Add(_label4);
		Controls.Add(_label3);
		Controls.Add(_label2);
		Controls.Add(_label1);
		Controls.Add(applicationBanner1);
		Controls.Add(okButton);
		FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "ProductAboutBox";
		ShowIcon = false;
		ShowInTaskbar = false;
		StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		Text = "About {ProductName}";
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	protected System.Windows.Forms.Button okButton;
	protected ApplicationBanner applicationBanner1;
	protected System.Windows.Forms.Label _label5;
	protected System.Windows.Forms.Label _label6;
	protected System.Windows.Forms.Label _label7;
	protected System.Windows.Forms.Label _label8;
	protected System.Windows.Forms.Label _label9;
	protected System.Windows.Forms.Label _label10;
	protected System.Windows.Forms.Label _label11;
	protected System.Windows.Forms.Label _label12;
	protected System.Windows.Forms.Label _label1;
	protected System.Windows.Forms.Label _label2;
	protected System.Windows.Forms.Label _label3;
	protected System.Windows.Forms.Label _label4;
	protected System.Windows.Forms.LinkLabel _link1;
	protected System.Windows.Forms.Label _companyNumberLabel;
}