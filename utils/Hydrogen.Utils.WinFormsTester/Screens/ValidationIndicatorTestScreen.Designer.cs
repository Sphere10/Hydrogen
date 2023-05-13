//-----------------------------------------------------------------------
// <copyright file="ValidationIndicatorTestForm.Designer.cs" company="Sphere 10 Software">
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
	partial class ValidationIndicatorTestScreen {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidationIndicatorTestScreen));
            this._outputTextBox = new System.Windows.Forms.TextBox();
            this._validButton = new System.Windows.Forms.Button();
            this._errorButton = new System.Windows.Forms.Button();
            this._validatingButton = new System.Windows.Forms.Button();
            this._disabledButton = new System.Windows.Forms.Button();
            this._validationIndicator1 = new Hydrogen.Windows.Forms.ValidationIndicator();
            this._validationIndicator2 = new Hydrogen.Windows.Forms.ValidationIndicator();
            this._textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._enableDisableButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _outputTextBox
            // 
            this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._outputTextBox.Location = new System.Drawing.Point(12, 206);
            this._outputTextBox.Multiline = true;
            this._outputTextBox.Name = "_outputTextBox";
            this._outputTextBox.Size = new System.Drawing.Size(848, 151);
            this._outputTextBox.TabIndex = 0;
            // 
            // _validButton
            // 
            this._validButton.Location = new System.Drawing.Point(12, 12);
            this._validButton.Name = "_validButton";
            this._validButton.Size = new System.Drawing.Size(75, 23);
            this._validButton.TabIndex = 1;
            this._validButton.Text = "Valid";
            this._validButton.UseVisualStyleBackColor = true;
            this._validButton.Click += new System.EventHandler(this._validButton_Click);
            // 
            // _errorButton
            // 
            this._errorButton.Location = new System.Drawing.Point(12, 41);
            this._errorButton.Name = "_errorButton";
            this._errorButton.Size = new System.Drawing.Size(75, 23);
            this._errorButton.TabIndex = 2;
            this._errorButton.Text = "Error";
            this._errorButton.UseVisualStyleBackColor = true;
            this._errorButton.Click += new System.EventHandler(this._errorButton_Click);
            // 
            // _validatingButton
            // 
            this._validatingButton.Location = new System.Drawing.Point(12, 70);
            this._validatingButton.Name = "_validatingButton";
            this._validatingButton.Size = new System.Drawing.Size(75, 23);
            this._validatingButton.TabIndex = 3;
            this._validatingButton.Text = "Validating";
            this._validatingButton.UseVisualStyleBackColor = true;
            this._validatingButton.Click += new System.EventHandler(this._validatingButton_Click);
            // 
            // _disabledButton
            // 
            this._disabledButton.Location = new System.Drawing.Point(12, 99);
            this._disabledButton.Name = "_disabledButton";
            this._disabledButton.Size = new System.Drawing.Size(75, 23);
            this._disabledButton.TabIndex = 4;
            this._disabledButton.Text = "Disabled";
            this._disabledButton.UseVisualStyleBackColor = true;
            this._disabledButton.Click += new System.EventHandler(this._disabledButton_Click);
            // 
            // _validationIndicator1
            // 
            this._validationIndicator1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_validationIndicator1.BackgroundImage")));
            this._validationIndicator1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._validationIndicator1.Location = new System.Drawing.Point(90, 15);
            this._validationIndicator1.Margin = new System.Windows.Forms.Padding(0);
            this._validationIndicator1.Name = "_validationIndicator1";
            this._validationIndicator1.Size = new System.Drawing.Size(20, 20);
            this._validationIndicator1.TabIndex = 5;
            this._validationIndicator1.ValidationStateChanged += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.ValidationIndicator, Hydrogen.Windows.Forms.ValidationState, Hydrogen.Windows.Forms.ValidationState>(this._validationIndicator1_ValidationStateChanged);
            // 
            // _validationIndicator2
            // 
            this._validationIndicator2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_validationIndicator2.BackgroundImage")));
            this._validationIndicator2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._validationIndicator2.Location = new System.Drawing.Point(430, 86);
            this._validationIndicator2.Margin = new System.Windows.Forms.Padding(0);
            this._validationIndicator2.Name = "_validationIndicator2";
            this._validationIndicator2.RunValidatorWhenEnabled = true;
            this._validationIndicator2.Size = new System.Drawing.Size(20, 20);
            this._validationIndicator2.TabIndex = 6;
            this._validationIndicator2.PerformValidation += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.ValidationIndicator, Hydrogen.Windows.Forms.ValidationIndicatorEvent>(this._validationIndicator2_PerformValidation);
            this._validationIndicator2.ValidationStateChanged += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.ValidationIndicator, Hydrogen.Windows.Forms.ValidationState, Hydrogen.Windows.Forms.ValidationState>(this._validationIndicator2_ValidationStateChanged);
            // 
            // _textBox
            // 
            this._textBox.Location = new System.Drawing.Point(298, 86);
            this._textBox.Name = "_textBox";
            this._textBox.Size = new System.Drawing.Size(129, 20);
            this._textBox.TabIndex = 7;
            this._textBox.TextChanged += new System.EventHandler(this._textBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(295, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Type \'Herman\' for success";
            // 
            // _enableDisableButton
            // 
            this._enableDisableButton.Location = new System.Drawing.Point(298, 112);
            this._enableDisableButton.Name = "_enableDisableButton";
            this._enableDisableButton.Size = new System.Drawing.Size(129, 23);
            this._enableDisableButton.TabIndex = 9;
            this._enableDisableButton.Text = "Enable/Disable";
            this._enableDisableButton.UseVisualStyleBackColor = true;
            this._enableDisableButton.Click += new System.EventHandler(this._enableDisableButton_Click);
            // 
            // ValidationIndicatorTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 369);
            this.Controls.Add(this._enableDisableButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._textBox);
            this.Controls.Add(this._validationIndicator2);
            this.Controls.Add(this._validationIndicator1);
            this.Controls.Add(this._disabledButton);
            this.Controls.Add(this._validatingButton);
            this.Controls.Add(this._errorButton);
            this.Controls.Add(this._validButton);
            this.Controls.Add(this._outputTextBox);
            this.Name = "ValidationIndicatorTestScreen";
            this.Text = "ValidationIndicatorTestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _outputTextBox;
		private System.Windows.Forms.Button _validButton;
		private System.Windows.Forms.Button _errorButton;
		private System.Windows.Forms.Button _validatingButton;
		private System.Windows.Forms.Button _disabledButton;
		private Hydrogen.Windows.Forms.ValidationIndicator _validationIndicator1;
        private Hydrogen.Windows.Forms.ValidationIndicator _validationIndicator2;
		private System.Windows.Forms.TextBox _textBox;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _enableDisableButton;
	}
}
