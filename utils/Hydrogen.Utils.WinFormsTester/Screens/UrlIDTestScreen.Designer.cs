//-----------------------------------------------------------------------
// <copyright file="UrlIDTestForm.Designer.cs" company="Sphere 10 Software">
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
    partial class UrlIDTestScreen {
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
            this._generateButton = new System.Windows.Forms.Button();
            this._textBoxEx = new Hydrogen.Windows.Forms.TextBoxEx();
            this.SuspendLayout();
            // 
            // _generateButton
            // 
            this._generateButton.Location = new System.Drawing.Point(12, 12);
            this._generateButton.Name = "_generateButton";
            this._generateButton.Size = new System.Drawing.Size(75, 23);
            this._generateButton.TabIndex = 0;
            this._generateButton.Text = "Generate";
            this._generateButton.UseVisualStyleBackColor = true;
            this._generateButton.Click += new System.EventHandler(this._generateButton_Click);
            // 
            // _textBoxEx
            // 
            this._textBoxEx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxEx.Location = new System.Drawing.Point(12, 41);
            this._textBoxEx.Multiline = true;
            this._textBoxEx.Name = "_textBoxEx";
            this._textBoxEx.Size = new System.Drawing.Size(652, 297);
            this._textBoxEx.TabIndex = 1;
            // 
            // CodeIDGenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 350);
            this.Controls.Add(this._textBoxEx);
            this.Controls.Add(this._generateButton);
            this.Name = "UrlIDTestScreen";
            this.Text = "CodeIDGenForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _generateButton;
        private Hydrogen.Windows.Forms.TextBoxEx _textBoxEx;
    }
}
