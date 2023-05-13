// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class DatabaseConnectionBar {
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
            this._connectionProviderPanel = new System.Windows.Forms.Panel();
            this._dbmsCombo = new Hydrogen.Windows.Forms.EnumComboBox();
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this._tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // OptionsButton
            // 
            this.OptionsButton.Enabled = false;
            this.OptionsButton.Location = new System.Drawing.Point(750, 2);
            this.OptionsButton.Visible = false;
            // 
            // _connectionProviderPanel
            // 
            this._connectionProviderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._connectionProviderPanel.Location = new System.Drawing.Point(125, 0);
            this._connectionProviderPanel.Margin = new System.Windows.Forms.Padding(0);
            this._connectionProviderPanel.Name = "_connectionProviderPanel";
            this._tableLayoutPanel.SetRowSpan(this._connectionProviderPanel, 2);
            this._connectionProviderPanel.Size = new System.Drawing.Size(479, 40);
            this._connectionProviderPanel.TabIndex = 54;
            // 
            // _dbmsCombo
            // 
            this._dbmsCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._dbmsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._dbmsCombo.FormattingEnabled = true;
            this._dbmsCombo.Items.AddRange(new object[] {
            "SQL Server",
            "Sqlite",
            "Firebird (server)",
            "Firebird (file)"});
            this._dbmsCombo.Location = new System.Drawing.Point(3, 17);
            this._dbmsCombo.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this._dbmsCombo.Name = "_dbmsCombo";
            this._dbmsCombo.Size = new System.Drawing.Size(119, 21);
            this._dbmsCombo.TabIndex = 56;
            this._dbmsCombo.SelectedIndexChanged += new System.EventHandler(this._dbmsCombo_SelectedIndexChanged);
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this._tableLayoutPanel.ColumnCount = 2;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Controls.Add(this.label2, 0, 0);
            this._tableLayoutPanel.Controls.Add(this._dbmsCombo, 0, 1);
            this._tableLayoutPanel.Controls.Add(this._connectionProviderPanel, 1, 0);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 2;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(604, 40);
            this._tableLayoutPanel.TabIndex = 57;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 12);
            this.label2.TabIndex = 44;
            this.label2.Text = "DBMS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // DatabaseConnectionBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayoutPanel);
            this.MinimumSize = new System.Drawing.Size(500, 40);
            this.Name = "DatabaseConnectionBar";
            this.Size = new System.Drawing.Size(604, 40);
            this.Controls.SetChildIndex(this._tableLayoutPanel, 0);
            this.Controls.SetChildIndex(this.OptionsButton, 0);
            this._tableLayoutPanel.ResumeLayout(false);
            this._tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.Panel _connectionProviderPanel;
		protected Hydrogen.Windows.Forms.EnumComboBox _dbmsCombo;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.Label label2;
    }
}
