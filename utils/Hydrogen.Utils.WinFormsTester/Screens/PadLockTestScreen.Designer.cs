//-----------------------------------------------------------------------
// <copyright file="PadLockTestForm.Designer.cs" company="Sphere 10 Software">
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
	partial class PadLockTestScreen {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PadLockTestScreen));
			this._outputTextBox = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.padLockButton2 = new Hydrogen.Windows.Forms.PadLockButton();
			this.padLockButton1 = new Hydrogen.Windows.Forms.PadLockButton();
			this.SuspendLayout();
			// 
			// _outputTextBox
			// 
			this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputTextBox.Location = new System.Drawing.Point(12, 128);
			this._outputTextBox.Multiline = true;
			this._outputTextBox.Name = "_outputTextBox";
			this._outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._outputTextBox.Size = new System.Drawing.Size(564, 137);
			this._outputTextBox.TabIndex = 4;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(98, 12);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(100, 20);
			this.textBox2.TabIndex = 5;
			// 
			// padLockButton2
			// 
			this.padLockButton2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("padLockButton2.BackgroundImage")));
			this.padLockButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.padLockButton2.Enabled = false;
			this.padLockButton2.Location = new System.Drawing.Point(309, 38);
			this.padLockButton2.MaximumSize = new System.Drawing.Size(20, 20);
			this.padLockButton2.MinimumSize = new System.Drawing.Size(20, 20);
			this.padLockButton2.Name = "padLockButton2";
			this.padLockButton2.Size = new System.Drawing.Size(20, 20);
			this.padLockButton2.TabIndex = 7;
			this.padLockButton2.UseVisualStyleBackColor = true;
			// 
			// padLockButton1
			// 
			this.padLockButton1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("padLockButton1.BackgroundImage")));
			this.padLockButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.padLockButton1.Location = new System.Drawing.Point(204, 12);
			this.padLockButton1.MaximumSize = new System.Drawing.Size(20, 20);
			this.padLockButton1.MinimumSize = new System.Drawing.Size(20, 20);
			this.padLockButton1.Name = "padLockButton1";
			this.padLockButton1.Size = new System.Drawing.Size(20, 20);
			this.padLockButton1.TabIndex = 6;
			this.padLockButton1.UseVisualStyleBackColor = true;
			this.padLockButton1.PadLockStateChanged += new Hydrogen.EventHandlerEx<Hydrogen.Windows.Forms.PadLockButton, Hydrogen.Windows.Forms.PadLockButton.PadLockState>(this.padLockButton1_PadLockStateChanged);
			// 
			// PadLockTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(588, 277);
			this.Controls.Add(this.padLockButton2);
			this.Controls.Add(this.padLockButton1);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this._outputTextBox);
			this.Name = "PadLockTestScreen";
			this.Text = "PadLockTestForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _outputTextBox;
		private System.Windows.Forms.TextBox textBox2;
		private Hydrogen.Windows.Forms.PadLockButton padLockButton1;
		private Hydrogen.Windows.Forms.PadLockButton padLockButton2;


	}
}
