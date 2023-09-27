//-----------------------------------------------------------------------
// <copyright file="PasswordDialogTestForm.Designer.cs" company="Sphere 10 Software">
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
	partial class PasswordDialogTestScreen {
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
			this._standardButton = new System.Windows.Forms.Button();
			this._customButton = new System.Windows.Forms.Button();
			this._outputTextBox = new System.Windows.Forms.TextBox();
			this._logonButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _standardButton
			// 
			this._standardButton.Location = new System.Drawing.Point(12, 12);
			this._standardButton.Name = "_standardButton";
			this._standardButton.Size = new System.Drawing.Size(75, 23);
			this._standardButton.TabIndex = 0;
			this._standardButton.Text = "Standard";
			this._standardButton.UseVisualStyleBackColor = true;
			this._standardButton.Click += new System.EventHandler(this._standardButton_Click);
			// 
			// _customButton
			// 
			this._customButton.Location = new System.Drawing.Point(13, 42);
			this._customButton.Name = "_customButton";
			this._customButton.Size = new System.Drawing.Size(75, 23);
			this._customButton.TabIndex = 1;
			this._customButton.Text = "Custom";
			this._customButton.UseVisualStyleBackColor = true;
			this._customButton.Click += new System.EventHandler(this._customButton_Click);
			// 
			// _outputTextBox
			// 
			this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputTextBox.Location = new System.Drawing.Point(13, 148);
			this._outputTextBox.Multiline = true;
			this._outputTextBox.Name = "_outputTextBox";
			this._outputTextBox.Size = new System.Drawing.Size(623, 94);
			this._outputTextBox.TabIndex = 2;
			// 
			// _logonButton
			// 
			this._logonButton.Location = new System.Drawing.Point(194, 12);
			this._logonButton.Name = "_logonButton";
			this._logonButton.Size = new System.Drawing.Size(75, 23);
			this._logonButton.TabIndex = 3;
			this._logonButton.Text = "Logon";
			this._logonButton.UseVisualStyleBackColor = true;
			this._logonButton.Click += new System.EventHandler(this._logonButton_Click);
			// 
			// PasswordDialogTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(648, 254);
			this.Controls.Add(this._logonButton);
			this.Controls.Add(this._outputTextBox);
			this.Controls.Add(this._customButton);
			this.Controls.Add(this._standardButton);
			this.Name = "PasswordDialogTestScreen";
			this.Text = "PasswordDialogTestForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _standardButton;
		private System.Windows.Forms.Button _customButton;
		private System.Windows.Forms.TextBox _outputTextBox;
		private System.Windows.Forms.Button _logonButton;
	}
}
