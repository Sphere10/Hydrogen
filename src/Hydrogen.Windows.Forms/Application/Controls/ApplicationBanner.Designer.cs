// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Windows.Forms;

namespace Hydrogen.Windows.Forms
{
	partial class ApplicationBanner
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			_gradientPanel = new GradientPanel();
			_versionLabel = new System.Windows.Forms.Label();
			_iconPanel = new System.Windows.Forms.Panel();
			_companyNameLabel = new System.Windows.Forms.Label();
			_productNameLabel = new System.Windows.Forms.Label();
			_gradientPanel.SuspendLayout();
			SuspendLayout();
			// 
			// _gradientPanel
			// 
			_gradientPanel.Angle = 0;
			_gradientPanel.Blend = null;
			_gradientPanel.Controls.Add(_versionLabel);
			_gradientPanel.Controls.Add(_iconPanel);
			_gradientPanel.Controls.Add(_companyNameLabel);
			_gradientPanel.Controls.Add(_productNameLabel);
			_gradientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			_gradientPanel.FromColor = System.Drawing.Color.RoyalBlue;
			_gradientPanel.Location = new System.Drawing.Point(0, 0);
			_gradientPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_gradientPanel.Name = "_gradientPanel";
			_gradientPanel.Size = new System.Drawing.Size(700, 111);
			_gradientPanel.TabIndex = 6;
			_gradientPanel.ToColor = System.Drawing.Color.LightBlue;
			// 
			// _versionLabel
			// 
			_versionLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_versionLabel.AutoSize = true;
			_versionLabel.BackColor = System.Drawing.Color.Transparent;
			_versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			_versionLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
			_versionLabel.Location = new System.Drawing.Point(142, 74);
			_versionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			_versionLabel.Name = "_versionLabel";
			_versionLabel.Size = new System.Drawing.Size(158, 16);
			_versionLabel.TabIndex = 34;
			_versionLabel.Text = "Version {ProductVersion}";
			// 
			// _iconPanel
			// 
			_iconPanel.AutoSize = true;
			_iconPanel.BackColor = System.Drawing.Color.Transparent;
			_iconPanel.Location = new System.Drawing.Point(21, 6);
			_iconPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_iconPanel.MaximumSize = new System.Drawing.Size(100, 100);
			_iconPanel.Name = "_iconPanel";
			_iconPanel.Size = new System.Drawing.Size(100, 100);
			_iconPanel.TabIndex = 33;
			_iconPanel.SizeChanged += _iconPanel_SizeChanged;
			// 
			// _companyNameLabel
			// 
			_companyNameLabel.AutoSize = true;
			_companyNameLabel.BackColor = System.Drawing.Color.Transparent;
			_companyNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			_companyNameLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
			_companyNameLabel.Location = new System.Drawing.Point(144, 16);
			_companyNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			_companyNameLabel.Name = "_companyNameLabel";
			_companyNameLabel.Size = new System.Drawing.Size(76, 12);
			_companyNameLabel.TabIndex = 1;
			_companyNameLabel.Text = "{CompanyName}";
			// 
			// _productNameLabel
			// 
			_productNameLabel.AutoSize = true;
			_productNameLabel.BackColor = System.Drawing.Color.Transparent;
			_productNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			_productNameLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
			_productNameLabel.Location = new System.Drawing.Point(138, 30);
			_productNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			_productNameLabel.Name = "_productNameLabel";
			_productNameLabel.Size = new System.Drawing.Size(236, 37);
			_productNameLabel.TabIndex = 0;
			_productNameLabel.Text = "{ProductName}";
			// 
			// ApplicationBanner
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(_gradientPanel);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			MaximumSize = new System.Drawing.Size(9999, 111);
			Name = "ApplicationBanner";
			Size = new System.Drawing.Size(700, 111);
			_gradientPanel.ResumeLayout(false);
			_gradientPanel.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		protected System.Windows.Forms.Label _companyNameLabel;
		protected System.Windows.Forms.Label _productNameLabel;
		protected System.Windows.Forms.Label _versionLabel;
		protected System.Windows.Forms.Panel _iconPanel;
		protected GradientPanel _gradientPanel;
	}
}
