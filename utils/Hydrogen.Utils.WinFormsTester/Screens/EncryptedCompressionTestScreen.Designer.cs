//-----------------------------------------------------------------------
// <copyright file="EncryptedCompressionTestForm.Designer.cs" company="Sphere 10 Software">
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
	partial class EncryptedCompressionTestScreen {
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
			this._testButton = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.fileSelectorControl1 = new Hydrogen.Windows.Forms.PathSelectorControl();
			this._passwordTextBox = new Hydrogen.Windows.Forms.TextBoxEx();
			this.SuspendLayout();
			// 
			// _testButton
			// 
			this._testButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._testButton.Location = new System.Drawing.Point(410, 12);
			this._testButton.Name = "_testButton";
			this._testButton.Size = new System.Drawing.Size(110, 23);
			this._testButton.TabIndex = 1;
			this._testButton.Text = "&Test";
			this._testButton.UseVisualStyleBackColor = true;
			this._testButton.Click += new System.EventHandler(this._testButton_Click);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(12, 68);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(508, 420);
			this.textBox1.TabIndex = 2;
			// 
			// fileSelectorControl1
			// 
			this.fileSelectorControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fileSelectorControl1.Location = new System.Drawing.Point(12, 12);
			this.fileSelectorControl1.Mode = Hydrogen.Windows.Forms.PathSelectionMode.OpenFile;
			this.fileSelectorControl1.Name = "fileSelectorControl1";
			this.fileSelectorControl1.Size = new System.Drawing.Size(392, 20);
			this.fileSelectorControl1.TabIndex = 0;
			// 
			// _passwordTextBox
			// 
			this._passwordTextBox.Location = new System.Drawing.Point(12, 42);
			this._passwordTextBox.Name = "_passwordTextBox";
			this._passwordTextBox.PlaceHolderText = "Password";
			this._passwordTextBox.Size = new System.Drawing.Size(352, 20);
			this._passwordTextBox.TabIndex = 3;
			// 
			// EncryptedCompressionTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(532, 500);
			this.Controls.Add(this._passwordTextBox);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this._testButton);
			this.Controls.Add(this.fileSelectorControl1);
			this.Name = "EncryptedCompressionTestScreen";
			this.Text = "CompressionTestForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Hydrogen.Windows.Forms.PathSelectorControl fileSelectorControl1;
		private System.Windows.Forms.Button _testButton;
		private System.Windows.Forms.TextBox textBox1;
		private Hydrogen.Windows.Forms.TextBoxEx _passwordTextBox;

	}
}
