// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class CrudEntityEditorDialog {
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
			this._entityEditorControlPanel = new System.Windows.Forms.Panel();
			this._saveButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._deleteButton = new System.Windows.Forms.Button();
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._buttonPanel = new System.Windows.Forms.Panel();
			this._errorRichTextBox = new System.Windows.Forms.RichTextBox();
			this._tableLayoutPanel.SuspendLayout();
			this._buttonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _entityEditorControlPanel
			// 
			this._entityEditorControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._entityEditorControlPanel.Location = new System.Drawing.Point(3, 3);
			this._entityEditorControlPanel.Name = "_entityEditorControlPanel";
			this._entityEditorControlPanel.Size = new System.Drawing.Size(290, 293);
			this._entityEditorControlPanel.TabIndex = 0;
			// 
			// _saveButton
			// 
			this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._saveButton.Location = new System.Drawing.Point(131, 1);
			this._saveButton.Name = "_saveButton";
			this._saveButton.Size = new System.Drawing.Size(75, 23);
			this._saveButton.TabIndex = 1;
			this._saveButton.Text = "&Save";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(212, 1);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 2;
			this._cancelButton.Text = "&Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _deleteButton
			// 
			this._deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._deleteButton.Location = new System.Drawing.Point(3, 1);
			this._deleteButton.Name = "_deleteButton";
			this._deleteButton.Size = new System.Drawing.Size(75, 23);
			this._deleteButton.TabIndex = 3;
			this._deleteButton.Text = "&Delete";
			this._deleteButton.UseVisualStyleBackColor = true;
			this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.ColumnCount = 1;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Controls.Add(this._entityEditorControlPanel, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._buttonPanel, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._errorRichTextBox, 0, 1);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 3;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(296, 329);
			this._tableLayoutPanel.TabIndex = 4;
			// 
			// _buttonPanel
			// 
			this._buttonPanel.Controls.Add(this._saveButton);
			this._buttonPanel.Controls.Add(this._deleteButton);
			this._buttonPanel.Controls.Add(this._cancelButton);
			this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._buttonPanel.Location = new System.Drawing.Point(3, 302);
			this._buttonPanel.Name = "_buttonPanel";
			this._buttonPanel.Size = new System.Drawing.Size(290, 24);
			this._buttonPanel.TabIndex = 1;
			// 
			// _errorRichTextBox
			// 
			this._errorRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._errorRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._errorRichTextBox.ForeColor = System.Drawing.Color.Red;
			this._errorRichTextBox.Location = new System.Drawing.Point(3, 302);
			this._errorRichTextBox.Name = "_errorRichTextBox";
			this._errorRichTextBox.ReadOnly = true;
			this._errorRichTextBox.Size = new System.Drawing.Size(290, 1);
			this._errorRichTextBox.TabIndex = 2;
			this._errorRichTextBox.Text = "";
			this._errorRichTextBox.WordWrap = false;
			// 
			// CrudEntityEditorDialog
			// 
			this.AcceptButton = this._saveButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(296, 329);
			this.Controls.Add(this._tableLayoutPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(265, 228);
			this.Name = "CrudEntityEditorDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EntityEditorDialog_FormClosing);
			this._tableLayoutPanel.ResumeLayout(false);
			this._buttonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _entityEditorControlPanel;
		private System.Windows.Forms.Button _saveButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _deleteButton;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.Panel _buttonPanel;
		private System.Windows.Forms.RichTextBox _errorRichTextBox;
	}
}
