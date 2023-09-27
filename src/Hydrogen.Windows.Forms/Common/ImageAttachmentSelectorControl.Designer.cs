// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
    partial class ImageAttachmentSelectorControl {
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
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this._fileSelectorControl = new Hydrogen.Windows.Forms.PathSelectorControl();
            this._noneRadioButton = new System.Windows.Forms.RadioButton();
            this._clipboardRadioButton = new System.Windows.Forms.RadioButton();
            this._filenameRadioButton = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _pictureBox
            // 
            this._pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._pictureBox.Location = new System.Drawing.Point(348, 0);
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.Size = new System.Drawing.Size(74, 66);
            this._pictureBox.TabIndex = 11;
            this._pictureBox.TabStop = false;
            // 
            // _fileSelectorControl
            // 
            this._fileSelectorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._fileSelectorControl.Location = new System.Drawing.Point(47, 0);
            this._fileSelectorControl.Name = "_fileSelectorControl";
            this._fileSelectorControl.Size = new System.Drawing.Size(295, 20);
            this._fileSelectorControl.TabIndex = 10;
            // 
            // _noneRadioButton
            // 
            this._noneRadioButton.AutoSize = true;
            this._noneRadioButton.Checked = true;
            this._noneRadioButton.Location = new System.Drawing.Point(0, 45);
            this._noneRadioButton.Name = "_noneRadioButton";
            this._noneRadioButton.Size = new System.Drawing.Size(51, 17);
            this._noneRadioButton.TabIndex = 9;
            this._noneRadioButton.TabStop = true;
            this._noneRadioButton.Text = "None";
            this._noneRadioButton.UseVisualStyleBackColor = true;
            this._noneRadioButton.CheckedChanged += new System.EventHandler(this._noneRadioButton_CheckedChanged);
            // 
            // _clipboardRadioButton
            // 
            this._clipboardRadioButton.AutoSize = true;
            this._clipboardRadioButton.Location = new System.Drawing.Point(0, 24);
            this._clipboardRadioButton.Name = "_clipboardRadioButton";
            this._clipboardRadioButton.Size = new System.Drawing.Size(127, 17);
            this._clipboardRadioButton.TabIndex = 8;
            this._clipboardRadioButton.TabStop = true;
            this._clipboardRadioButton.Text = "Extract from clipboard";
            this._clipboardRadioButton.UseVisualStyleBackColor = true;
            this._clipboardRadioButton.CheckedChanged += new System.EventHandler(this._clipboardRadioButton_CheckedChanged);
            // 
            // _filenameRadioButton
            // 
            this._filenameRadioButton.AutoSize = true;
            this._filenameRadioButton.Location = new System.Drawing.Point(0, 0);
            this._filenameRadioButton.Name = "_filenameRadioButton";
            this._filenameRadioButton.Size = new System.Drawing.Size(41, 17);
            this._filenameRadioButton.TabIndex = 7;
            this._filenameRadioButton.TabStop = true;
            this._filenameRadioButton.Text = "File";
            this._filenameRadioButton.UseVisualStyleBackColor = true;
            this._filenameRadioButton.CheckedChanged += new System.EventHandler(this._filenameRadioButton_CheckedChanged);
            // 
            // ImageAttachmentSelectorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._pictureBox);
            this.Controls.Add(this._fileSelectorControl);
            this.Controls.Add(this._noneRadioButton);
            this.Controls.Add(this._clipboardRadioButton);
            this.Controls.Add(this._filenameRadioButton);
            this.Name = "ImageAttachmentSelectorControl";
            this.Size = new System.Drawing.Size(425, 66);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _pictureBox;
        private PathSelectorControl _fileSelectorControl;
        private System.Windows.Forms.RadioButton _noneRadioButton;
        private System.Windows.Forms.RadioButton _clipboardRadioButton;
        private System.Windows.Forms.RadioButton _filenameRadioButton;
    }
}
