//-----------------------------------------------------------------------
// <copyright file="CrudTestForm.Designer.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
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
			_outputTextBox = new System.Windows.Forms.TextBox();
			_generateDeleteErrorCheckBox = new System.Windows.Forms.CheckBox();
			_crudDialogButton = new System.Windows.Forms.Button();
			_generateCreateErrorCheckBox = new System.Windows.Forms.CheckBox();
			_generateUpdateErrorCheckBox = new System.Windows.Forms.CheckBox();
			_selectFirstEntityButton = new System.Windows.Forms.Button();
			_rightClickCheckBox = new System.Windows.Forms.CheckBox();
			_leftClickCheckBox = new System.Windows.Forms.CheckBox();
			_refreshGrid = new System.Windows.Forms.Button();
			_autoSizeCheckBox = new System.Windows.Forms.CheckBox();
			_autoSelectOnCreateCheckBox = new System.Windows.Forms.CheckBox();
			_crudComboBox = new Windows.Forms.Crud.CrudComboBox();
			_flagsCheckedListBox = new FlagsCheckedListBox();
			_crudGrid = new CrudGrid();
			_allowCellEditingCheckBox = new System.Windows.Forms.CheckBox();
			_refreshEntireGridOnUpdateCheckBox = new System.Windows.Forms.CheckBox();
			_refreshEntireGridOnDeleteCheckBox = new System.Windows.Forms.CheckBox();
			SuspendLayout();
			// 
			// _outputTextBox
			// 
			_outputTextBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_outputTextBox.Location = new System.Drawing.Point(14, 653);
			_outputTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_outputTextBox.Multiline = true;
			_outputTextBox.Name = "_outputTextBox";
			_outputTextBox.Size = new System.Drawing.Size(1077, 94);
			_outputTextBox.TabIndex = 2;
			// 
			// _generateDeleteErrorCheckBox
			// 
			_generateDeleteErrorCheckBox.AutoSize = true;
			_generateDeleteErrorCheckBox.Location = new System.Drawing.Point(19, 248);
			_generateDeleteErrorCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_generateDeleteErrorCheckBox.Name = "_generateDeleteErrorCheckBox";
			_generateDeleteErrorCheckBox.Size = new System.Drawing.Size(136, 19);
			_generateDeleteErrorCheckBox.TabIndex = 3;
			_generateDeleteErrorCheckBox.Text = "Generate delete error";
			_generateDeleteErrorCheckBox.UseVisualStyleBackColor = true;
			_generateDeleteErrorCheckBox.CheckedChanged += _generateDeleteErrorCheckBox_CheckedChanged;
			// 
			// _crudDialogButton
			// 
			_crudDialogButton.Location = new System.Drawing.Point(20, 587);
			_crudDialogButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_crudDialogButton.Name = "_crudDialogButton";
			_crudDialogButton.Size = new System.Drawing.Size(147, 27);
			_crudDialogButton.TabIndex = 4;
			_crudDialogButton.Text = "Test CRUD Dialog";
			_crudDialogButton.UseVisualStyleBackColor = true;
			_crudDialogButton.Click += _crudDialogButton_Click;
			// 
			// _generateCreateErrorCheckBox
			// 
			_generateCreateErrorCheckBox.AutoSize = true;
			_generateCreateErrorCheckBox.Location = new System.Drawing.Point(19, 275);
			_generateCreateErrorCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_generateCreateErrorCheckBox.Name = "_generateCreateErrorCheckBox";
			_generateCreateErrorCheckBox.Size = new System.Drawing.Size(136, 19);
			_generateCreateErrorCheckBox.TabIndex = 6;
			_generateCreateErrorCheckBox.Text = "Generate create error";
			_generateCreateErrorCheckBox.UseVisualStyleBackColor = true;
			_generateCreateErrorCheckBox.CheckedChanged += _generateCreateErrorCheckBox_CheckedChanged;
			// 
			// _generateUpdateErrorCheckBox
			// 
			_generateUpdateErrorCheckBox.AutoSize = true;
			_generateUpdateErrorCheckBox.Location = new System.Drawing.Point(19, 301);
			_generateUpdateErrorCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_generateUpdateErrorCheckBox.Name = "_generateUpdateErrorCheckBox";
			_generateUpdateErrorCheckBox.Size = new System.Drawing.Size(141, 19);
			_generateUpdateErrorCheckBox.TabIndex = 7;
			_generateUpdateErrorCheckBox.Text = "Generate update error";
			_generateUpdateErrorCheckBox.UseVisualStyleBackColor = true;
			_generateUpdateErrorCheckBox.CheckedChanged += _createUpdateErrorCheckBox_CheckedChanged;
			// 
			// _selectFirstEntityButton
			// 
			_selectFirstEntityButton.Location = new System.Drawing.Point(20, 554);
			_selectFirstEntityButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_selectFirstEntityButton.Name = "_selectFirstEntityButton";
			_selectFirstEntityButton.Size = new System.Drawing.Size(147, 27);
			_selectFirstEntityButton.TabIndex = 8;
			_selectFirstEntityButton.Text = "Select First Entity";
			_selectFirstEntityButton.UseVisualStyleBackColor = true;
			_selectFirstEntityButton.Click += _selectFirstEntityButton_Click;
			// 
			// _rightClickCheckBox
			// 
			_rightClickCheckBox.AutoSize = true;
			_rightClickCheckBox.Location = new System.Drawing.Point(19, 354);
			_rightClickCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_rightClickCheckBox.Name = "_rightClickCheckBox";
			_rightClickCheckBox.Size = new System.Drawing.Size(133, 19);
			_rightClickCheckBox.TabIndex = 10;
			_rightClickCheckBox.Text = "Right click for menu";
			_rightClickCheckBox.UseVisualStyleBackColor = true;
			_rightClickCheckBox.CheckedChanged += _rightClickCheckBox_CheckedChanged;
			// 
			// _leftClickCheckBox
			// 
			_leftClickCheckBox.AutoSize = true;
			_leftClickCheckBox.Location = new System.Drawing.Point(19, 328);
			_leftClickCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_leftClickCheckBox.Name = "_leftClickCheckBox";
			_leftClickCheckBox.Size = new System.Drawing.Size(133, 19);
			_leftClickCheckBox.TabIndex = 9;
			_leftClickCheckBox.Text = "Left click to deselect";
			_leftClickCheckBox.UseVisualStyleBackColor = true;
			_leftClickCheckBox.CheckedChanged += _leftClickCheckBox_CheckedChanged;
			// 
			// _refreshGrid
			// 
			_refreshGrid.Location = new System.Drawing.Point(20, 520);
			_refreshGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_refreshGrid.Name = "_refreshGrid";
			_refreshGrid.Size = new System.Drawing.Size(147, 27);
			_refreshGrid.TabIndex = 11;
			_refreshGrid.Text = "Refresh Grid";
			_refreshGrid.UseVisualStyleBackColor = true;
			_refreshGrid.Click += _refreshGrid_Click;
			// 
			// _autoSizeCheckBox
			// 
			_autoSizeCheckBox.AutoSize = true;
			_autoSizeCheckBox.Location = new System.Drawing.Point(19, 378);
			_autoSizeCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_autoSizeCheckBox.Name = "_autoSizeCheckBox";
			_autoSizeCheckBox.Size = new System.Drawing.Size(103, 19);
			_autoSizeCheckBox.TabIndex = 12;
			_autoSizeCheckBox.Text = "Auto page size";
			_autoSizeCheckBox.UseVisualStyleBackColor = true;
			_autoSizeCheckBox.CheckedChanged += _autoSizeCheckBox_CheckedChanged;
			// 
			// _autoSelectOnCreateCheckBox
			// 
			_autoSelectOnCreateCheckBox.AutoSize = true;
			_autoSelectOnCreateCheckBox.Location = new System.Drawing.Point(19, 405);
			_autoSelectOnCreateCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_autoSelectOnCreateCheckBox.Name = "_autoSelectOnCreateCheckBox";
			_autoSelectOnCreateCheckBox.Size = new System.Drawing.Size(137, 19);
			_autoSelectOnCreateCheckBox.TabIndex = 13;
			_autoSelectOnCreateCheckBox.Text = "Auto select on create";
			_autoSelectOnCreateCheckBox.UseVisualStyleBackColor = true;
			_autoSelectOnCreateCheckBox.CheckedChanged += _autoSelectOnCreateCheckBox_CheckedChanged_1;
			// 
			// _crudComboBox
			// 
			_crudComboBox.AllowResizeDropDown = true;
			_crudComboBox.ControlSize = new System.Drawing.Size(524, 306);
			_crudComboBox.DropDownSizeMode = CustomComboBox.SizeMode.UseInitialControlSize;
			_crudComboBox.DropSize = new System.Drawing.Size(121, 106);
			_crudComboBox.Location = new System.Drawing.Point(20, 621);
			_crudComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_crudComboBox.Name = "_crudComboBox";
			_crudComboBox.PlaceHolderText = "Select Entity";
			_crudComboBox.Size = new System.Drawing.Size(146, 23);
			_crudComboBox.TabIndex = 5;
			_crudComboBox.EntitySelectionChanged += _crudComboBox_EntitySelectionChanged;
			// 
			// _flagsCheckedListBox
			// 
			_flagsCheckedListBox.BackColor = System.Drawing.SystemColors.Control;
			_flagsCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			_flagsCheckedListBox.CheckOnClick = true;
			_flagsCheckedListBox.FormattingEnabled = true;
			_flagsCheckedListBox.Location = new System.Drawing.Point(14, 14);
			_flagsCheckedListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_flagsCheckedListBox.Name = "_flagsCheckedListBox";
			_flagsCheckedListBox.Size = new System.Drawing.Size(194, 218);
			_flagsCheckedListBox.TabIndex = 1;
			_flagsCheckedListBox.SelectedValueChanged += _flagsCheckedListBox_SelectedValueChanged;
			// 
			// _crudGrid
			// 
			_crudGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_crudGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			_crudGrid.Capabilities = DataSourceCapabilities.CanCreate | DataSourceCapabilities.CanRead;
			_crudGrid.Location = new System.Drawing.Point(217, 15);
			_crudGrid.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			_crudGrid.MinimumSize = new System.Drawing.Size(372, 100);
			_crudGrid.Name = "_crudGrid";
			_crudGrid.RightClickForContextMenu = false;
			_crudGrid.Size = new System.Drawing.Size(876, 632);
			_crudGrid.TabIndex = 0;
			_crudGrid.EntitySelected += _crudGrid_EntitySelected;
			_crudGrid.EntityDeselected += _crudGrid_EntityDeselected;
			_crudGrid.EntityCreated += _crudGrid_EntityCreated;
			_crudGrid.EntityUpdated += _crudGrid_EntityUpdated;
			_crudGrid.EntityDeleted += _crudGrid_EntityDeleted;
			// 
			// _allowCellEditingCheckBox
			// 
			_allowCellEditingCheckBox.AutoSize = true;
			_allowCellEditingCheckBox.Location = new System.Drawing.Point(19, 432);
			_allowCellEditingCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_allowCellEditingCheckBox.Name = "_allowCellEditingCheckBox";
			_allowCellEditingCheckBox.Size = new System.Drawing.Size(117, 19);
			_allowCellEditingCheckBox.TabIndex = 14;
			_allowCellEditingCheckBox.Text = "Allow cell editing";
			_allowCellEditingCheckBox.UseVisualStyleBackColor = true;
			_allowCellEditingCheckBox.CheckedChanged += _allowCellEditingCheckBox_CheckedChanged;
			// 
			// _refreshEntireGridOnUpdateCheckBox
			// 
			_refreshEntireGridOnUpdateCheckBox.AutoSize = true;
			_refreshEntireGridOnUpdateCheckBox.Location = new System.Drawing.Point(19, 458);
			_refreshEntireGridOnUpdateCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_refreshEntireGridOnUpdateCheckBox.Name = "_refreshEntireGridOnUpdateCheckBox";
			_refreshEntireGridOnUpdateCheckBox.Size = new System.Drawing.Size(179, 19);
			_refreshEntireGridOnUpdateCheckBox.TabIndex = 15;
			_refreshEntireGridOnUpdateCheckBox.Text = "Refresh entire grid on update";
			_refreshEntireGridOnUpdateCheckBox.UseVisualStyleBackColor = true;
			_refreshEntireGridOnUpdateCheckBox.CheckedChanged += _refreshEntireGridOnUpdateCheckBox_CheckedChanged;
			// 
			// _refreshEntireGridOnDeleteCheckBox
			// 
			_refreshEntireGridOnDeleteCheckBox.AutoSize = true;
			_refreshEntireGridOnDeleteCheckBox.Location = new System.Drawing.Point(19, 485);
			_refreshEntireGridOnDeleteCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_refreshEntireGridOnDeleteCheckBox.Name = "_refreshEntireGridOnDeleteCheckBox";
			_refreshEntireGridOnDeleteCheckBox.Size = new System.Drawing.Size(174, 19);
			_refreshEntireGridOnDeleteCheckBox.TabIndex = 16;
			_refreshEntireGridOnDeleteCheckBox.Text = "Refresh entire grid on delete";
			_refreshEntireGridOnDeleteCheckBox.UseVisualStyleBackColor = true;
			_refreshEntireGridOnDeleteCheckBox.CheckedChanged += _refreshEntireGridOnDeleteCheckBox_CheckedChanged;
			// 
			// CrudTestScreen
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(_refreshEntireGridOnDeleteCheckBox);
			Controls.Add(_refreshEntireGridOnUpdateCheckBox);
			Controls.Add(_allowCellEditingCheckBox);
			Controls.Add(_autoSelectOnCreateCheckBox);
			Controls.Add(_autoSizeCheckBox);
			Controls.Add(_refreshGrid);
			Controls.Add(_rightClickCheckBox);
			Controls.Add(_leftClickCheckBox);
			Controls.Add(_selectFirstEntityButton);
			Controls.Add(_generateUpdateErrorCheckBox);
			Controls.Add(_generateCreateErrorCheckBox);
			Controls.Add(_crudComboBox);
			Controls.Add(_crudDialogButton);
			Controls.Add(_generateDeleteErrorCheckBox);
			Controls.Add(_outputTextBox);
			Controls.Add(_flagsCheckedListBox);
			Controls.Add(_crudGrid);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "CrudTestScreen";
			Size = new System.Drawing.Size(1106, 762);
			ResumeLayout(false);
			PerformLayout();
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
