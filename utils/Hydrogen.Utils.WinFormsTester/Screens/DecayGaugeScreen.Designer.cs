//-----------------------------------------------------------------------
// <copyright file="DecayGaugeForm.Designer.cs" company="Sphere 10 Software">
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
	partial class DecayGaugeScreen {
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
				_mouseHook.UninstallHook();
				_keyboardHook.UninstallHook();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this._gauge = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._decayCoefficient = new System.Windows.Forms.NumericUpDown();
			this._decayExponent = new System.Windows.Forms.NumericUpDown();
			this._decayOffset = new System.Windows.Forms.NumericUpDown();
			this._eventStrength = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this._gaugeMax = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this._gaugeStart = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this._resetButton = new System.Windows.Forms.Button();
			this._mouseKeyboardEventTriggers = new System.Windows.Forms.CheckBox();
			this._triggerEventButton = new System.Windows.Forms.Button();
			this._timer = new System.Windows.Forms.Timer(this.components);
			this._gaugeReadout = new System.Windows.Forms.Label();
			this._keyboardEventStrength = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this._mouseEventStrength = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._decayCoefficient)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._decayExponent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._decayOffset)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._eventStrength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._gaugeMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._gaugeStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._keyboardEventStrength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._mouseEventStrength)).BeginInit();
			this.SuspendLayout();
			// 
			// _gauge
			// 
			this._gauge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._gauge.Location = new System.Drawing.Point(12, 184);
			this._gauge.MarqueeAnimationSpeed = 0;
			this._gauge.Name = "_gauge";
			this._gauge.Size = new System.Drawing.Size(1089, 24);
			this._gauge.TabIndex = 0;
			this._gauge.Value = 50;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(41, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Decay Coefficient";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(46, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Decay Exponent";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(63, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Decay Offset";
			// 
			// _decayCoefficient
			// 
			this._decayCoefficient.DecimalPlaces = 2;
			this._decayCoefficient.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._decayCoefficient.Location = new System.Drawing.Point(138, 34);
			this._decayCoefficient.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._decayCoefficient.Name = "_decayCoefficient";
			this._decayCoefficient.Size = new System.Drawing.Size(120, 20);
			this._decayCoefficient.TabIndex = 7;
			this._decayCoefficient.ValueChanged += new System.EventHandler(this._decayCoefficient_ValueChanged);
			// 
			// _decayExponent
			// 
			this._decayExponent.DecimalPlaces = 2;
			this._decayExponent.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._decayExponent.Location = new System.Drawing.Point(138, 60);
			this._decayExponent.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._decayExponent.Name = "_decayExponent";
			this._decayExponent.Size = new System.Drawing.Size(120, 20);
			this._decayExponent.TabIndex = 8;
			this._decayExponent.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._decayExponent.ValueChanged += new System.EventHandler(this._decayExponent_ValueChanged);
			// 
			// _decayOffset
			// 
			this._decayOffset.DecimalPlaces = 2;
			this._decayOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._decayOffset.Location = new System.Drawing.Point(138, 86);
			this._decayOffset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._decayOffset.Name = "_decayOffset";
			this._decayOffset.Size = new System.Drawing.Size(120, 20);
			this._decayOffset.TabIndex = 9;
			this._decayOffset.ValueChanged += new System.EventHandler(this._decayOffset_ValueChanged);
			// 
			// _eventStrength
			// 
			this._eventStrength.DecimalPlaces = 3;
			this._eventStrength.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._eventStrength.Location = new System.Drawing.Point(769, 140);
			this._eventStrength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._eventStrength.Name = "_eventStrength";
			this._eventStrength.Size = new System.Drawing.Size(120, 20);
			this._eventStrength.TabIndex = 11;
			this._eventStrength.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(685, 140);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "Event Strength";
			// 
			// _gaugeMax
			// 
			this._gaugeMax.DecimalPlaces = 2;
			this._gaugeMax.Location = new System.Drawing.Point(420, 36);
			this._gaugeMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._gaugeMax.Name = "_gaugeMax";
			this._gaugeMax.Size = new System.Drawing.Size(120, 20);
			this._gaugeMax.TabIndex = 13;
			this._gaugeMax.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._gaugeMax.ValueChanged += new System.EventHandler(this._gaugeMax_ValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(352, 38);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(62, 13);
			this.label5.TabIndex = 12;
			this.label5.Text = "Gauge Max";
			// 
			// _gaugeStart
			// 
			this._gaugeStart.DecimalPlaces = 2;
			this._gaugeStart.Location = new System.Drawing.Point(420, 62);
			this._gaugeStart.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._gaugeStart.Name = "_gaugeStart";
			this._gaugeStart.Size = new System.Drawing.Size(120, 20);
			this._gaugeStart.TabIndex = 15;
			this._gaugeStart.ValueChanged += new System.EventHandler(this._gaugeStart_ValueChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(350, 64);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 13);
			this.label6.TabIndex = 14;
			this.label6.Text = "Gauge Start";
			// 
			// _resetButton
			// 
			this._resetButton.Location = new System.Drawing.Point(420, 88);
			this._resetButton.Name = "_resetButton";
			this._resetButton.Size = new System.Drawing.Size(120, 23);
			this._resetButton.TabIndex = 16;
			this._resetButton.Text = "Reset";
			this._resetButton.UseVisualStyleBackColor = true;
			this._resetButton.Click += new System.EventHandler(this._resetButton_Click);
			// 
			// _mouseKeyboardEventTriggers
			// 
			this._mouseKeyboardEventTriggers.AutoSize = true;
			this._mouseKeyboardEventTriggers.Location = new System.Drawing.Point(700, 37);
			this._mouseKeyboardEventTriggers.Name = "_mouseKeyboardEventTriggers";
			this._mouseKeyboardEventTriggers.Size = new System.Drawing.Size(217, 17);
			this._mouseKeyboardEventTriggers.TabIndex = 17;
			this._mouseKeyboardEventTriggers.Text = "Enable Mouse & Keyboard Event Triggers";
			this._mouseKeyboardEventTriggers.UseVisualStyleBackColor = true;
			this._mouseKeyboardEventTriggers.CheckedChanged += new System.EventHandler(this._mouseKeyboardEventTriggers_CheckedChanged);
			// 
			// _triggerEventButton
			// 
			this._triggerEventButton.Location = new System.Drawing.Point(700, 111);
			this._triggerEventButton.Name = "_triggerEventButton";
			this._triggerEventButton.Size = new System.Drawing.Size(201, 23);
			this._triggerEventButton.TabIndex = 18;
			this._triggerEventButton.Text = "Register Event";
			this._triggerEventButton.UseVisualStyleBackColor = true;
			this._triggerEventButton.Click += new System.EventHandler(this._triggerEventButton_Click);
			// 
			// _timer
			// 
			this._timer.Interval = 2;
			this._timer.Tick += new System.EventHandler(this._timer_Tick);
			// 
			// _gaugeReadout
			// 
			this._gaugeReadout.AutoSize = true;
			this._gaugeReadout.BackColor = System.Drawing.Color.Transparent;
			this._gaugeReadout.Location = new System.Drawing.Point(486, 190);
			this._gaugeReadout.Name = "_gaugeReadout";
			this._gaugeReadout.Size = new System.Drawing.Size(64, 13);
			this._gaugeReadout.TabIndex = 19;
			this._gaugeReadout.Text = "Gauge Start";
			// 
			// _keyboardEventStrength
			// 
			this._keyboardEventStrength.DecimalPlaces = 3;
			this._keyboardEventStrength.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._keyboardEventStrength.Location = new System.Drawing.Point(769, 57);
			this._keyboardEventStrength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._keyboardEventStrength.Name = "_keyboardEventStrength";
			this._keyboardEventStrength.Size = new System.Drawing.Size(120, 20);
			this._keyboardEventStrength.TabIndex = 21;
			this._keyboardEventStrength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(664, 59);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(99, 13);
			this.label7.TabIndex = 20;
			this.label7.Text = "Key Event Strength";
			// 
			// _mouseEventStrength
			// 
			this._mouseEventStrength.DecimalPlaces = 3;
			this._mouseEventStrength.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._mouseEventStrength.Location = new System.Drawing.Point(769, 81);
			this._mouseEventStrength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._mouseEventStrength.Name = "_mouseEventStrength";
			this._mouseEventStrength.Size = new System.Drawing.Size(120, 20);
			this._mouseEventStrength.TabIndex = 23;
			this._mouseEventStrength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(650, 83);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(113, 13);
			this.label8.TabIndex = 22;
			this.label8.Text = "Mouse Event Strength";
			// 
			// DecayGaugeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1113, 220);
			this.Controls.Add(this._mouseEventStrength);
			this.Controls.Add(this.label8);
			this.Controls.Add(this._keyboardEventStrength);
			this.Controls.Add(this.label7);
			this.Controls.Add(this._gaugeReadout);
			this.Controls.Add(this._triggerEventButton);
			this.Controls.Add(this._mouseKeyboardEventTriggers);
			this.Controls.Add(this._resetButton);
			this.Controls.Add(this._gaugeStart);
			this.Controls.Add(this.label6);
			this.Controls.Add(this._gaugeMax);
			this.Controls.Add(this.label5);
			this.Controls.Add(this._eventStrength);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._decayOffset);
			this.Controls.Add(this._decayExponent);
			this.Controls.Add(this._decayCoefficient);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._gauge);
			this.Name = "DecayGaugeScreen";
			this.Text = "DecayGaugeForm";
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DecayGaugeForm_KeyPress);
			((System.ComponentModel.ISupportInitialize)(this._decayCoefficient)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._decayExponent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._decayOffset)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._eventStrength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._gaugeMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._gaugeStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._keyboardEventStrength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._mouseEventStrength)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar _gauge;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown _decayCoefficient;
		private System.Windows.Forms.NumericUpDown _decayExponent;
		private System.Windows.Forms.NumericUpDown _decayOffset;
		private System.Windows.Forms.NumericUpDown _eventStrength;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown _gaugeMax;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown _gaugeStart;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button _resetButton;
		private System.Windows.Forms.CheckBox _mouseKeyboardEventTriggers;
		private System.Windows.Forms.Button _triggerEventButton;
		private System.Windows.Forms.Timer _timer;
		private System.Windows.Forms.Label _gaugeReadout;
		private System.Windows.Forms.NumericUpDown _keyboardEventStrength;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown _mouseEventStrength;
		private System.Windows.Forms.Label label8;
	}
}
