//-----------------------------------------------------------------------
// <copyright file="CBACSVConverterForm.Designer.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Utils.WinFormsTester
{
    partial class CBACSVConverterScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._CBACSVTextBox = new System.Windows.Forms.TextBox();
            this._outputTextBox = new System.Windows.Forms.TextBox();
            this._convertButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._CBACSVTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._outputTextBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._convertButton, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1209, 547);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _CBACSVTextBox
            // 
            this._CBACSVTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._CBACSVTextBox.Location = new System.Drawing.Point(3, 3);
            this._CBACSVTextBox.Multiline = true;
            this._CBACSVTextBox.Name = "_CBACSVTextBox";
            this._CBACSVTextBox.Size = new System.Drawing.Size(1203, 251);
            this._CBACSVTextBox.TabIndex = 0;
            // 
            // _outputTextBox
            // 
            this._outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._outputTextBox.Location = new System.Drawing.Point(3, 292);
            this._outputTextBox.Multiline = true;
            this._outputTextBox.Name = "_outputTextBox";
            this._outputTextBox.Size = new System.Drawing.Size(1203, 252);
            this._outputTextBox.TabIndex = 1;
            // 
            // _convertButton
            // 
            this._convertButton.Location = new System.Drawing.Point(3, 260);
            this._convertButton.Name = "_convertButton";
            this._convertButton.Size = new System.Drawing.Size(75, 23);
            this._convertButton.TabIndex = 2;
            this._convertButton.Text = "Convert";
            this._convertButton.UseVisualStyleBackColor = true;
            this._convertButton.Click += new System.EventHandler(this._convertButton_Click);
            // 
            // CBACSVConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 547);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CBACSVConverterScreen";
            this.Text = "CBACSVConverterForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox _CBACSVTextBox;
        private System.Windows.Forms.TextBox _outputTextBox;
        private System.Windows.Forms.Button _convertButton;
    }
}
