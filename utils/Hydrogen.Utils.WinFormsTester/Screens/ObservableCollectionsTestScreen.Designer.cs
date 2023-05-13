//-----------------------------------------------------------------------
// <copyright file="ObservableCollectionsTest.Designer.cs" company="Sphere 10 Software">
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
    partial class ObservableCollectionsTestScreen {
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
            this.textBoxEx1 = new Hydrogen.Windows.Forms.TextBoxEx();
            this._dictionaryTestButton = new System.Windows.Forms.Button();
            this._listTestButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxEx1
            // 
            this.textBoxEx1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEx1.Location = new System.Drawing.Point(12, 41);
            this.textBoxEx1.Multiline = true;
            this.textBoxEx1.Name = "textBoxEx1";
            this.textBoxEx1.Size = new System.Drawing.Size(751, 302);
            this.textBoxEx1.TabIndex = 0;
            // 
            // _dictionaryTestButton
            // 
            this._dictionaryTestButton.Location = new System.Drawing.Point(12, 12);
            this._dictionaryTestButton.Name = "_dictionaryTestButton";
            this._dictionaryTestButton.Size = new System.Drawing.Size(121, 23);
            this._dictionaryTestButton.TabIndex = 3;
            this._dictionaryTestButton.Text = "Dictionary Test";
            this._dictionaryTestButton.UseVisualStyleBackColor = true;
            this._dictionaryTestButton.Click += new System.EventHandler(this._dictionaryTestButton_Click);
            // 
            // _listTestButton
            // 
            this._listTestButton.Location = new System.Drawing.Point(139, 12);
            this._listTestButton.Name = "_listTestButton";
            this._listTestButton.Size = new System.Drawing.Size(121, 23);
            this._listTestButton.TabIndex = 4;
            this._listTestButton.Text = "List Test";
            this._listTestButton.UseVisualStyleBackColor = true;
            this._listTestButton.Click += new System.EventHandler(this._listTestButton_Click);
            // 
            // ObservableCollectionsTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 355);
            this.Controls.Add(this._listTestButton);
            this.Controls.Add(this._dictionaryTestButton);
            this.Controls.Add(this.textBoxEx1);
            this.Name = "ObservableCollectionsTestScreen";
            this.Text = "ObservableCollectionsTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBoxEx textBoxEx1;
        private System.Windows.Forms.Button _dictionaryTestButton;
        private System.Windows.Forms.Button _listTestButton;
    }
}
