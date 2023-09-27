// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
    partial class PathSelectorControl {
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
            this._fileSelectorButton = new System.Windows.Forms.Button();
            this._filenameTextBox = new Hydrogen.Windows.Forms.TextBoxEx();
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _fileSelectorButton
            // 
            this._fileSelectorButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._fileSelectorButton.Location = new System.Drawing.Point(396, 0);
            this._fileSelectorButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this._fileSelectorButton.Name = "_fileSelectorButton";
            this._fileSelectorButton.Size = new System.Drawing.Size(27, 20);
            this._fileSelectorButton.TabIndex = 5;
            this._fileSelectorButton.Text = "...";
            this._fileSelectorButton.UseVisualStyleBackColor = true;
            this._fileSelectorButton.Click += new System.EventHandler(this._fileSelectorButton_Click);
            // 
            // _filenameTextBox
            // 
            this._filenameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._filenameTextBox.Location = new System.Drawing.Point(0, 0);
            this._filenameTextBox.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this._filenameTextBox.Name = "_filenameTextBox";
            this._filenameTextBox.Size = new System.Drawing.Size(390, 20);
            this._filenameTextBox.TabIndex = 4;
            this._filenameTextBox.Enter += new System.EventHandler(this._filenameTextBox_Enter);
            this._filenameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._filenameTextBox_Validating);
            this._filenameTextBox.Validated += new System.EventHandler(this._filenameTextBox_Validated);
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 2;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._tableLayoutPanel.Controls.Add(this._fileSelectorButton, 1, 0);
            this._tableLayoutPanel.Controls.Add(this._filenameTextBox, 0, 0);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 1;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(423, 20);
            this._tableLayoutPanel.TabIndex = 6;
            // 
            // PathSelectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PathSelectorControl";
            this.Size = new System.Drawing.Size(423, 20);
            this._tableLayoutPanel.ResumeLayout(false);
            this._tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _fileSelectorButton;
        private Hydrogen.Windows.Forms.TextBoxEx _filenameTextBox;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
    }
}
