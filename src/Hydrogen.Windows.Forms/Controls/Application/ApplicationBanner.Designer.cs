//-----------------------------------------------------------------------
// <copyright file="ApplicationBanner.Designer.cs" company="Sphere 10 Software">
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

using Hydrogen.Windows.Forms;

namespace Hydrogen.Windows.Forms {
    partial class ApplicationBanner {
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
            this._gradientPanel = new Hydrogen.Windows.Forms.GradientPanel();
            this._versionLabel = new System.Windows.Forms.Label();
            this._iconPanel = new System.Windows.Forms.Panel();
            this._companyNameLabel = new System.Windows.Forms.Label();
            this._productNameLabel = new System.Windows.Forms.Label();
            this._gradientPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _gradientPanel
            // 
            this._gradientPanel.Angle = 0;
            this._gradientPanel.Blend = null;
            this._gradientPanel.Controls.Add(this._versionLabel);
            this._gradientPanel.Controls.Add(this._iconPanel);
            this._gradientPanel.Controls.Add(this._companyNameLabel);
            this._gradientPanel.Controls.Add(this._productNameLabel);
            this._gradientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gradientPanel.FromColor = System.Drawing.Color.RoyalBlue;
            this._gradientPanel.Location = new System.Drawing.Point(0, 0);
            this._gradientPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._gradientPanel.Name = "_gradientPanel";
            this._gradientPanel.Size = new System.Drawing.Size(700, 111);
            this._gradientPanel.TabIndex = 6;
            this._gradientPanel.ToColor = System.Drawing.Color.LightBlue;
            // 
            // _versionLabel
            // 
            this._versionLabel.AutoSize = true;
            this._versionLabel.BackColor = System.Drawing.Color.Transparent;
            this._versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._versionLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._versionLabel.Location = new System.Drawing.Point(142, 74);
            this._versionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._versionLabel.Name = "_versionLabel";
            this._versionLabel.Size = new System.Drawing.Size(158, 16);
            this._versionLabel.TabIndex = 34;
            this._versionLabel.Text = "Version {ProductVersion}";
            // 
            // _iconPanel
            // 
            this._iconPanel.BackColor = System.Drawing.Color.Transparent;
            this._iconPanel.Location = new System.Drawing.Point(21, 6);
            this._iconPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._iconPanel.Name = "_iconPanel";
            this._iconPanel.Size = new System.Drawing.Size(100, 100);
            this._iconPanel.TabIndex = 33;
            this._iconPanel.SizeChanged += new System.EventHandler(this._iconPanel_SizeChanged);
            // 
            // _companyNameLabel
            // 
            this._companyNameLabel.AutoSize = true;
            this._companyNameLabel.BackColor = System.Drawing.Color.Transparent;
            this._companyNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._companyNameLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._companyNameLabel.Location = new System.Drawing.Point(144, 16);
            this._companyNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._companyNameLabel.Name = "_companyNameLabel";
            this._companyNameLabel.Size = new System.Drawing.Size(76, 12);
            this._companyNameLabel.TabIndex = 1;
            this._companyNameLabel.Text = "{CompanyName}";
            // 
            // _productNameLabel
            // 
            this._productNameLabel.AutoSize = true;
            this._productNameLabel.BackColor = System.Drawing.Color.Transparent;
            this._productNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._productNameLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._productNameLabel.Location = new System.Drawing.Point(138, 30);
            this._productNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._productNameLabel.Name = "_productNameLabel";
            this._productNameLabel.Size = new System.Drawing.Size(236, 37);
            this._productNameLabel.TabIndex = 0;
            this._productNameLabel.Text = "{ProductName}";
            // 
            // ApplicationBanner
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._gradientPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ApplicationBanner";
            this.Size = new System.Drawing.Size(700, 111);
            this._gradientPanel.ResumeLayout(false);
            this._gradientPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		protected System.Windows.Forms.Label _companyNameLabel;
		protected System.Windows.Forms.Label _productNameLabel;
		protected System.Windows.Forms.Label _versionLabel;
		protected System.Windows.Forms.Panel _iconPanel;
		protected GradientPanel _gradientPanel;
    }
}
