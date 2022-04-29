//-----------------------------------------------------------------------
// <copyright file="CrudTestForm.Designer.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester {
	partial class CrudTestScreen {
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
			this._outputTextBox = new System.Windows.Forms.TextBox();
			this._generateDeleteErrorCheckBox = new System.Windows.Forms.CheckBox();
			this._crudDialogButton = new System.Windows.Forms.Button();
			this._generateCreateErrorCheckBox = new System.Windows.Forms.CheckBox();
			this._generateUpdateErrorCheckBox = new System.Windows.Forms.CheckBox();
			this._selectFirstEntityButton = new System.Windows.Forms.Button();
			this._rightClickCheckBox = new System.Windows.Forms.CheckBox();
			this._leftClickCheckBox = new System.Windows.Forms.CheckBox();
			this._refreshGrid = new System.Windows.Forms.Button();
			this._autoSizeCheckBox = new System.Windows.Forms.CheckBox();
			this._autoSelectOnCreateCheckBox = new System.Windows.Forms.CheckBox();
			this._crudComboBox = new Hydrogen.Windows.Forms.Crud.CrudComboBox();
			this._flagsCheckedListBox = new Hydrogen.Windows.Forms.FlagsCheckedListBox();
			this._crudGrid = new Hydrogen.Windows.Forms.CrudGrid();
			this._allowCellEditingCheckBox = new System.Windows.Forms.CheckBox();
			this._refreshEntireGridOnUpdateCheckBox = new System.Windows.Forms.CheckBox();
			this._refreshEntireGridOnDeleteCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _outputTextBox
			// 
			this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputTextBox.Location = new System.Drawing.Point(12, 566);
			this._outputTextBox.Multiline = true;
			this._outputTextBox.Name = "_outputTextBox";
			this._outputTextBox.Size = new System.Drawing.Size(924, 82);
			this._outputTextBox.TabIndex = 2;
			// 
			// _generateDeleteErrorCheckBox
			// 
			this._generateDeleteErrorCheckBox.AutoSize = true;
			this._generateDeleteErrorCheckBox.Location = new System.Drawing.Point(16, 215);
			this._generateDeleteErrorCheckBox.Name = "_generateDeleteErrorCheckBox";
			this._generateDeleteErrorCheckBox.Size = new System.Drawing.Size(126, 17);
			this._generateDeleteErrorCheckBox.TabIndex = 3;
			this._generateDeleteErrorCheckBox.Text = "Generate delete error";
			this._generateDeleteErrorCheckBox.UseVisualStyleBackColor = true;
			this._generateDeleteErrorCheckBox.CheckedChanged += new System.EventHandler(this._generateDeleteErrorCheckBox_CheckedChanged);
			// 
			// _crudDialogButton
			// 
			this._crudDialogButton.Location = new System.Drawing.Point(17, 509);
			this._crudDialogButton.Name = "_crudDialogButton";
			this._crudDialogButton.Size = new System.Drawing.Size(126, 23);
			this._crudDialogButton.TabIndex = 4;
			this._crudDialogButton.Text = "Test CRUD Dialog";
			this._crudDialogButton.UseVisualStyleBackColor = true;
			this._crudDialogButton.Click += new System.EventHandler(this._crudDialogButton_Click);
			// 
			// _generateCreateErrorCheckBox
			// 
			this._generateCreateErrorCheckBox.AutoSize = true;
			this._generateCreateErrorCheckBox.Location = new System.Drawing.Point(16, 238);
			this._generateCreateErrorCheckBox.Name = "_generateCreateErrorCheckBox";
			this._generateCreateErrorCheckBox.Size = new System.Drawing.Size(127, 17);
			this._generateCreateErrorCheckBox.TabIndex = 6;
			this._generateCreateErrorCheckBox.Text = "Generate create error";
			this._generateCreateErrorCheckBox.UseVisualStyleBackColor = true;
			this._generateCreateErrorCheckBox.CheckedChanged += new System.EventHandler(this._generateCreateErrorCheckBox_CheckedChanged);
			// 
			// _generateUpdateErrorCheckBox
			// 
			this._generateUpdateErrorCheckBox.AutoSize = true;
			this._generateUpdateErrorCheckBox.Location = new System.Drawing.Point(16, 261);
			this._generateUpdateErrorCheckBox.Name = "_generateUpdateErrorCheckBox";
			this._generateUpdateErrorCheckBox.Size = new System.Drawing.Size(130, 17);
			this._generateUpdateErrorCheckBox.TabIndex = 7;
			this._generateUpdateErrorCheckBox.Text = "Generate update error";
			this._generateUpdateErrorCheckBox.UseVisualStyleBackColor = true;
			this._generateUpdateErrorCheckBox.CheckedChanged += new System.EventHandler(this._createUpdateErrorCheckBox_CheckedChanged);
			// 
			// _selectFirstEntityButton
			// 
			this._selectFirstEntityButton.Location = new System.Drawing.Point(17, 480);
			this._selectFirstEntityButton.Name = "_selectFirstEntityButton";
			this._selectFirstEntityButton.Size = new System.Drawing.Size(126, 23);
			this._selectFirstEntityButton.TabIndex = 8;
			this._selectFirstEntityButton.Text = "Select First Entity";
			this._selectFirstEntityButton.UseVisualStyleBackColor = true;
			this._selectFirstEntityButton.Click += new System.EventHandler(this._selectFirstEntityButton_Click);
			// 
			// _rightClickCheckBox
			// 
			this._rightClickCheckBox.AutoSize = true;
			this._rightClickCheckBox.Location = new System.Drawing.Point(16, 307);
			this._rightClickCheckBox.Name = "_rightClickCheckBox";
			this._rightClickCheckBox.Size = new System.Drawing.Size(120, 17);
			this._rightClickCheckBox.TabIndex = 10;
			this._rightClickCheckBox.Text = "Right click for menu";
			this._rightClickCheckBox.UseVisualStyleBackColor = true;
			this._rightClickCheckBox.CheckedChanged += new System.EventHandler(this._rightClickCheckBox_CheckedChanged);
			// 
			// _leftClickCheckBox
			// 
			this._leftClickCheckBox.AutoSize = true;
			this._leftClickCheckBox.Location = new System.Drawing.Point(16, 284);
			this._leftClickCheckBox.Name = "_leftClickCheckBox";
			this._leftClickCheckBox.Size = new System.Drawing.Size(124, 17);
			this._leftClickCheckBox.TabIndex = 9;
			this._leftClickCheckBox.Text = "Left click to deselect";
			this._leftClickCheckBox.UseVisualStyleBackColor = true;
			this._leftClickCheckBox.CheckedChanged += new System.EventHandler(this._leftClickCheckBox_CheckedChanged);
			// 
			// _refreshGrid
			// 
			this._refreshGrid.Location = new System.Drawing.Point(17, 451);
			this._refreshGrid.Name = "_refreshGrid";
			this._refreshGrid.Size = new System.Drawing.Size(126, 23);
			this._refreshGrid.TabIndex = 11;
			this._refreshGrid.Text = "Refresh Grid";
			this._refreshGrid.UseVisualStyleBackColor = true;
			this._refreshGrid.Click += new System.EventHandler(this._refreshGrid_Click);
			// 
			// _autoSizeCheckBox
			// 
			this._autoSizeCheckBox.AutoSize = true;
			this._autoSizeCheckBox.Location = new System.Drawing.Point(16, 328);
			this._autoSizeCheckBox.Name = "_autoSizeCheckBox";
			this._autoSizeCheckBox.Size = new System.Drawing.Size(96, 17);
			this._autoSizeCheckBox.TabIndex = 12;
			this._autoSizeCheckBox.Text = "Auto page size";
			this._autoSizeCheckBox.UseVisualStyleBackColor = true;
			this._autoSizeCheckBox.CheckedChanged += new System.EventHandler(this._autoSizeCheckBox_CheckedChanged);
			// 
			// _autoSelectOnCreateCheckBox
			// 
			this._autoSelectOnCreateCheckBox.AutoSize = true;
			this._autoSelectOnCreateCheckBox.Location = new System.Drawing.Point(16, 351);
			this._autoSelectOnCreateCheckBox.Name = "_autoSelectOnCreateCheckBox";
			this._autoSelectOnCreateCheckBox.Size = new System.Drawing.Size(127, 17);
			this._autoSelectOnCreateCheckBox.TabIndex = 13;
			this._autoSelectOnCreateCheckBox.Text = "Auto select on create";
			this._autoSelectOnCreateCheckBox.UseVisualStyleBackColor = true;
			this._autoSelectOnCreateCheckBox.CheckedChanged += new System.EventHandler(this._autoSelectOnCreateCheckBox_CheckedChanged_1);
			// 
			// _crudComboBox
			// 
			this._crudComboBox.AllowResizeDropDown = true;
			this._crudComboBox.ControlSize = new System.Drawing.Size(524, 306);
			this._crudComboBox.DropDownSizeMode = Hydrogen.Windows.Forms.CustomComboBox.SizeMode.UseInitialControlSize;
			this._crudComboBox.DropSize = new System.Drawing.Size(121, 106);
			this._crudComboBox.Location = new System.Drawing.Point(17, 538);
			this._crudComboBox.Name = "_crudComboBox";
			this._crudComboBox.PlaceHolderText = "Select Entity";
			this._crudComboBox.Size = new System.Drawing.Size(126, 21);
			this._crudComboBox.TabIndex = 5;
			this._crudComboBox.EntitySelectionChanged += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.Crud.CrudComboBox, object>(this._crudComboBox_EntitySelectionChanged);
			// 
			// _flagsCheckedListBox
			// 
			this._flagsCheckedListBox.BackColor = System.Drawing.SystemColors.Control;
			this._flagsCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._flagsCheckedListBox.CheckOnClick = true;
			this._flagsCheckedListBox.FormattingEnabled = true;
			this._flagsCheckedListBox.Location = new System.Drawing.Point(12, 12);
			this._flagsCheckedListBox.Name = "_flagsCheckedListBox";
			this._flagsCheckedListBox.Size = new System.Drawing.Size(167, 197);
			this._flagsCheckedListBox.TabIndex = 1;
			this._flagsCheckedListBox.SelectedValueChanged += new System.EventHandler(this._flagsCheckedListBox_SelectedValueChanged);
			// 
			// _crudGrid
			// 
			this._crudGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._crudGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._crudGrid.Capabilities = ((Hydrogen.DataSourceCapabilities)((Hydrogen.DataSourceCapabilities.CanCreate | Hydrogen.DataSourceCapabilities.CanRead)));
			this._crudGrid.Location = new System.Drawing.Point(185, 12);
			this._crudGrid.MinimumSize = new System.Drawing.Size(319, 87);
			this._crudGrid.Name = "_crudGrid";
			this._crudGrid.RightClickForContextMenu = false;
			this._crudGrid.Size = new System.Drawing.Size(751, 548);
			this._crudGrid.TabIndex = 0;
			this._crudGrid.EntitySelected += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.CrudGrid, object>(this._crudGrid_EntitySelected);
			this._crudGrid.EntityDeselected += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.CrudGrid, object>(this._crudGrid_EntityDeselected);
			this._crudGrid.EntityCreated += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.CrudGrid, object>(this._crudGrid_EntityCreated);
			this._crudGrid.EntityUpdated += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.CrudGrid, object>(this._crudGrid_EntityUpdated);
			this._crudGrid.EntityDeleted += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.CrudGrid, object>(this._crudGrid_EntityDeleted);
			// 
			// _allowCellEditingCheckBox
			// 
			this._allowCellEditingCheckBox.AutoSize = true;
			this._allowCellEditingCheckBox.Location = new System.Drawing.Point(16, 374);
			this._allowCellEditingCheckBox.Name = "_allowCellEditingCheckBox";
			this._allowCellEditingCheckBox.Size = new System.Drawing.Size(104, 17);
			this._allowCellEditingCheckBox.TabIndex = 14;
			this._allowCellEditingCheckBox.Text = "Allow cell editing";
			this._allowCellEditingCheckBox.UseVisualStyleBackColor = true;
			this._allowCellEditingCheckBox.CheckedChanged += new System.EventHandler(this._allowCellEditingCheckBox_CheckedChanged);
			// 
			// _refreshEntireGridOnUpdateCheckBox
			// 
			this._refreshEntireGridOnUpdateCheckBox.AutoSize = true;
			this._refreshEntireGridOnUpdateCheckBox.Location = new System.Drawing.Point(16, 397);
			this._refreshEntireGridOnUpdateCheckBox.Name = "_refreshEntireGridOnUpdateCheckBox";
			this._refreshEntireGridOnUpdateCheckBox.Size = new System.Drawing.Size(163, 17);
			this._refreshEntireGridOnUpdateCheckBox.TabIndex = 15;
			this._refreshEntireGridOnUpdateCheckBox.Text = "Refresh entire grid on update";
			this._refreshEntireGridOnUpdateCheckBox.UseVisualStyleBackColor = true;
			this._refreshEntireGridOnUpdateCheckBox.CheckedChanged += new System.EventHandler(this._refreshEntireGridOnUpdateCheckBox_CheckedChanged);
			// 
			// _refreshEntireGridOnDeleteCheckBox
			// 
			this._refreshEntireGridOnDeleteCheckBox.AutoSize = true;
			this._refreshEntireGridOnDeleteCheckBox.Location = new System.Drawing.Point(16, 420);
			this._refreshEntireGridOnDeleteCheckBox.Name = "_refreshEntireGridOnDeleteCheckBox";
			this._refreshEntireGridOnDeleteCheckBox.Size = new System.Drawing.Size(159, 17);
			this._refreshEntireGridOnDeleteCheckBox.TabIndex = 16;
			this._refreshEntireGridOnDeleteCheckBox.Text = "Refresh entire grid on delete";
			this._refreshEntireGridOnDeleteCheckBox.UseVisualStyleBackColor = true;
			this._refreshEntireGridOnDeleteCheckBox.CheckedChanged += new System.EventHandler(this._refreshEntireGridOnDeleteCheckBox_CheckedChanged);
			// 
			// CrudTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(948, 660);
			this.Controls.Add(this._refreshEntireGridOnDeleteCheckBox);
			this.Controls.Add(this._refreshEntireGridOnUpdateCheckBox);
			this.Controls.Add(this._allowCellEditingCheckBox);
			this.Controls.Add(this._autoSelectOnCreateCheckBox);
			this.Controls.Add(this._autoSizeCheckBox);
			this.Controls.Add(this._refreshGrid);
			this.Controls.Add(this._rightClickCheckBox);
			this.Controls.Add(this._leftClickCheckBox);
			this.Controls.Add(this._selectFirstEntityButton);
			this.Controls.Add(this._generateUpdateErrorCheckBox);
			this.Controls.Add(this._generateCreateErrorCheckBox);
			this.Controls.Add(this._crudComboBox);
			this.Controls.Add(this._crudDialogButton);
			this.Controls.Add(this._generateDeleteErrorCheckBox);
			this.Controls.Add(this._outputTextBox);
			this.Controls.Add(this._flagsCheckedListBox);
			this.Controls.Add(this._crudGrid);
			this.Name = "CrudTestScreen";
			this.Text = "CrudTestForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Hydrogen.Windows.Forms.CrudGrid _crudGrid;
		private Hydrogen.Windows.Forms.FlagsCheckedListBox _flagsCheckedListBox;
		private System.Windows.Forms.TextBox _outputTextBox;
		private System.Windows.Forms.CheckBox _generateDeleteErrorCheckBox;
		private System.Windows.Forms.Button _crudDialogButton;
		private Hydrogen.Windows.Forms.Crud.CrudComboBox _crudComboBox;
		private System.Windows.Forms.CheckBox _generateCreateErrorCheckBox;
		private System.Windows.Forms.CheckBox _generateUpdateErrorCheckBox;
		private System.Windows.Forms.Button _selectFirstEntityButton;
		private System.Windows.Forms.CheckBox _rightClickCheckBox;
		private System.Windows.Forms.CheckBox _leftClickCheckBox;
		private System.Windows.Forms.Button _refreshGrid;
		private System.Windows.Forms.CheckBox _autoSizeCheckBox;
		private System.Windows.Forms.CheckBox _autoSelectOnCreateCheckBox;
		private System.Windows.Forms.CheckBox _allowCellEditingCheckBox;
		private System.Windows.Forms.CheckBox _refreshEntireGridOnUpdateCheckBox;
		private System.Windows.Forms.CheckBox _refreshEntireGridOnDeleteCheckBox;

	}
}
