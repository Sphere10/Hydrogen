//-----------------------------------------------------------------------
// <copyright file="HooksForm.Designer.cs" company="Sphere 10 Software">
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
	partial class HooksScreen {
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._wheelEvents = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this._keyUpEventsLabel = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this._keyDownEventsLabel = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this._keyActivityEventsLabel = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this._mouseStartEventsLabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._mouseStopEventsLabel = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this._clickUpEventLabel = new System.Windows.Forms.Label();
			this._clickDownEventsLabel = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._mouseMoveEventsLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._mouseActivityEventsLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Enabled = false;
			this.textBox1.Location = new System.Drawing.Point(12, 25);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(414, 365);
			this.textBox1.TabIndex = 0;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(12, 9);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(40, 13);
			this.label9.TabIndex = 9;
			this.label9.Text = "Events";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this._wheelEvents);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this._keyUpEventsLabel);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this._keyDownEventsLabel);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this._keyActivityEventsLabel);
			this.groupBox1.Controls.Add(this.label14);
			this.groupBox1.Controls.Add(this._mouseStartEventsLabel);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this._mouseStopEventsLabel);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this._clickUpEventLabel);
			this.groupBox1.Controls.Add(this._clickDownEventsLabel);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this._mouseMoveEventsLabel);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this._mouseActivityEventsLabel);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(432, 25);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(324, 365);
			this.groupBox1.TabIndex = 23;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Event Counts";
			// 
			// _wheelEvents
			// 
			this._wheelEvents.AutoSize = true;
			this._wheelEvents.Location = new System.Drawing.Point(233, 313);
			this._wheelEvents.Name = "_wheelEvents";
			this._wheelEvents.Size = new System.Drawing.Size(35, 13);
			this._wheelEvents.TabIndex = 42;
			this._wheelEvents.Text = "label7";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(130, 313);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(74, 13);
			this.label13.TabIndex = 41;
			this.label13.Text = "Wheel Events";
			// 
			// _keyUpEventsLabel
			// 
			this._keyUpEventsLabel.AutoSize = true;
			this._keyUpEventsLabel.Location = new System.Drawing.Point(232, 85);
			this._keyUpEventsLabel.Name = "_keyUpEventsLabel";
			this._keyUpEventsLabel.Size = new System.Drawing.Size(41, 13);
			this._keyUpEventsLabel.TabIndex = 40;
			this._keyUpEventsLabel.Text = "label11";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(126, 85);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(78, 13);
			this.label8.TabIndex = 39;
			this.label8.Text = "Key Up Events";
			// 
			// _keyDownEventsLabel
			// 
			this._keyDownEventsLabel.AutoSize = true;
			this._keyDownEventsLabel.Location = new System.Drawing.Point(232, 51);
			this._keyDownEventsLabel.Name = "_keyDownEventsLabel";
			this._keyDownEventsLabel.Size = new System.Drawing.Size(35, 13);
			this._keyDownEventsLabel.TabIndex = 38;
			this._keyDownEventsLabel.Text = "label4";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(112, 51);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(92, 13);
			this.label12.TabIndex = 37;
			this.label12.Text = "Key Down Events";
			// 
			// _keyActivityEventsLabel
			// 
			this._keyActivityEventsLabel.AutoSize = true;
			this._keyActivityEventsLabel.Location = new System.Drawing.Point(232, 16);
			this._keyActivityEventsLabel.Name = "_keyActivityEventsLabel";
			this._keyActivityEventsLabel.Size = new System.Drawing.Size(35, 13);
			this._keyActivityEventsLabel.TabIndex = 36;
			this._keyActivityEventsLabel.Text = "label2";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(106, 16);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(98, 13);
			this.label14.TabIndex = 35;
			this.label14.Text = "Key Activity Events";
			// 
			// _mouseStartEventsLabel
			// 
			this._mouseStartEventsLabel.AutoSize = true;
			this._mouseStartEventsLabel.Location = new System.Drawing.Point(233, 188);
			this._mouseStartEventsLabel.Name = "_mouseStartEventsLabel";
			this._mouseStartEventsLabel.Size = new System.Drawing.Size(41, 13);
			this._mouseStartEventsLabel.TabIndex = 34;
			this._mouseStartEventsLabel.Text = "label11";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(104, 188);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 13);
			this.label4.TabIndex = 33;
			this.label4.Text = "Mouse Start Events";
			// 
			// _mouseStopEventsLabel
			// 
			this._mouseStopEventsLabel.AutoSize = true;
			this._mouseStopEventsLabel.Location = new System.Drawing.Point(233, 217);
			this._mouseStopEventsLabel.Name = "_mouseStopEventsLabel";
			this._mouseStopEventsLabel.Size = new System.Drawing.Size(41, 13);
			this._mouseStopEventsLabel.TabIndex = 32;
			this._mouseStopEventsLabel.Text = "label11";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(104, 217);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(100, 13);
			this.label10.TabIndex = 31;
			this.label10.Text = "Mouse Stop Events";
			// 
			// _clickUpEventLabel
			// 
			this._clickUpEventLabel.AutoSize = true;
			this._clickUpEventLabel.Location = new System.Drawing.Point(233, 282);
			this._clickUpEventLabel.Name = "_clickUpEventLabel";
			this._clickUpEventLabel.Size = new System.Drawing.Size(35, 13);
			this._clickUpEventLabel.TabIndex = 30;
			this._clickUpEventLabel.Text = "label8";
			// 
			// _clickDownEventsLabel
			// 
			this._clickDownEventsLabel.AutoSize = true;
			this._clickDownEventsLabel.Location = new System.Drawing.Point(233, 251);
			this._clickDownEventsLabel.Name = "_clickDownEventsLabel";
			this._clickDownEventsLabel.Size = new System.Drawing.Size(35, 13);
			this._clickDownEventsLabel.TabIndex = 29;
			this._clickDownEventsLabel.Text = "label7";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(121, 282);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(83, 13);
			this.label6.TabIndex = 28;
			this.label6.Text = "Click Up Events";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(107, 251);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(97, 13);
			this.label5.TabIndex = 27;
			this.label5.Text = "Click Down Events";
			// 
			// _mouseMoveEventsLabel
			// 
			this._mouseMoveEventsLabel.AutoSize = true;
			this._mouseMoveEventsLabel.Location = new System.Drawing.Point(232, 154);
			this._mouseMoveEventsLabel.Name = "_mouseMoveEventsLabel";
			this._mouseMoveEventsLabel.Size = new System.Drawing.Size(35, 13);
			this._mouseMoveEventsLabel.TabIndex = 26;
			this._mouseMoveEventsLabel.Text = "label4";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(99, 154);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 13);
			this.label3.TabIndex = 25;
			this.label3.Text = "Mouse Move Events";
			// 
			// _mouseActivityEventsLabel
			// 
			this._mouseActivityEventsLabel.AutoSize = true;
			this._mouseActivityEventsLabel.Location = new System.Drawing.Point(232, 119);
			this._mouseActivityEventsLabel.Name = "_mouseActivityEventsLabel";
			this._mouseActivityEventsLabel.Size = new System.Drawing.Size(35, 13);
			this._mouseActivityEventsLabel.TabIndex = 24;
			this._mouseActivityEventsLabel.Text = "label2";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(92, 119);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 13);
			this.label1.TabIndex = 23;
			this.label1.Text = "Mouse Activity Events";
			// 
			// HooksForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(768, 402);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textBox1);
			this.Name = "HooksScreen";
			this.Text = "HooksForm";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label _wheelEvents;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label _keyUpEventsLabel;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label _keyDownEventsLabel;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label _keyActivityEventsLabel;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label _mouseStartEventsLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label _mouseStopEventsLabel;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label _clickUpEventLabel;
		private System.Windows.Forms.Label _clickDownEventsLabel;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label _mouseMoveEventsLabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label _mouseActivityEventsLabel;
		private System.Windows.Forms.Label label1;
	}
}
