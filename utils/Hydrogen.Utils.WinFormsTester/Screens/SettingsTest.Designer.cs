//-----------------------------------------------------------------------
// <copyright file="SettingsTest.Designer.cs" company="Sphere 10 Software">
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
    partial class SettingsTest {
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
            this._test1Button = new System.Windows.Forms.Button();
            this._outputTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _test1Button
            // 
            this._test1Button.Location = new System.Drawing.Point(12, 12);
            this._test1Button.Name = "_test1Button";
            this._test1Button.Size = new System.Drawing.Size(75, 23);
            this._test1Button.TabIndex = 0;
            this._test1Button.Text = "Test 1";
            this._test1Button.UseVisualStyleBackColor = true;
            this._test1Button.Click += new System.EventHandler(this._test1Button_Click);
            // 
            // _outputTextBox
            // 
            this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._outputTextBox.Location = new System.Drawing.Point(12, 41);
            this._outputTextBox.Multiline = true;
            this._outputTextBox.Name = "_outputTextBox";
            this._outputTextBox.Size = new System.Drawing.Size(572, 278);
            this._outputTextBox.TabIndex = 1;
            // 
            // SettingsTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 331);
            this.Controls.Add(this._outputTextBox);
            this.Controls.Add(this._test1Button);
            this.Name = "SettingsTest";
            this.Text = "SettingsTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _test1Button;
        private System.Windows.Forms.TextBox _outputTextBox;
    }
}
