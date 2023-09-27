// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class DatabaseConnectionPanel {
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
            this._dbmsCombo = new Hydrogen.Windows.Forms.EnumComboBox();
            this._connectionProviderPanel = new System.Windows.Forms.Panel();
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this._tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dbmsCombo
            // 
            this._dbmsCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._dbmsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._dbmsCombo.FormattingEnabled = true;
            this._dbmsCombo.Items.AddRange(new object[] {
            "SQL Server",
            "Sqlite",
            "Firebird (server)",
            "Firebird (file)"});
            this._dbmsCombo.Location = new System.Drawing.Point(70, 0);
            this._dbmsCombo.Margin = new System.Windows.Forms.Padding(0);
            this._dbmsCombo.Name = "_dbmsCombo";
            this._dbmsCombo.Size = new System.Drawing.Size(147, 21);
            this._dbmsCombo.TabIndex = 56;
            this._dbmsCombo.SelectedIndexChanged += new System.EventHandler(this._dbmsCombo_SelectedIndexChanged);
            // 
            // _connectionProviderPanel
            // 
            this._connectionProviderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableLayoutPanel.SetColumnSpan(this._connectionProviderPanel, 2);
            this._connectionProviderPanel.Location = new System.Drawing.Point(0, 24);
            this._connectionProviderPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._connectionProviderPanel.Name = "_connectionProviderPanel";
            this._connectionProviderPanel.Size = new System.Drawing.Size(617, 107);
            this._connectionProviderPanel.TabIndex = 54;
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 2;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Controls.Add(this.label2, 0, 0);
            this._tableLayoutPanel.Controls.Add(this._dbmsCombo, 1, 0);
            this._tableLayoutPanel.Controls.Add(this._connectionProviderPanel, 0, 1);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 2;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.33334F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(617, 131);
            this._tableLayoutPanel.TabIndex = 57;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 21);
            this.label2.TabIndex = 44;
            this.label2.Text = "DBMS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DatabaseConnectionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayoutPanel);
            this.Name = "DatabaseConnectionPanel";
            this.Size = new System.Drawing.Size(617, 131);
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
