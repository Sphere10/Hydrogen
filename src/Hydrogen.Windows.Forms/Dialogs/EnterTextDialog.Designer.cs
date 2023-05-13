// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class EnterTextDialog {
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
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this._userInstructionLabel = new System.Windows.Forms.Label();
			this._textBox = new Hydrogen.Windows.Forms.TextBoxEx();
			this.SuspendLayout();
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(474, 205);
			this.button2.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(88, 27);
			this.button2.TabIndex = 7;
			this.button2.Text = "&Cancel";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(374, 205);
			this.button1.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(88, 27);
			this.button1.TabIndex = 6;
			this.button1.Text = "&OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// _userInstructionLabel
			// 
			this._userInstructionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._userInstructionLabel.Location = new System.Drawing.Point(15, 9);
			this._userInstructionLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this._userInstructionLabel.Name = "_userInstructionLabel";
			this._userInstructionLabel.Size = new System.Drawing.Size(547, 22);
			this._userInstructionLabel.TabIndex = 5;
			this._userInstructionLabel.Text = "label1";
			this._userInstructionLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// _textBox
			// 
			this._textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._textBox.HideSelection = false;
			this._textBox.Location = new System.Drawing.Point(15, 38);
			this._textBox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
			this._textBox.Multiline = true;
			this._textBox.Name = "_textBox";
			this._textBox.Size = new System.Drawing.Size(547, 153);
			this._textBox.TabIndex = 4;
			// 
			// EnterTextDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(577, 238);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this._userInstructionLabel);
			this.Controls.Add(this._textBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
			this.Name = "EnterTextDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Enter Text";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label _userInstructionLabel;
		private TextBoxEx _textBox;
	}
}
