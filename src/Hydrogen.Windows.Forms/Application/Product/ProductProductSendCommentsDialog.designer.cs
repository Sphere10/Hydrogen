// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {

    partial class ProductProductSendCommentsDialog {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._basicContactDetailsControl = new Hydrogen.Windows.Forms.BasicContactDetailsControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._commentTextBox = new System.Windows.Forms.TextBox();
            this._cancelButton = new System.Windows.Forms.Button();
            this._sendButton = new System.Windows.Forms.Button();
			this.applicationBanner1 = new Hydrogen.Windows.Forms.ApplicationBanner();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
			this._whoAreYouControl = new Hydrogen.Windows.Forms.WhoAreYouControl();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this._basicContactDetailsControl);
            this.groupBox1.Location = new System.Drawing.Point(12, 378);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(528, 72);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What are your contact details?";
            // 
            // _basicContactDetailsControl
            // 
            this._basicContactDetailsControl.Location = new System.Drawing.Point(14, 18);
            this._basicContactDetailsControl.Name = "_basicContactDetailsControl";
            this._basicContactDetailsControl.Size = new System.Drawing.Size(508, 48);
            this._basicContactDetailsControl.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this._commentTextBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 102);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(528, 147);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Comments";
            // 
            // _commentTextBox
            // 
            this._commentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._commentTextBox.Location = new System.Drawing.Point(6, 19);
            this._commentTextBox.Multiline = true;
            this._commentTextBox.Name = "_commentTextBox";
            this._commentTextBox.Size = new System.Drawing.Size(516, 122);
            this._commentTextBox.TabIndex = 0;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.Location = new System.Drawing.Point(465, 456);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 10;
            this._cancelButton.Text = "&Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _sendButton
            // 
            this._sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._sendButton.Location = new System.Drawing.Point(384, 456);
            this._sendButton.Name = "_sendButton";
            this._sendButton.Size = new System.Drawing.Size(75, 23);
            this._sendButton.TabIndex = 11;
            this._sendButton.Text = "&Send";
            this._sendButton.UseVisualStyleBackColor = true;
            this._sendButton.Click += new System.EventHandler(this._sendButton_Click);
            // 
            // applicationBanner1
            // 
            this.applicationBanner1.EnableStateChangeEvent = false;
			this.applicationBanner1.Text = "Send Comment";
            this.applicationBanner1.Dock = System.Windows.Forms.DockStyle.Top;
            this.applicationBanner1.Location = new System.Drawing.Point(0, 0);
            this.applicationBanner1.Name = "applicationBanner1";
            this.applicationBanner1.Size = new System.Drawing.Size(552, 96);
            this.applicationBanner1.TabIndex = 12;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this._whoAreYouControl);
            this.groupBox3.Location = new System.Drawing.Point(12, 255);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(528, 117);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "What type of user are you?";
            // 
            // _whoAreYouControl
            // 
            this._whoAreYouControl.EnableStateChangeEvent = false;
            this._whoAreYouControl.Location = new System.Drawing.Point(14, 19);
            this._whoAreYouControl.Name = "_whoAreYouControl";
            this._whoAreYouControl.Size = new System.Drawing.Size(197, 92);
            this._whoAreYouControl.TabIndex = 0;
			this._whoAreYouControl.UserType = Hydrogen.Application.UserType.HomeUser;
            // 
            // SendCommentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 491);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.applicationBanner1);
            this.Controls.Add(this._sendButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimizeBox = false;
            this.Name = "SendCommentsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Send Comments";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox _commentTextBox;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _sendButton;
        private BasicContactDetailsControl _basicContactDetailsControl;
        private ApplicationBanner applicationBanner1;
        private System.Windows.Forms.GroupBox groupBox3;
        private Hydrogen.Windows.Forms.WhoAreYouControl _whoAreYouControl;
    }
}

