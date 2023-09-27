//-----------------------------------------------------------------------
// <copyright file="FlagsCheckedBoxListTestForm.Designer.cs" company="Sphere 10 Software">
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
	partial class FlagsCheckedBoxListScreen {
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
			this._outputTextBox = new System.Windows.Forms.TextBox();
			this._flagsCheckedListBox = new Hydrogen.Windows.Forms.FlagsCheckedListBox();
			this.SuspendLayout();
			// 
			// _outputTextBox
			// 
			this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputTextBox.Location = new System.Drawing.Point(12, 187);
			this._outputTextBox.Multiline = true;
			this._outputTextBox.Name = "_outputTextBox";
			this._outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._outputTextBox.Size = new System.Drawing.Size(260, 63);
			this._outputTextBox.TabIndex = 5;
			// 
			// _flagsCheckedListBox
			// 
			this._flagsCheckedListBox.CheckOnClick = true;
			this._flagsCheckedListBox.FormattingEnabled = true;
			this._flagsCheckedListBox.Location = new System.Drawing.Point(15, 12);
			this._flagsCheckedListBox.Name = "_flagsCheckedListBox";
			this._flagsCheckedListBox.Size = new System.Drawing.Size(257, 169);
			this._flagsCheckedListBox.TabIndex = 0;
			this._flagsCheckedListBox.SelectedValueChanged += new System.EventHandler(this._flagsCheckedListBox_SelectedValueChanged);
			// 
			// FlagsCheckedBoxListTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this._outputTextBox);
			this.Controls.Add(this._flagsCheckedListBox);
			this.Name = "FlagsCheckedBoxListScreen";
			this.Text = "FlagsCheckedListForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Hydrogen.Windows.Forms.FlagsCheckedListBox _flagsCheckedListBox;
		private System.Windows.Forms.TextBox _outputTextBox;
	}
}
