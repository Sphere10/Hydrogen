// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class DialogEx {
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this._textLabel = new System.Windows.Forms.Label();
            this._textLabelPanel = new System.Windows.Forms.Panel();
            this._pictureBoxEx = new Hydrogen.Windows.Forms.PictureBoxEx();
            this._alwaysCheckBox = new System.Windows.Forms.CheckBox();
            this._textLabelPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxEx)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(406, 149);
            this.button1.Margin = new System.Windows.Forms.Padding(9, 12, 9, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 26);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(316, 149);
            this.button2.Margin = new System.Windows.Forms.Padding(9, 12, 9, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 26);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(224, 149);
            this.button3.Margin = new System.Windows.Forms.Padding(9, 12, 9, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 26);
            this.button3.TabIndex = 4;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(132, 149);
            this.button4.Margin = new System.Windows.Forms.Padding(9, 12, 9, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(88, 26);
            this.button4.TabIndex = 5;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // _textLabel
            // 
            this._textLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textLabel.AutoEllipsis = true;
            this._textLabel.Location = new System.Drawing.Point(0, 0);
            this._textLabel.Margin = new System.Windows.Forms.Padding(9, 0, 9, 0);
            this._textLabel.Name = "_textLabel";
            this._textLabel.Size = new System.Drawing.Size(366, 106);
            this._textLabel.TabIndex = 1;
            this._textLabel.Text = "This is a very large message. ";
            // 
            // _textLabelPanel
            // 
            this._textLabelPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._textLabelPanel.AutoScroll = true;
            this._textLabelPanel.Controls.Add(this._textLabel);
            this._textLabelPanel.Location = new System.Drawing.Point(124, 26);
            this._textLabelPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._textLabelPanel.Name = "_textLabelPanel";
            this._textLabelPanel.Size = new System.Drawing.Size(366, 106);
            this._textLabelPanel.TabIndex = 6;
            // 
            // _pictureBoxEx
            // 
            this._pictureBoxEx.Location = new System.Drawing.Point(13, 14);
            this._pictureBoxEx.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._pictureBoxEx.Name = "_pictureBoxEx";
            this._pictureBoxEx.Size = new System.Drawing.Size(81, 70);
            this._pictureBoxEx.SystemIcon = Hydrogen.Windows.Forms.SystemIconType.None;
            this._pictureBoxEx.TabIndex = 7;
            this._pictureBoxEx.TabStop = false;
            // 
            // _alwaysCheckBox
            // 
            this._alwaysCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._alwaysCheckBox.AutoSize = true;
            this._alwaysCheckBox.Location = new System.Drawing.Point(13, 154);
            this._alwaysCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._alwaysCheckBox.Name = "_alwaysCheckBox";
            this._alwaysCheckBox.Size = new System.Drawing.Size(118, 19);
            this._alwaysCheckBox.TabIndex = 8;
            this._alwaysCheckBox.Text = "Apply to all items";
            this._alwaysCheckBox.UseVisualStyleBackColor = true;
            this._alwaysCheckBox.CheckedChanged += new System.EventHandler(this._alwaysCheckBox_CheckedChanged);
            // 
            // DialogEx
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 180);
            this.ControlBox = false;
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._alwaysCheckBox);
            this.Controls.Add(this._pictureBoxEx);
            this.Controls.Add(this._textLabelPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(9, 12, 9, 12);
            this.MaximumSize = new System.Drawing.Size(1497, 3159);
            this.MinimizeBox = false;
            this.Name = "DialogEx";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "_dialogTitle";
            this.TopMost = true;
            this._textLabelPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxEx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		protected System.Windows.Forms.Button button1;
		protected System.Windows.Forms.Button button2;
		protected System.Windows.Forms.Button button3;
		protected System.Windows.Forms.Button button4;
		protected System.Windows.Forms.Label _textLabel;
		protected PictureBoxEx _pictureBoxEx;
		protected System.Windows.Forms.Panel _textLabelPanel;
		private System.Windows.Forms.CheckBox _alwaysCheckBox;
	}
}
