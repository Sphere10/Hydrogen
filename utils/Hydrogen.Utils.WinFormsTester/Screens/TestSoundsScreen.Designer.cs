//-----------------------------------------------------------------------
// <copyright file="TestSoundsForm.Designer.cs" company="Sphere 10 Software">
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
	partial class TestSoundsScreen {
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
			button1 = new System.Windows.Forms.Button();
			button2 = new System.Windows.Forms.Button();
			button3 = new System.Windows.Forms.Button();
			button4 = new System.Windows.Forms.Button();
			_systemPlayerButton = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(14, 93);
			button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(88, 27);
			button1.TabIndex = 0;
			button1.Text = "button1";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// button2
			// 
			button2.Location = new System.Drawing.Point(108, 93);
			button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(88, 27);
			button2.TabIndex = 1;
			button2.Text = "button2";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// button3
			// 
			button3.Location = new System.Drawing.Point(203, 93);
			button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(88, 27);
			button3.TabIndex = 2;
			button3.Text = "button3";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// button4
			// 
			button4.Location = new System.Drawing.Point(298, 93);
			button4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			button4.Name = "button4";
			button4.Size = new System.Drawing.Size(88, 27);
			button4.TabIndex = 3;
			button4.Text = "button4";
			button4.UseVisualStyleBackColor = true;
			button4.Click += button4_Click;
			// 
			// _systemPlayerButton
			// 
			_systemPlayerButton.Location = new System.Drawing.Point(14, 161);
			_systemPlayerButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_systemPlayerButton.Name = "_systemPlayerButton";
			_systemPlayerButton.Size = new System.Drawing.Size(88, 27);
			_systemPlayerButton.TabIndex = 4;
			_systemPlayerButton.Text = "system player";
			_systemPlayerButton.UseVisualStyleBackColor = true;
			_systemPlayerButton.Click += _systemPlayerButton_Click;
			// 
			// TestSoundsScreen
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(_systemPlayerButton);
			Controls.Add(button4);
			Controls.Add(button3);
			Controls.Add(button2);
			Controls.Add(button1);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "TestSoundsScreen";
			Size = new System.Drawing.Size(656, 368);
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button _systemPlayerButton;
	}
}
