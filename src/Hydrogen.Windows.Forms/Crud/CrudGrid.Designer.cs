// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class CrudGrid {
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
			this.components = new System.ComponentModel.Container();
			this._layoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._topPanel = new System.Windows.Forms.Panel();
			this._titleLabel = new System.Windows.Forms.Label();
			this._createButton = new System.Windows.Forms.Button();
			this._deleteButton = new System.Windows.Forms.Button();
			this._bottomPanel = new System.Windows.Forms.Panel();
			this._lastPageButton = new System.Windows.Forms.Button();
			this._firstPageButton = new System.Windows.Forms.Button();
			this._nextPageButton = new System.Windows.Forms.Button();
			this._pageSizeUpDown = new System.Windows.Forms.NumericUpDown();
			this._previousPageButton = new System.Windows.Forms.Button();
			this._totalRecordsLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._pageSizeLabel = new System.Windows.Forms.Label();
			this._gridContainerPanel = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this._deselectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._selectionContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._pageCountLabel = new System.Windows.Forms.Label();
			this._searchTextBox = new Hydrogen.Windows.Forms.SearchTextBox();
			this._pageNumberBox = new Hydrogen.Windows.Forms.IntBox();
			this._grid = new SourceGrid.Grid();
			this._layoutPanel.SuspendLayout();
			this._topPanel.SuspendLayout();
			this._bottomPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._pageSizeUpDown)).BeginInit();
			this._gridContainerPanel.SuspendLayout();
			this._selectionContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _layoutPanel
			// 
			this._layoutPanel.ColumnCount = 1;
			this._layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutPanel.Controls.Add(this._topPanel, 0, 0);
			this._layoutPanel.Controls.Add(this._bottomPanel, 0, 2);
			this._layoutPanel.Controls.Add(this._gridContainerPanel, 0, 1);
			this._layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layoutPanel.Location = new System.Drawing.Point(0, 0);
			this._layoutPanel.Name = "_layoutPanel";
			this._layoutPanel.RowCount = 3;
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this._layoutPanel.Size = new System.Drawing.Size(524, 306);
			this._layoutPanel.TabIndex = 1;
			// 
			// _topPanel
			// 
			this._topPanel.Controls.Add(this._titleLabel);
			this._topPanel.Controls.Add(this._searchTextBox);
			this._topPanel.Controls.Add(this._createButton);
			this._topPanel.Controls.Add(this._deleteButton);
			this._topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._topPanel.Location = new System.Drawing.Point(3, 3);
			this._topPanel.Name = "_topPanel";
			this._topPanel.Size = new System.Drawing.Size(518, 25);
			this._topPanel.TabIndex = 2;
			// 
			// _titleLabel
			// 
			this._titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleLabel.Location = new System.Drawing.Point(62, 5);
			this._titleLabel.Name = "_titleLabel";
			this._titleLabel.Size = new System.Drawing.Size(331, 18);
			this._titleLabel.TabIndex = 3;
			this._titleLabel.Text = "label5";
			this._titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _createButton
			// 
			this._createButton.Image = global::Hydrogen.Windows.Forms.Resources.Plus;
			this._createButton.Location = new System.Drawing.Point(1, 2);
			this._createButton.Name = "_createButton";
			this._createButton.Size = new System.Drawing.Size(28, 23);
			this._createButton.TabIndex = 1;
			this._createButton.UseVisualStyleBackColor = true;
			this._createButton.Click += new System.EventHandler(this._createButton_Click);
			// 
			// _deleteButton
			// 
			this._deleteButton.Image = global::Hydrogen.Windows.Forms.Resources.Minus;
			this._deleteButton.Location = new System.Drawing.Point(28, 2);
			this._deleteButton.Name = "_deleteButton";
			this._deleteButton.Size = new System.Drawing.Size(28, 23);
			this._deleteButton.TabIndex = 0;
			this._deleteButton.Tag = "2";
			this._deleteButton.UseVisualStyleBackColor = true;
			this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
			// 
			// _bottomPanel
			// 
			this._bottomPanel.Controls.Add(this._pageCountLabel);
			this._bottomPanel.Controls.Add(this._pageNumberBox);
			this._bottomPanel.Controls.Add(this._lastPageButton);
			this._bottomPanel.Controls.Add(this._firstPageButton);
			this._bottomPanel.Controls.Add(this._nextPageButton);
			this._bottomPanel.Controls.Add(this._pageSizeUpDown);
			this._bottomPanel.Controls.Add(this._previousPageButton);
			this._bottomPanel.Controls.Add(this._totalRecordsLabel);
			this._bottomPanel.Controls.Add(this.label3);
			this._bottomPanel.Controls.Add(this._pageSizeLabel);
			this._bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bottomPanel.Location = new System.Drawing.Point(3, 278);
			this._bottomPanel.Name = "_bottomPanel";
			this._bottomPanel.Size = new System.Drawing.Size(518, 25);
			this._bottomPanel.TabIndex = 3;
			// 
			// _lastPageButton
			// 
			this._lastPageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._lastPageButton.Image = global::Hydrogen.Windows.Forms.Resources.MaxArrow;
			this._lastPageButton.Location = new System.Drawing.Point(492, 2);
			this._lastPageButton.Name = "_lastPageButton";
			this._lastPageButton.Size = new System.Drawing.Size(20, 20);
			this._lastPageButton.TabIndex = 12;
			this._lastPageButton.UseVisualStyleBackColor = true;
			this._lastPageButton.Click += new System.EventHandler(this._lastPageButton_Click);
			// 
			// _firstPageButton
			// 
			this._firstPageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._firstPageButton.Image = global::Hydrogen.Windows.Forms.Resources.MinArrow;
			this._firstPageButton.Location = new System.Drawing.Point(334, 2);
			this._firstPageButton.Name = "_firstPageButton";
			this._firstPageButton.Size = new System.Drawing.Size(20, 20);
			this._firstPageButton.TabIndex = 11;
			this._firstPageButton.UseVisualStyleBackColor = true;
			this._firstPageButton.Click += new System.EventHandler(this._firstPageButton_Click);
			// 
			// _nextPageButton
			// 
			this._nextPageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._nextPageButton.Image = global::Hydrogen.Windows.Forms.Resources.RArrow;
			this._nextPageButton.Location = new System.Drawing.Point(473, 2);
			this._nextPageButton.Name = "_nextPageButton";
			this._nextPageButton.Size = new System.Drawing.Size(20, 20);
			this._nextPageButton.TabIndex = 10;
			this._nextPageButton.UseVisualStyleBackColor = true;
			this._nextPageButton.Click += new System.EventHandler(this._nextPageButton_Click);
			// 
			// _pageSizeUpDown
			// 
			this._pageSizeUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._pageSizeUpDown.Font = new System.Drawing.Font("Arial", 8.25F);
			this._pageSizeUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._pageSizeUpDown.Location = new System.Drawing.Point(158, 3);
			this._pageSizeUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._pageSizeUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._pageSizeUpDown.Name = "_pageSizeUpDown";
			this._pageSizeUpDown.Size = new System.Drawing.Size(45, 20);
			this._pageSizeUpDown.TabIndex = 7;
			this._pageSizeUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._pageSizeUpDown.ValueChanged += new System.EventHandler(this._pageSizeUpDown_ValueChanged);
			// 
			// _previousPageButton
			// 
			this._previousPageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._previousPageButton.Image = global::Hydrogen.Windows.Forms.Resources.LArrow;
			this._previousPageButton.Location = new System.Drawing.Point(353, 2);
			this._previousPageButton.Name = "_previousPageButton";
			this._previousPageButton.Size = new System.Drawing.Size(20, 20);
			this._previousPageButton.TabIndex = 9;
			this._previousPageButton.UseVisualStyleBackColor = true;
			this._previousPageButton.Click += new System.EventHandler(this._previousPageButton_Click);
			// 
			// _totalRecordsLabel
			// 
			this._totalRecordsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._totalRecordsLabel.AutoSize = true;
			this._totalRecordsLabel.Font = new System.Drawing.Font("Arial", 8.25F);
			this._totalRecordsLabel.Location = new System.Drawing.Point(34, 5);
			this._totalRecordsLabel.Name = "_totalRecordsLabel";
			this._totalRecordsLabel.Size = new System.Drawing.Size(13, 14);
			this._totalRecordsLabel.TabIndex = 5;
			this._totalRecordsLabel.Text = "0";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial", 8.25F);
			this.label3.Location = new System.Drawing.Point(4, 5);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 14);
			this.label3.TabIndex = 4;
			this.label3.Text = "Total:";
			// 
			// _pageSizeLabel
			// 
			this._pageSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._pageSizeLabel.AutoSize = true;
			this._pageSizeLabel.Font = new System.Drawing.Font("Arial", 8.25F);
			this._pageSizeLabel.Location = new System.Drawing.Point(94, 5);
			this._pageSizeLabel.Name = "_pageSizeLabel";
			this._pageSizeLabel.Size = new System.Drawing.Size(58, 14);
			this._pageSizeLabel.TabIndex = 3;
			this._pageSizeLabel.Text = "Page Size:";
			// 
			// _gridContainerPanel
			// 
			this._gridContainerPanel.Controls.Add(this._grid);
			this._gridContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._gridContainerPanel.Location = new System.Drawing.Point(0, 31);
			this._gridContainerPanel.Margin = new System.Windows.Forms.Padding(0);
			this._gridContainerPanel.Name = "_gridContainerPanel";
			this._gridContainerPanel.Size = new System.Drawing.Size(524, 244);
			this._gridContainerPanel.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.AutoEllipsis = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(36, 5);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(61, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "9999999";
			// 
			// _deselectToolStripMenuItem
			// 
			this._deselectToolStripMenuItem.Name = "_deselectToolStripMenuItem";
			this._deselectToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this._deselectToolStripMenuItem.Text = "Deselect";
			this._deselectToolStripMenuItem.Click += new System.EventHandler(this._deselectToolStripMenuItem_Click);
			// 
			// _editToolStripMenuItem
			// 
			this._editToolStripMenuItem.Name = "_editToolStripMenuItem";
			this._editToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this._editToolStripMenuItem.Text = "&Edit";
			this._editToolStripMenuItem.Click += new System.EventHandler(this._editToolStripMenuItem_Click);
			// 
			// _deleteToolStripMenuItem
			// 
			this._deleteToolStripMenuItem.Name = "_deleteToolStripMenuItem";
			this._deleteToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this._deleteToolStripMenuItem.Text = "&Delete";
			this._deleteToolStripMenuItem.Click += new System.EventHandler(this._deleteToolStripMenuItem_Click);
			// 
			// _selectionContextMenuStrip
			// 
			this._selectionContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._deselectToolStripMenuItem,
            this._editToolStripMenuItem,
            this._deleteToolStripMenuItem});
			this._selectionContextMenuStrip.Name = "_selectionContextMenuStrip";
			this._selectionContextMenuStrip.Size = new System.Drawing.Size(119, 70);
			// 
			// _pageCountLabel
			// 
			this._pageCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._pageCountLabel.AutoEllipsis = true;
			this._pageCountLabel.BackColor = System.Drawing.Color.Transparent;
			this._pageCountLabel.Font = new System.Drawing.Font("Arial", 8.25F);
			this._pageCountLabel.Location = new System.Drawing.Point(430, 5);
			this._pageCountLabel.Name = "_pageCountLabel";
			this._pageCountLabel.Size = new System.Drawing.Size(43, 13);
			this._pageCountLabel.TabIndex = 14;
			this._pageCountLabel.Text = "/ 12345";
			// 
			// _searchTextBox
			// 
			this._searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._searchTextBox.Location = new System.Drawing.Point(399, 3);
			this._searchTextBox.Name = "_searchTextBox";
			this._searchTextBox.PlaceHolderText = "Search";
			this._searchTextBox.Size = new System.Drawing.Size(116, 20);
			this._searchTextBox.TabIndex = 2;
			this._searchTextBox.TextChanged += new System.EventHandler(this._searchTextBox_TextChanged);
			// 
			// _pageNumberBox
			// 
			this._pageNumberBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._pageNumberBox.Font = new System.Drawing.Font("Arial", 8.25F);
			this._pageNumberBox.Location = new System.Drawing.Point(379, 2);
			this._pageNumberBox.Name = "_pageNumberBox";
			this._pageNumberBox.NullText = "1";
			this._pageNumberBox.Size = new System.Drawing.Size(49, 20);
			this._pageNumberBox.TabIndex = 13;
			this._pageNumberBox.Text = "1";
			this._pageNumberBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._pageNumberBox.Value = 1;
			this._pageNumberBox.ValueChanged += new System.EventHandler(this._pageNumberBox_ValueChanged);
			// 
			// _grid
			// 
			this._grid.AutoSize = true;
			this._grid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._grid.CustomSort = true;
			this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this._grid.EnableSort = false;
			this._grid.Location = new System.Drawing.Point(0, 0);
			this._grid.Margin = new System.Windows.Forms.Padding(0);
			this._grid.Name = "_grid";
			this._grid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
			this._grid.SelectionMode = SourceGrid.GridSelectionMode.Row;
			this._grid.Size = new System.Drawing.Size(524, 244);
			this._grid.TabIndex = 5;
			this._grid.TabStop = true;
			this._grid.ToolTipText = "";
			this._grid.MouseClick += new System.Windows.Forms.MouseEventHandler(this._grid_MouseClick);
			this._grid.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._grid_MouseDoubleClick);
			// 
			// CrudGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._layoutPanel);
			this.MinimumSize = new System.Drawing.Size(319, 87);
			this.Name = "CrudGrid";
			this.Size = new System.Drawing.Size(524, 306);
			this._layoutPanel.ResumeLayout(false);
			this._topPanel.ResumeLayout(false);
			this._topPanel.PerformLayout();
			this._bottomPanel.ResumeLayout(false);
			this._bottomPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._pageSizeUpDown)).EndInit();
			this._gridContainerPanel.ResumeLayout(false);
			this._gridContainerPanel.PerformLayout();
			this._selectionContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _layoutPanel;
		private System.Windows.Forms.Panel _topPanel;
		private System.Windows.Forms.Button _createButton;
		private System.Windows.Forms.Button _deleteButton;
		private System.Windows.Forms.Panel _bottomPanel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label _pageSizeLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label _totalRecordsLabel;
		private System.Windows.Forms.NumericUpDown _pageSizeUpDown;
		private SearchTextBox _searchTextBox;
		private System.Windows.Forms.Label _titleLabel;
		private System.Windows.Forms.Button _nextPageButton;
		private System.Windows.Forms.Button _previousPageButton;
		private System.Windows.Forms.Button _lastPageButton;
		private System.Windows.Forms.Button _firstPageButton;
		private IntBox _pageNumberBox;
		private System.Windows.Forms.ToolStripMenuItem _deselectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _deleteToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip _selectionContextMenuStrip;
		private System.Windows.Forms.Panel _gridContainerPanel;
		private SourceGrid.Grid _grid;
		private System.Windows.Forms.Label _pageCountLabel;

	}
}
