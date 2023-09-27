// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms.Sqlite {
	partial class SqliteConnectionBar {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.label2 = new System.Windows.Forms.Label();
            this._fileSelectorControl = new Hydrogen.Windows.Forms.PathSelectorControl();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._syncComboBox = new Hydrogen.Windows.Forms.EnumComboBox();
            this._journalComboBox = new Hydrogen.Windows.Forms.EnumComboBox();
            this._journalLabel = new System.Windows.Forms.Label();
            this._syncLabel = new System.Windows.Forms.Label();
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // OptionsButton
            // 
            this.OptionsButton.Location = new System.Drawing.Point(631, 15);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(368, 12);
            this.label2.TabIndex = 46;
            this.label2.Text = "Filename";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _fileSelectorControl
            // 
            this._fileSelectorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._fileSelectorControl.Location = new System.Drawing.Point(0, 15);
            this._fileSelectorControl.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this._fileSelectorControl.Mode = Hydrogen.Windows.Forms.PathSelectionMode.File;
            this._fileSelectorControl.Name = "_fileSelectorControl";
            this._fileSelectorControl.Path = "";
            this._fileSelectorControl.PlaceHolderText = "";
            this._fileSelectorControl.Size = new System.Drawing.Size(368, 21);
            this._fileSelectorControl.TabIndex = 1;
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._passwordTextBox.Location = new System.Drawing.Point(534, 16);
            this._passwordTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.PasswordChar = '*';
            this._passwordTextBox.Size = new System.Drawing.Size(94, 20);
            this._passwordTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(531, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 12);
            this.label1.TabIndex = 49;
            this.label1.Text = "Password";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _syncComboBox
            // 
            this._syncComboBox.AllowEmptyOption = false;
            this._syncComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._syncComboBox.DisplayMember = "Display";
            this._syncComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._syncComboBox.EmptyOptionText = "";
            this._syncComboBox.FormattingEnabled = true;
            this._syncComboBox.Location = new System.Drawing.Point(454, 15);
            this._syncComboBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this._syncComboBox.Name = "_syncComboBox";
            this._syncComboBox.Size = new System.Drawing.Size(74, 21);
            this._syncComboBox.TabIndex = 50;
            this._syncComboBox.ValueMember = "Value";
            // 
            // _journalComboBox
            // 
            this._journalComboBox.AllowEmptyOption = false;
            this._journalComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._journalComboBox.DisplayMember = "Display";
            this._journalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._journalComboBox.EmptyOptionText = "";
            this._journalComboBox.FormattingEnabled = true;
            this._journalComboBox.Location = new System.Drawing.Point(374, 15);
            this._journalComboBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this._journalComboBox.Name = "_journalComboBox";
            this._journalComboBox.Size = new System.Drawing.Size(74, 21);
            this._journalComboBox.TabIndex = 51;
            this._journalComboBox.ValueMember = "Value";
            // 
            // _journalLabel
            // 
            this._journalLabel.AutoSize = true;
            this._journalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._journalLabel.Location = new System.Drawing.Point(371, 0);
            this._journalLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this._journalLabel.Name = "_journalLabel";
            this._journalLabel.Size = new System.Drawing.Size(77, 12);
            this._journalLabel.TabIndex = 52;
            this._journalLabel.Text = "Journal";
            this._journalLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _syncLabel
            // 
            this._syncLabel.AutoSize = true;
            this._syncLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._syncLabel.Location = new System.Drawing.Point(451, 0);
            this._syncLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this._syncLabel.Name = "_syncLabel";
            this._syncLabel.Size = new System.Drawing.Size(77, 12);
            this._syncLabel.TabIndex = 53;
            this._syncLabel.Text = "Sync";
            this._syncLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this._tableLayoutPanel.ColumnCount = 5;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this._tableLayoutPanel.Controls.Add(this._syncLabel, 2, 0);
            this._tableLayoutPanel.Controls.Add(this._passwordTextBox, 3, 1);
            this._tableLayoutPanel.Controls.Add(this._journalLabel, 1, 0);
            this._tableLayoutPanel.Controls.Add(this.label1, 3, 0);
            this._tableLayoutPanel.Controls.Add(this._syncComboBox, 2, 1);
            this._tableLayoutPanel.Controls.Add(this._fileSelectorControl, 0, 1);
            this._tableLayoutPanel.Controls.Add(this._journalComboBox, 1, 1);
            this._tableLayoutPanel.Controls.Add(this.label2, 0, 0);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 2;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(659, 37);
            this._tableLayoutPanel.TabIndex = 54;
            // 
            // SqliteConnectionBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayoutPanel);
            this.Name = "SqliteConnectionBar";
            this.Size = new System.Drawing.Size(659, 37);
            this.Controls.SetChildIndex(this._tableLayoutPanel, 0);
            this.Controls.SetChildIndex(this.OptionsButton, 0);
            this._tableLayoutPanel.ResumeLayout(false);
            this._tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private PathSelectorControl _fileSelectorControl;
		private System.Windows.Forms.TextBox _passwordTextBox;
		private System.Windows.Forms.Label label1;
        private EnumComboBox _syncComboBox;
        private EnumComboBox _journalComboBox;
        private System.Windows.Forms.Label _journalLabel;
        private System.Windows.Forms.Label _syncLabel;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
    }
}
