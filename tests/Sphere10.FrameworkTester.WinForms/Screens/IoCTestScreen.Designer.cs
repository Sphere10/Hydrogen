//-----------------------------------------------------------------------
// <copyright file="IoCTestForm.Designer.cs" company="Sphere 10 Software">
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

namespace Sphere10.FrameworkTester.WinForms {
	partial class IoCTestScreen {
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
			this._outputTextBox = new Sphere10.Framework.Windows.Forms.TextBoxEx();
			this.SuspendLayout();
			// 
			// _testButton
			// 
			this._testButton.Location = new System.Drawing.Point(12, 12);
			this._testButton.Name = "_testButton";
			this._testButton.Size = new System.Drawing.Size(75, 23);
			this._testButton.TabIndex = 0;
			this._testButton.Text = "Test";
			this._testButton.UseVisualStyleBackColor = true;
			this._testButton.Click += new System.EventHandler(this._testButton_Click);
			// 
			// _outputTextBox
			// 
			this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputTextBox.Location = new System.Drawing.Point(12, 41);
			this._outputTextBox.Multiline = true;
			this._outputTextBox.Name = "_outputTextBox";
			this._outputTextBox.Size = new System.Drawing.Size(617, 260);
			this._outputTextBox.TabIndex = 1;
			// 
			// IoCTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(641, 313);
			this.Controls.Add(this._outputTextBox);
			this.Controls.Add(this._testButton);
			this.Name = "IoCTestScreen";
			this.Text = "IoCTestForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _testButton;
		private Sphere10.Framework.Windows.Forms.TextBoxEx _outputTextBox;
	}
}
