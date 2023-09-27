// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class PasswordDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordDialog));
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._textLabel = new System.Windows.Forms.Label();
            this._hidePasswordCheckBox = new System.Windows.Forms.CheckBox();
            this._repeatTextBox = new System.Windows.Forms.TextBox();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._errorRichTextBox = new System.Windows.Forms.RichTextBox();
            this.pictureBoxEx1 = new Hydrogen.Windows.Forms.PictureBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEx1)).BeginInit();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(168, 128);
            this._okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(88, 27);
            this._okButton.TabIndex = 4;
            this._okButton.Text = "&Ok";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(262, 128);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(88, 27);
            this._cancelButton.TabIndex = 5;
            this._cancelButton.Text = "&Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _textLabel
            // 
            this._textLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textLabel.AutoEllipsis = true;
            this._textLabel.Location = new System.Drawing.Point(59, 16);
            this._textLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._textLabel.Name = "_textLabel";
            this._textLabel.Size = new System.Drawing.Size(303, 37);
            this._textLabel.TabIndex = 1;
            this._textLabel.Text = "_text";
            // 
            // _hidePasswordCheckBox
            // 
            this._hidePasswordCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._hidePasswordCheckBox.AutoSize = true;
            this._hidePasswordCheckBox.Checked = true;
            this._hidePasswordCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._hidePasswordCheckBox.Location = new System.Drawing.Point(15, 133);
            this._hidePasswordCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._hidePasswordCheckBox.Name = "_hidePasswordCheckBox";
            this._hidePasswordCheckBox.Size = new System.Drawing.Size(104, 19);
            this._hidePasswordCheckBox.TabIndex = 3;
            this._hidePasswordCheckBox.Text = "Hide password";
            this._hidePasswordCheckBox.UseVisualStyleBackColor = true;
            this._hidePasswordCheckBox.CheckedChanged += new System.EventHandler(this._showPasswordCheckBox_CheckedChanged);
            // 
            // _repeatTextBox
            // 
            this._repeatTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._repeatTextBox.Location = new System.Drawing.Point(168, 87);
            this._repeatTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._repeatTextBox.Name = "_repeatTextBox";
            this._repeatTextBox.PasswordChar = '*';
            this._repeatTextBox.Size = new System.Drawing.Size(180, 23);
            this._repeatTextBox.TabIndex = 2;
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._passwordTextBox.Location = new System.Drawing.Point(168, 57);
            this._passwordTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.PasswordChar = '*';
            this._passwordTextBox.Size = new System.Drawing.Size(181, 23);
            this._passwordTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Enter Password:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Repeat Password:";
            // 
            // _errorRichTextBox
            // 
            this._errorRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._errorRichTextBox.BackColor = System.Drawing.SystemColors.Control;
            this._errorRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._errorRichTextBox.Enabled = false;
            this._errorRichTextBox.ForeColor = System.Drawing.Color.Red;
            this._errorRichTextBox.Location = new System.Drawing.Point(15, 117);
            this._errorRichTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._errorRichTextBox.Name = "_errorRichTextBox";
            this._errorRichTextBox.ReadOnly = true;
            this._errorRichTextBox.Size = new System.Drawing.Size(336, 0);
            this._errorRichTextBox.TabIndex = 9;
            this._errorRichTextBox.Text = "";
            // 
            // pictureBoxEx1
            // 
            this.pictureBoxEx1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxEx1.Image")));
            this.pictureBoxEx1.Location = new System.Drawing.Point(14, 16);
            this.pictureBoxEx1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBoxEx1.Name = "pictureBoxEx1";
            this.pictureBoxEx1.Size = new System.Drawing.Size(37, 37);
            this.pictureBoxEx1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxEx1.SystemIcon = Hydrogen.Windows.Forms.SystemIconType.Shield;
            this.pictureBoxEx1.TabIndex = 0;
            this.pictureBoxEx1.TabStop = false;
            // 
            // PasswordDialog
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(364, 164);
            this.ControlBox = false;
            this.Controls.Add(this._errorRichTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._passwordTextBox);
            this.Controls.Add(this._repeatTextBox);
            this.Controls.Add(this._hidePasswordCheckBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._textLabel);
            this.Controls.Add(this.pictureBoxEx1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximumSize = new System.Drawing.Size(645, 1032);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(347, 145);
            this.Name = "PasswordDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "_dialogTitle";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEx1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		protected PictureBoxEx pictureBoxEx1;
		protected System.Windows.Forms.Button _okButton;
		protected System.Windows.Forms.Button _cancelButton;
		protected System.Windows.Forms.Label _textLabel;
		private System.Windows.Forms.CheckBox _hidePasswordCheckBox;
		private System.Windows.Forms.TextBox _repeatTextBox;
		private System.Windows.Forms.TextBox _passwordTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RichTextBox _errorRichTextBox;



	}
}
