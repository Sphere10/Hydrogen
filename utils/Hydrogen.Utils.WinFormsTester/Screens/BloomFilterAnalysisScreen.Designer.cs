//-----------------------------------------------------------------------
// <copyright file="TextAreaTests.Designer.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Hydrogen.Utils.WinFormsTester {
    partial class BloomFilterAnalysisScreen {
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
			this._standardTextBox = new System.Windows.Forms.TextBox();
			this._metricsTableButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _standardTextBox
			// 
			this._standardTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._standardTextBox.Location = new System.Drawing.Point(12, 41);
			this._standardTextBox.Multiline = true;
			this._standardTextBox.Name = "_standardTextBox";
			this._standardTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._standardTextBox.Size = new System.Drawing.Size(609, 224);
			this._standardTextBox.TabIndex = 0;
			// 
			// _metricsTableButton
			// 
			this._metricsTableButton.Location = new System.Drawing.Point(12, 12);
			this._metricsTableButton.Name = "_metricsTableButton";
			this._metricsTableButton.Size = new System.Drawing.Size(110, 23);
			this._metricsTableButton.TabIndex = 2;
			this._metricsTableButton.Text = "Metrics Table";
			this._metricsTableButton.UseVisualStyleBackColor = true;
			this._metricsTableButton.Click += new System.EventHandler(this._metricsTableButton_Click);
			// 
			// BloomFilterAnalysisScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._metricsTableButton);
			this.Controls.Add(this._standardTextBox);
			this.Name = "BloomFilterAnalysisScreen";
			this.Size = new System.Drawing.Size(633, 277);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.TextBox _standardTextBox;
        private System.Windows.Forms.Button _metricsTableButton;
	}
}
