//-----------------------------------------------------------------------
// <copyright file="AppointmentBookForm.Designer.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Utils.WinFormsTester {
	partial class AppointmentBookScreen {
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
			this._populateButton = new System.Windows.Forms.Button();
			this._outputTextBox = new System.Windows.Forms.TextBox();
			this._columnFilterComboBox = new Hydrogen.Windows.Forms.EnumComboBox();
			this.UnallocatedBook = new Hydrogen.Windows.Forms.AppointmentBook.UnallocatedBook();
			this.AppointmentBook = new Hydrogen.Windows.Forms.AppointmentBook.AppointmentBook();
			this._timeViewSelector = new Hydrogen.Windows.Forms.EnumComboBox();
			this.SuspendLayout();
			// 
			// _populateButton
			// 
			this._populateButton.Location = new System.Drawing.Point(12, 12);
			this._populateButton.Name = "_populateButton";
			this._populateButton.Size = new System.Drawing.Size(120, 23);
			this._populateButton.TabIndex = 1;
			this._populateButton.Text = "Generate Data";
			this._populateButton.UseVisualStyleBackColor = true;
			this._populateButton.Click += new System.EventHandler(this._populateButton_Click);
			// 
			// _outputTextBox
			// 
			this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputTextBox.Location = new System.Drawing.Point(12, 562);
			this._outputTextBox.Multiline = true;
			this._outputTextBox.Name = "_outputTextBox";
			this._outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this._outputTextBox.Size = new System.Drawing.Size(923, 130);
			this._outputTextBox.TabIndex = 5;
			// 
			// _columnFilterComboBox
			// 
			this._columnFilterComboBox.AllowEmptyOption = false;
			this._columnFilterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._columnFilterComboBox.DisplayMember = "Display";
			this._columnFilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._columnFilterComboBox.EmptyOptionText = "";
			this._columnFilterComboBox.FormattingEnabled = true;
			this._columnFilterComboBox.Location = new System.Drawing.Point(687, 14);
			this._columnFilterComboBox.Name = "_columnFilterComboBox";
			this._columnFilterComboBox.Size = new System.Drawing.Size(121, 21);
			this._columnFilterComboBox.TabIndex = 6;
			this._columnFilterComboBox.ValueMember = "Value";
			this._columnFilterComboBox.SelectedIndexChanged += new System.EventHandler(this._columnFilterComboBox_SelectedIndexChanged);
			// 
			// UnallocatedBook
			// 
			this.UnallocatedBook.AllowDrop = true;
			this.UnallocatedBook.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.UnallocatedBook.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.UnallocatedBook.CanResize = false;
			this.UnallocatedBook.ColumnFilter = Hydrogen.Windows.Forms.AppointmentBook.AppointmentBookViewModelFilter.All;
			this.UnallocatedBook.DataSource = null;
			this.UnallocatedBook.DistanceMovedToInitiateDragging = 12F;
			this.UnallocatedBook.HasColHeaders = true;
			this.UnallocatedBook.HasRowHeaders = false;
			this.UnallocatedBook.Location = new System.Drawing.Point(12, 41);
			this.UnallocatedBook.MaxColumnWidth = 400;
			this.UnallocatedBook.MinColumnWidth = 100;
			this.UnallocatedBook.Name = "UnallocatedBook";
			this.UnallocatedBook.RowHeaderColumnWidth = 32;
			this.UnallocatedBook.Size = new System.Drawing.Size(120, 515);
			this.UnallocatedBook.TabIndex = 4;
			this.UnallocatedBook.ViewModel = null;
			this.UnallocatedBook.AppointmentSelected += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentEvent>(this._unallocatedBook_AppointmentSelected);
			this.UnallocatedBook.AppointmentDoubleClicked += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentEvent>(this.UnallocatedBook_AppointmentDoubleClicked);
			this.UnallocatedBook.AppointmentDeselected += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentEvent>(this._unallocatedBook_AppointmentDeselected);
			this.UnallocatedBook.AppointmentDragStarting += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDragStartingEvent>(this._unallocatedBook_AppointmentDragStarting);
			this.UnallocatedBook.AppointmentDragging += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDraggingEvent>(this._unallocatedBook_AppointmentDragging);
			this.UnallocatedBook.AppointmentDragged += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDraggedEvent>(this._unallocatedBook_AppointmentDrag);
			this.UnallocatedBook.AppointmentDropStarting += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDropStartingEvent>(this._unallocatedBook_AppointmentDropStarting);
			this.UnallocatedBook.AppointmentDrop += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDropEvent>(this._unallocatedBook_AppointmentDrop);
			// 
			// AppointmentBook
			// 
			this.AppointmentBook.AllowDrop = true;
			this.AppointmentBook.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.AppointmentBook.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.AppointmentBook.CanResize = true;
			this.AppointmentBook.ColumnFilter = Hydrogen.Windows.Forms.AppointmentBook.AppointmentBookViewModelFilter.All;
			this.AppointmentBook.DataSource = null;
			this.AppointmentBook.DistanceMovedToInitiateDragging = 12F;
			this.AppointmentBook.HasColHeaders = true;
			this.AppointmentBook.HasRowHeaders = true;
			this.AppointmentBook.Location = new System.Drawing.Point(138, 41);
			this.AppointmentBook.MaxColumnWidth = 400;
			this.AppointmentBook.MinColumnWidth = 100;
			this.AppointmentBook.Name = "AppointmentBook";
			this.AppointmentBook.RowHeaderColumnWidth = 32;
			this.AppointmentBook.Size = new System.Drawing.Size(797, 515);
			this.AppointmentBook.TabIndex = 3;
			this.AppointmentBook.ViewModel = null;
			this.AppointmentBook.AppointmentBookFreeRegionSelected += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentBookFreeRegionSelected>(this._appointmentBook_AppointmentBookFreeRegionSelected);
			this.AppointmentBook.AppointmentSelected += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentEvent>(this._appointmentBook_AppointmentSelected);
			this.AppointmentBook.AppointmentDoubleClicked += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentEvent>(this.AppointmentBook_AppointmentDoubleClicked);
			this.AppointmentBook.AppointmentDeselected += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentEvent>(this._appointmentBook_AppointmentDeselected);
			this.AppointmentBook.AppointmentResizingStarted += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentEvent>(this._appointmentBook_AppointmentResizingStarted);
			this.AppointmentBook.AppointmentResizing += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentResizingEvent>(this._appointmentBook_AppointmentResizing);
			this.AppointmentBook.AppointmentResizingFinished += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentResizingFinishedEvent>(this._appointmentBook_AppointmentResizingFinished);
			this.AppointmentBook.AppointmentDragStarting += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDragStartingEvent>(this._appointmentBook_AppointmentDragStarting);
			this.AppointmentBook.AppointmentDragging += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDraggingEvent>(this._appointmentBook_AppointmentDragging);
			this.AppointmentBook.AppointmentDragged += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDraggedEvent>(this._appointmentBook_AppointmentDrag);
			this.AppointmentBook.AppointmentDropStarting += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDropStartingEvent>(this._appointmentBook_AppointmentDropStarting);
			this.AppointmentBook.AppointmentDrop += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.AppointmentBook.AppointmentDropEvent>(this._appointmentBook_AppointmentDrop);
			// 
			// _timeViewSelector
			// 
			this._timeViewSelector.AllowEmptyOption = false;
			this._timeViewSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._timeViewSelector.DisplayMember = "Display";
			this._timeViewSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._timeViewSelector.EmptyOptionText = "";
			this._timeViewSelector.FormattingEnabled = true;
			this._timeViewSelector.Location = new System.Drawing.Point(814, 14);
			this._timeViewSelector.Name = "_timeViewSelector";
			this._timeViewSelector.Size = new System.Drawing.Size(121, 21);
			this._timeViewSelector.TabIndex = 2;
			this._timeViewSelector.ValueMember = "Value";
			this._timeViewSelector.SelectedIndexChanged += new System.EventHandler(this._timeViewSelector_SelectedIndexChanged);
			// 
			// AppointmentBookForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(947, 704);
			this.Controls.Add(this._columnFilterComboBox);
			this.Controls.Add(this._outputTextBox);
			this.Controls.Add(this.UnallocatedBook);
			this.Controls.Add(this.AppointmentBook);
			this.Controls.Add(this._timeViewSelector);
			this.Controls.Add(this._populateButton);
			this.Name = "AppointmentBookScreen";
			this.Text = "ResourceCalendarForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _populateButton;
		private Hydrogen.Windows.Forms.EnumComboBox _timeViewSelector;
		private Hydrogen.Windows.Forms.AppointmentBook.AppointmentBook AppointmentBook;
		private Hydrogen.Windows.Forms.AppointmentBook.UnallocatedBook UnallocatedBook;
		private System.Windows.Forms.TextBox _outputTextBox;
		private Hydrogen.Windows.Forms.EnumComboBox _columnFilterComboBox;


	}
}
