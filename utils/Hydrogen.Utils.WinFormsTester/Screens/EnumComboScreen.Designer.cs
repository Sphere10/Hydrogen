//-----------------------------------------------------------------------
// <copyright file="EnumComboForm.Designer.cs" company="Sphere 10 Software">
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
	partial class EnumComboScreen {
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
			this._enumComboBox = new Hydrogen.Windows.Forms.EnumComboBox();
			this._setNullButton = new System.Windows.Forms.Button();
			this._setEnumVal1Button = new System.Windows.Forms.Button();
			this._setEnumValue2Button = new System.Windows.Forms.Button();
			this._getValueButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _enumComboBox
			// 
			this._enumComboBox.AllowEmptyOption = true;
			this._enumComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._enumComboBox.EmptyOptionText = "Empty Option Text";
			this._enumComboBox.EnumType = null;
			this._enumComboBox.FormattingEnabled = true;
			this._enumComboBox.Location = new System.Drawing.Point(12, 12);
			this._enumComboBox.Name = "_enumComboBox";
			this._enumComboBox.SelectedEnum = null;
			this._enumComboBox.Size = new System.Drawing.Size(274, 21);
			this._enumComboBox.TabIndex = 0;
			// 
			// _setNullButton
			// 
			this._setNullButton.Location = new System.Drawing.Point(315, 10);
			this._setNullButton.Name = "_setNullButton";
			this._setNullButton.Size = new System.Drawing.Size(150, 23);
			this._setNullButton.TabIndex = 1;
			this._setNullButton.Text = "Set NULL";
			this._setNullButton.UseVisualStyleBackColor = true;
			this._setNullButton.Click += new System.EventHandler(this._setNullButton_Click);
			// 
			// _setEnumVal1Button
			// 
			this._setEnumVal1Button.Location = new System.Drawing.Point(471, 10);
			this._setEnumVal1Button.Name = "_setEnumVal1Button";
			this._setEnumVal1Button.Size = new System.Drawing.Size(143, 23);
			this._setEnumVal1Button.TabIndex = 2;
			this._setEnumVal1Button.Text = "Set EnumVal 1";
			this._setEnumVal1Button.UseVisualStyleBackColor = true;
			this._setEnumVal1Button.Click += new System.EventHandler(this._setEnumVal1Button_Click);
			// 
			// _setEnumValue2Button
			// 
			this._setEnumValue2Button.Location = new System.Drawing.Point(620, 10);
			this._setEnumValue2Button.Name = "_setEnumValue2Button";
			this._setEnumValue2Button.Size = new System.Drawing.Size(122, 23);
			this._setEnumValue2Button.TabIndex = 3;
			this._setEnumValue2Button.Text = "Set EnumVal 2";
			this._setEnumValue2Button.UseVisualStyleBackColor = true;
			this._setEnumValue2Button.Click += new System.EventHandler(this._setEnumValue2Button_Click);
			// 
			// _getValueButton
			// 
			this._getValueButton.Location = new System.Drawing.Point(315, 39);
			this._getValueButton.Name = "_getValueButton";
			this._getValueButton.Size = new System.Drawing.Size(150, 23);
			this._getValueButton.TabIndex = 4;
			this._getValueButton.Text = "Get Value";
			this._getValueButton.UseVisualStyleBackColor = true;
			this._getValueButton.Click += new System.EventHandler(this._getValueButton_Click);
			// 
			// EnumComboForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(774, 340);
			this.Controls.Add(this._getValueButton);
			this.Controls.Add(this._setEnumValue2Button);
			this.Controls.Add(this._setEnumVal1Button);
			this.Controls.Add(this._setNullButton);
			this.Controls.Add(this._enumComboBox);
			this.Name = "EnumComboForm";
			this.Text = "EnumComboForm";
			this.ResumeLayout(false);

		}

		#endregion

		private Hydrogen.Windows.Forms.EnumComboBox _enumComboBox;
		private System.Windows.Forms.Button _setNullButton;
		private System.Windows.Forms.Button _setEnumVal1Button;
		private System.Windows.Forms.Button _setEnumValue2Button;
		private System.Windows.Forms.Button _getValueButton;
	}
}
