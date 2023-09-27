// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms.Firebird {
	partial class FirebirdConnectionPanel {
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
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._serverTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this._databaseTextBox = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this._usernameTextBox = new System.Windows.Forms.TextBox();
			this._passwordTextBox = new System.Windows.Forms.TextBox();
			this._portTextBox = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.ColumnCount = 2;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Controls.Add(this._serverTextBox, 1, 0);
			this._tableLayoutPanel.Controls.Add(this.label1, 0, 4);
			this._tableLayoutPanel.Controls.Add(this.label7, 0, 1);
			this._tableLayoutPanel.Controls.Add(this.label8, 0, 3);
			this._tableLayoutPanel.Controls.Add(this._databaseTextBox, 1, 1);
			this._tableLayoutPanel.Controls.Add(this.label9, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._usernameTextBox, 1, 2);
			this._tableLayoutPanel.Controls.Add(this._passwordTextBox, 1, 3);
			this._tableLayoutPanel.Controls.Add(this._portTextBox, 1, 4);
			this._tableLayoutPanel.Controls.Add(this.label10, 0, 0);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 5;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(558, 123);
			this._tableLayoutPanel.TabIndex = 51;
			// 
			// _serverTextBox
			// 
			this._serverTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._serverTextBox.Location = new System.Drawing.Point(70, 0);
			this._serverTextBox.Margin = new System.Windows.Forms.Padding(0);
			this._serverTextBox.Name = "_serverTextBox";
			this._serverTextBox.Size = new System.Drawing.Size(488, 23);
			this._serverTextBox.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(3, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 27);
			this.label1.TabIndex = 47;
			this.label1.Text = "Port";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label7.Location = new System.Drawing.Point(3, 24);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(64, 24);
			this.label7.TabIndex = 49;
			this.label7.Text = "Database";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label8.Location = new System.Drawing.Point(3, 72);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 24);
			this.label8.TabIndex = 46;
			this.label8.Text = "Password";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _databaseTextBox
			// 
			this._databaseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._databaseTextBox.Location = new System.Drawing.Point(70, 24);
			this._databaseTextBox.Margin = new System.Windows.Forms.Padding(0);
			this._databaseTextBox.Name = "_databaseTextBox";
			this._databaseTextBox.Size = new System.Drawing.Size(488, 23);
			this._databaseTextBox.TabIndex = 2;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label9.Location = new System.Drawing.Point(3, 48);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(64, 24);
			this.label9.TabIndex = 45;
			this.label9.Text = "Username";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _usernameTextBox
			// 
			this._usernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._usernameTextBox.Location = new System.Drawing.Point(70, 48);
			this._usernameTextBox.Margin = new System.Windows.Forms.Padding(0);
			this._usernameTextBox.Name = "_usernameTextBox";
			this._usernameTextBox.Size = new System.Drawing.Size(488, 23);
			this._usernameTextBox.TabIndex = 3;
			// 
			// _passwordTextBox
			// 
			this._passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._passwordTextBox.Location = new System.Drawing.Point(70, 72);
			this._passwordTextBox.Margin = new System.Windows.Forms.Padding(0);
			this._passwordTextBox.Name = "_passwordTextBox";
			this._passwordTextBox.PasswordChar = '*';
			this._passwordTextBox.Size = new System.Drawing.Size(488, 23);
			this._passwordTextBox.TabIndex = 4;
			// 
			// _portTextBox
			// 
			this._portTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._portTextBox.Location = new System.Drawing.Point(70, 98);
			this._portTextBox.Margin = new System.Windows.Forms.Padding(0);
			this._portTextBox.Name = "_portTextBox";
			this._portTextBox.Size = new System.Drawing.Size(488, 23);
			this._portTextBox.TabIndex = 5;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label10.Location = new System.Drawing.Point(3, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(64, 24);
			this.label10.TabIndex = 44;
			this.label10.Text = "Server";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FirebirdConnectionPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayoutPanel);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "FirebirdConnectionPanel";
			this.Size = new System.Drawing.Size(558, 123);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

		}

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.TextBox _serverTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _databaseTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox _usernameTextBox;
        private System.Windows.Forms.TextBox _passwordTextBox;
        private System.Windows.Forms.TextBox _portTextBox;
        private System.Windows.Forms.Label label10;
    }
}
