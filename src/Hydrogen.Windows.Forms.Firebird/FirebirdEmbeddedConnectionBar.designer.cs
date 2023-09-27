// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms.Firebird {
	partial class FirebirdEmbeddedConnectionBar {
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
			this._usernameTextBox = new System.Windows.Forms.TextBox();
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._passwordTextBox = new System.Windows.Forms.TextBox();
			this._journalLabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._fileSelectorControl = new Hydrogen.Windows.Forms.PathSelectorControl();
			this.label5 = new System.Windows.Forms.Label();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// OptionsButton
			// 
			this.OptionsButton.Location = new System.Drawing.Point(631, 15);
			// 
			// _usernameTextBox
			// 
			this._usernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._usernameTextBox.Location = new System.Drawing.Point(434, 15);
			this._usernameTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._usernameTextBox.Name = "_usernameTextBox";
			this._usernameTextBox.Size = new System.Drawing.Size(94, 23);
			this._usernameTextBox.TabIndex = 2;
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
			this._tableLayoutPanel.ColumnCount = 4;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
			this._tableLayoutPanel.Controls.Add(this._passwordTextBox, 2, 1);
			this._tableLayoutPanel.Controls.Add(this._journalLabel, 1, 0);
			this._tableLayoutPanel.Controls.Add(this.label4, 2, 0);
			this._tableLayoutPanel.Controls.Add(this._fileSelectorControl, 0, 1);
			this._tableLayoutPanel.Controls.Add(this.label5, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._usernameTextBox, 1, 1);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 2;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(659, 37);
			this._tableLayoutPanel.TabIndex = 55;
			// 
			// _passwordTextBox
			// 
			this._passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._passwordTextBox.Location = new System.Drawing.Point(534, 15);
			this._passwordTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._passwordTextBox.Name = "_passwordTextBox";
			this._passwordTextBox.PasswordChar = '*';
			this._passwordTextBox.Size = new System.Drawing.Size(94, 23);
			this._passwordTextBox.TabIndex = 2;
			// 
			// _journalLabel
			// 
			this._journalLabel.AutoSize = true;
			this._journalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._journalLabel.Location = new System.Drawing.Point(431, 0);
			this._journalLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._journalLabel.Name = "_journalLabel";
			this._journalLabel.Size = new System.Drawing.Size(97, 12);
			this._journalLabel.TabIndex = 52;
			this._journalLabel.Text = "Username";
			this._journalLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label4.Location = new System.Drawing.Point(531, 0);
			this.label4.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(97, 12);
			this.label4.TabIndex = 49;
			this.label4.Text = "Password";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
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
			this._fileSelectorControl.Size = new System.Drawing.Size(428, 21);
			this._fileSelectorControl.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Location = new System.Drawing.Point(0, 0);
			this.label5.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(428, 12);
			this.label5.TabIndex = 46;
			this.label5.Text = "Filename";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FirebirdEmbeddedConnectionBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "FirebirdEmbeddedConnectionBar";
			this.Size = new System.Drawing.Size(659, 37);
			this.Controls.SetChildIndex(this._tableLayoutPanel, 0);
			this.Controls.SetChildIndex(this.OptionsButton, 0);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TextBox _usernameTextBox;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.TextBox _passwordTextBox;
        private System.Windows.Forms.Label _journalLabel;
        private System.Windows.Forms.Label label4;
        private PathSelectorControl _fileSelectorControl;
        private System.Windows.Forms.Label label5;
    }
}
