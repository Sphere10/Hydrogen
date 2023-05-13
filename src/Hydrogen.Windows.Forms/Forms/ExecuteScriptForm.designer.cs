// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class ExecuteScriptForm {
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
			this._scriptTextBox = new System.Windows.Forms.TextBox();
			this._executeScriptButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._databaseConnectionStringLabel = new System.Windows.Forms.Label();
			this._copyToClipboardButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _scriptTextBox
			// 
			this._scriptTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._scriptTextBox.Location = new System.Drawing.Point(12, 25);
			this._scriptTextBox.Multiline = true;
			this._scriptTextBox.Name = "_scriptTextBox";
			this._scriptTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._scriptTextBox.Size = new System.Drawing.Size(706, 314);
			this._scriptTextBox.TabIndex = 0;
			this._scriptTextBox.WordWrap = false;
			// 
			// _executeScriptButton
			// 
			this._executeScriptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._executeScriptButton.Location = new System.Drawing.Point(592, 345);
			this._executeScriptButton.Name = "_executeScriptButton";
			this._executeScriptButton.Size = new System.Drawing.Size(126, 23);
			this._executeScriptButton.TabIndex = 1;
			this._executeScriptButton.Text = "Execute Script";
			this._executeScriptButton.UseVisualStyleBackColor = true;
			this._executeScriptButton.Click += new System.EventHandler(this._executeScriptButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Database:";
			// 
			// _databaseConnectionStringLabel
			// 
			this._databaseConnectionStringLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._databaseConnectionStringLabel.AutoEllipsis = true;
			this._databaseConnectionStringLabel.Location = new System.Drawing.Point(83, 9);
			this._databaseConnectionStringLabel.Name = "_databaseConnectionStringLabel";
			this._databaseConnectionStringLabel.Size = new System.Drawing.Size(635, 13);
			this._databaseConnectionStringLabel.TabIndex = 3;
			this._databaseConnectionStringLabel.Text = "label2";
			// 
			// _copyToClipboardButton
			// 
			this._copyToClipboardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._copyToClipboardButton.Location = new System.Drawing.Point(460, 345);
			this._copyToClipboardButton.Name = "_copyToClipboardButton";
			this._copyToClipboardButton.Size = new System.Drawing.Size(126, 23);
			this._copyToClipboardButton.TabIndex = 4;
			this._copyToClipboardButton.Text = "Copy to Clipboard";
			this._copyToClipboardButton.UseVisualStyleBackColor = true;
			this._copyToClipboardButton.Click += new System.EventHandler(this._copyToClipboardButton_Click);
			// 
			// ExecuteScriptForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(730, 380);
			this.Controls.Add(this._copyToClipboardButton);
			this.Controls.Add(this._databaseConnectionStringLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._executeScriptButton);
			this.Controls.Add(this._scriptTextBox);
			this.Name = "ExecuteScriptForm";
			this.Text = "ExecuteScriptForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _scriptTextBox;
		private System.Windows.Forms.Button _executeScriptButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label _databaseConnectionStringLabel;
		private System.Windows.Forms.Button _copyToClipboardButton;
	}
}
