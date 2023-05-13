// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
    partial class ListMerger {
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
            this._rightHeaderLabel = new System.Windows.Forms.Label();
            this._moveRightButton = new System.Windows.Forms.Button();
            this._moveLeftButton = new System.Windows.Forms.Button();
            this._leftHeaderLabel = new System.Windows.Forms.Label();
            this._rightListBox = new System.Windows.Forms.ListBox();
            this._leftListBox = new System.Windows.Forms.ListBox();
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this._tableLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rightHeaderLabel
            // 
            this._rightHeaderLabel.AutoEllipsis = true;
            this._rightHeaderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rightHeaderLabel.Location = new System.Drawing.Point(445, 0);
            this._rightHeaderLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this._rightHeaderLabel.Name = "_rightHeaderLabel";
            this._rightHeaderLabel.Size = new System.Drawing.Size(347, 30);
            this._rightHeaderLabel.TabIndex = 29;
            this._rightHeaderLabel.Text = "Right Header";
            this._rightHeaderLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _moveRightButton
            // 
            this._moveRightButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._moveRightButton.Image = global::Hydrogen.Windows.Forms.Resources.RArrow;
            this._moveRightButton.Location = new System.Drawing.Point(11, 130);
            this._moveRightButton.Margin = new System.Windows.Forms.Padding(6);
            this._moveRightButton.Name = "_moveRightButton";
            this._moveRightButton.Size = new System.Drawing.Size(48, 46);
            this._moveRightButton.TabIndex = 3;
            this._moveRightButton.UseVisualStyleBackColor = true;
            this._moveRightButton.Click += new System.EventHandler(this._moveRightButton_Click);
            // 
            // _moveLeftButton
            // 
            this._moveLeftButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._moveLeftButton.Image = global::Hydrogen.Windows.Forms.Resources.LArrow;
            this._moveLeftButton.Location = new System.Drawing.Point(11, 188);
            this._moveLeftButton.Margin = new System.Windows.Forms.Padding(6);
            this._moveLeftButton.Name = "_moveLeftButton";
            this._moveLeftButton.Size = new System.Drawing.Size(48, 46);
            this._moveLeftButton.TabIndex = 2;
            this._moveLeftButton.UseVisualStyleBackColor = true;
            this._moveLeftButton.Click += new System.EventHandler(this._moveLeftButton_Click);
            // 
            // _leftHeaderLabel
            // 
            this._leftHeaderLabel.AutoEllipsis = true;
            this._leftHeaderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._leftHeaderLabel.Location = new System.Drawing.Point(6, 0);
            this._leftHeaderLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this._leftHeaderLabel.Name = "_leftHeaderLabel";
            this._leftHeaderLabel.Size = new System.Drawing.Size(347, 30);
            this._leftHeaderLabel.TabIndex = 25;
            this._leftHeaderLabel.Text = "Left Header";
            this._leftHeaderLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _rightListBox
            // 
            this._rightListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rightListBox.FormattingEnabled = true;
            this._rightListBox.ItemHeight = 25;
            this._rightListBox.Location = new System.Drawing.Point(445, 36);
            this._rightListBox.Margin = new System.Windows.Forms.Padding(6);
            this._rightListBox.Name = "_rightListBox";
            this._rightListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._rightListBox.Size = new System.Drawing.Size(347, 384);
            this._rightListBox.TabIndex = 4;
            this._rightListBox.SelectedIndexChanged += new System.EventHandler(this._rightListBox_SelectedIndexChanged);
            this._rightListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._rightListBox_KeyDown);
            // 
            // _leftListBox
            // 
            this._leftListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._leftListBox.FormattingEnabled = true;
            this._leftListBox.ItemHeight = 25;
            this._leftListBox.Location = new System.Drawing.Point(6, 36);
            this._leftListBox.Margin = new System.Windows.Forms.Padding(6);
            this._leftListBox.Name = "_leftListBox";
            this._leftListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._leftListBox.Size = new System.Drawing.Size(347, 384);
            this._leftListBox.TabIndex = 1;
            this._leftListBox.SelectedIndexChanged += new System.EventHandler(this._leftListBox_SelectedIndexChanged);
            this._leftListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._leftListBox_KeyDown);
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 3;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tableLayoutPanel.Controls.Add(this._rightHeaderLabel, 2, 0);
            this._tableLayoutPanel.Controls.Add(this._rightListBox, 2, 1);
            this._tableLayoutPanel.Controls.Add(this._leftHeaderLabel, 0, 0);
            this._tableLayoutPanel.Controls.Add(this._leftListBox, 0, 1);
            this._tableLayoutPanel.Controls.Add(this.panel1, 1, 1);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 2;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(798, 426);
            this._tableLayoutPanel.TabIndex = 30;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._moveRightButton);
            this.panel1.Controls.Add(this._moveLeftButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(362, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(74, 390);
            this.panel1.TabIndex = 30;
            // 
            // ListMerger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ListMerger";
            this.Size = new System.Drawing.Size(798, 426);
            this._tableLayoutPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _rightHeaderLabel;
        private System.Windows.Forms.Button _moveRightButton;
        private System.Windows.Forms.Button _moveLeftButton;
        private System.Windows.Forms.Label _leftHeaderLabel;
        private System.Windows.Forms.ListBox _rightListBox;
        private System.Windows.Forms.ListBox _leftListBox;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.Panel panel1;
    }
}
