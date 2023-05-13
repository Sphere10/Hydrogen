// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms.MSSQL {
	partial class MSSQLConnectionBar {
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
			this._databaseTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this._serverTextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._usernameTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._passwordTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this._portTextBox = new System.Windows.Forms.TextBox();
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// OptionsButton
			// 
			this.OptionsButton.Location = new System.Drawing.Point(734, 15);
			// 
			// _databaseTextBox
			// 
			this._databaseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._databaseTextBox.Location = new System.Drawing.Point(229, 15);
			this._databaseTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._databaseTextBox.Name = "_databaseTextBox";
			this._databaseTextBox.Size = new System.Drawing.Size(135, 23);
			this._databaseTextBox.TabIndex = 2;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label6.Location = new System.Drawing.Point(226, 0);
			this.label6.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(138, 12);
			this.label6.TabIndex = 49;
			this.label6.Text = "Database";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// _serverTextBox
			// 
			this._serverTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._serverTextBox.Location = new System.Drawing.Point(3, 15);
			this._serverTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._serverTextBox.Name = "_serverTextBox";
			this._serverTextBox.Size = new System.Drawing.Size(220, 23);
			this._serverTextBox.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Location = new System.Drawing.Point(663, 0);
			this.label5.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(52, 12);
			this.label5.TabIndex = 47;
			this.label5.Text = "Port";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label4.Location = new System.Drawing.Point(508, 0);
			this.label4.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(149, 12);
			this.label4.TabIndex = 46;
			this.label4.Text = "Password";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// _usernameTextBox
			// 
			this._usernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._usernameTextBox.Location = new System.Drawing.Point(370, 15);
			this._usernameTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._usernameTextBox.Name = "_usernameTextBox";
			this._usernameTextBox.Size = new System.Drawing.Size(135, 23);
			this._usernameTextBox.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(367, 0);
			this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(138, 12);
			this.label3.TabIndex = 45;
			this.label3.Text = "Username";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// _passwordTextBox
			// 
			this._passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._passwordTextBox.Location = new System.Drawing.Point(511, 15);
			this._passwordTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._passwordTextBox.Name = "_passwordTextBox";
			this._passwordTextBox.PasswordChar = '*';
			this._passwordTextBox.Size = new System.Drawing.Size(146, 23);
			this._passwordTextBox.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(223, 12);
			this.label2.TabIndex = 44;
			this.label2.Text = "Server";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// _portTextBox
			// 
			this._portTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._portTextBox.Location = new System.Drawing.Point(663, 15);
			this._portTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._portTextBox.Name = "_portTextBox";
			this._portTextBox.Size = new System.Drawing.Size(52, 23);
			this._portTextBox.TabIndex = 5;
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
			this._tableLayoutPanel.ColumnCount = 6;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.3695F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.64968F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.64629F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.17564F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.158892F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 41F));
			this._tableLayoutPanel.Controls.Add(this._serverTextBox, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._databaseTextBox, 1, 1);
			this._tableLayoutPanel.Controls.Add(this._portTextBox, 4, 1);
			this._tableLayoutPanel.Controls.Add(this.label5, 4, 0);
			this._tableLayoutPanel.Controls.Add(this.label2, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._passwordTextBox, 3, 1);
			this._tableLayoutPanel.Controls.Add(this.label4, 3, 0);
			this._tableLayoutPanel.Controls.Add(this.label6, 1, 0);
			this._tableLayoutPanel.Controls.Add(this._usernameTextBox, 2, 1);
			this._tableLayoutPanel.Controls.Add(this.label3, 2, 0);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 2;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(762, 37);
			this._tableLayoutPanel.TabIndex = 50;
			// 
			// MSSQLConnectionBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "MSSQLConnectionBar";
			this.Size = new System.Drawing.Size(762, 37);
			this.Controls.SetChildIndex(this._tableLayoutPanel, 0);
			this.Controls.SetChildIndex(this.OptionsButton, 0);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox _databaseTextBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox _serverTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _usernameTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _passwordTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _portTextBox;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
    }
}
