//-----------------------------------------------------------------------
// <copyright file="FakeTransparentFormTester.Designer.cs" company="Sphere 10 Software">
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
	partial class FakeTransparentForm {
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
			this._toggleBorderButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _toggleBorderButton
			// 
			this._toggleBorderButton.Location = new System.Drawing.Point(208, 50);
			this._toggleBorderButton.Name = "_toggleBorderButton";
			this._toggleBorderButton.Size = new System.Drawing.Size(114, 23);
			this._toggleBorderButton.TabIndex = 0;
			this._toggleBorderButton.Text = "Toggle Border";
			this._toggleBorderButton.UseVisualStyleBackColor = true;
			this._toggleBorderButton.Click += new System.EventHandler(this._toggleBorderButton_Click);
			// 
			// FakeTransparentFormTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Fuchsia;
			this.ClientSize = new System.Drawing.Size(393, 205);
			this.Controls.Add(this._toggleBorderButton);
			this.Name = "FakeTransparentForm";
			this.Text = "FakeTransparentFormTester";
			this.TransparencyKey = System.Drawing.Color.Fuchsia;
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _toggleBorderButton;
	}
}
