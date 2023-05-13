//-----------------------------------------------------------------------
// <copyright file="DraggableControlsTestForm.Designer.cs" company="Sphere 10 Software">
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
	partial class DraggableControlsTestScreen {
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
            this._pictureBox1 = new Hydrogen.Windows.Forms.PictureBoxEx();
            this._pictureBox2 = new Hydrogen.Windows.Forms.PictureBoxEx();
            this._pictureBox3 = new Hydrogen.Windows.Forms.PictureBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // _pictureBox1
            // 
            this._pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this._pictureBox1.Image = global::Hydrogen.Utils.WinFormsTester.Resources.LargeSearchIcon;
            this._pictureBox1.Location = new System.Drawing.Point(48, 41);
            this._pictureBox1.Name = "_pictureBox1";
            this._pictureBox1.Size = new System.Drawing.Size(102, 94);
            this._pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pictureBox1.SystemIcon = Hydrogen.Windows.Forms.SystemIconType.None;
            this._pictureBox1.TabIndex = 0;
            this._pictureBox1.TabStop = false;
            this._pictureBox1.Click += new System.EventHandler(this._pictureBox1_Click);
            // 
            // _pictureBox2
            // 
            this._pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this._pictureBox2.Image = global::Hydrogen.Utils.WinFormsTester.Resources.LargeSearchIcon;
            this._pictureBox2.Location = new System.Drawing.Point(275, 139);
            this._pictureBox2.Name = "_pictureBox2";
            this._pictureBox2.Size = new System.Drawing.Size(102, 94);
            this._pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pictureBox2.SystemIcon = Hydrogen.Windows.Forms.SystemIconType.None;
            this._pictureBox2.TabIndex = 1;
            this._pictureBox2.TabStop = false;
            // 
            // _pictureBox3
            // 
            this._pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this._pictureBox3.Image = global::Hydrogen.Utils.WinFormsTester.Resources.LargeSearchIcon;
            this._pictureBox3.Location = new System.Drawing.Point(374, 29);
            this._pictureBox3.Name = "_pictureBox3";
            this._pictureBox3.Size = new System.Drawing.Size(102, 94);
            this._pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pictureBox3.SystemIcon = Hydrogen.Windows.Forms.SystemIconType.None;
            this._pictureBox3.TabIndex = 2;
            this._pictureBox3.TabStop = false;
            // 
            // DraggableControlsTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 373);
            this.Controls.Add(this._pictureBox3);
            this.Controls.Add(this._pictureBox2);
            this.Controls.Add(this._pictureBox1);
            this.Name = "DraggableControlsTestScreen";
            this.Text = "DraggableControlsTestForm";
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox3)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Hydrogen.Windows.Forms.PictureBoxEx _pictureBox1;
		private Hydrogen.Windows.Forms.PictureBoxEx _pictureBox2;
		private Hydrogen.Windows.Forms.PictureBoxEx _pictureBox3;
	}
}
