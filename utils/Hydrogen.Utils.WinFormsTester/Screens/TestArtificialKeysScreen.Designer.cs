//-----------------------------------------------------------------------
// <copyright file="TestArtificialKeysForm.Designer.cs" company="Sphere 10 Software">
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
	partial class TestArtificialKeysScreen {
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this._testButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(12, 49);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(869, 294);
			this.textBox1.TabIndex = 0;
			// 
			// _testButton
			// 
			this._testButton.Location = new System.Drawing.Point(12, 20);
			this._testButton.Name = "_testButton";
			this._testButton.Size = new System.Drawing.Size(136, 23);
			this._testButton.TabIndex = 1;
			this._testButton.Text = "Test";
			this._testButton.UseVisualStyleBackColor = true;
			this._testButton.Click += new System.EventHandler(this._testButton_Click);
			// 
			// TestArtificialKeysForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(893, 355);
			this.Controls.Add(this._testButton);
			this.Controls.Add(this.textBox1);
			this.Name = "TestArtificialKeysScreen";
			this.Text = "TestArtificialKeysForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button _testButton;
	}
}
