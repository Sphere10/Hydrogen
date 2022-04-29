//-----------------------------------------------------------------------
// <copyright file="DecayGaugeForm.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework;
using Sphere10.Framework.Windows;
using Sphere10.Framework.Windows.Forms;


namespace Sphere10.FrameworkTester.WinForms {
	public partial class DecayGaugeScreen : ApplicationScreen {
		private IMouseHook _mouseHook;
		private IKeyboardHook _keyboardHook;

		public DecayGaugeScreen() {
			InitializeComponent();
			Reset();
			_timer.Start();

			_mouseHook = new WindowsMouseHook();
			_mouseHook.InstallHook();
			_mouseHook.Activity += new EventHandler<MouseEvent>(_mouseHook_Activity);

			_keyboardHook = new WindowsKeyboardHook();
			_keyboardHook.InstallHook();
			_keyboardHook.KeyActivity += new EventHandler<KeyEvent>(_keyboardHook_KeyActivity);

		}


		public DecayGauge DecayGauge { get; private set; }


		private void Reset() {
			DecayGauge = new DecayGauge(_gaugeStart.GetValueDouble(), _decayOffset.GetValueDouble(), _decayCoefficient.GetValueDouble(), _decayExponent.GetValueDouble());
			_gauge.Minimum = 0;
			_gauge.Maximum = _gaugeMax.GetValueInt();
			DecayGauge.MaxValue = _gaugeMax.GetValueDouble();
			_gauge.Value = _gaugeStart.GetValueInt();
		}

		private void RegisterEvent() { DecayGauge.RegisterEvent(_eventStrength.GetValueDouble()); }

		private void DecayGaugeForm_KeyPress(object sender, KeyPressEventArgs e) {
			this.InvokeEx(
				() => {
					if (_mouseKeyboardEventTriggers.Checked) {
						RegisterEvent();
					}
				});
		}

		private void _triggerEventButton_Click(object sender, EventArgs e) { this.InvokeEx(RegisterEvent); }

		private void _resetButton_Click(object sender, EventArgs e) { this.InvokeEx(Reset); }

		private void _timer_Tick(object sender, EventArgs e) {
			this.InvokeEx(
				() => {
					_gauge.Value = (int) Math.Round(DecayGauge.Level, 0).ClipTo(_gauge.Minimum, _gauge.Maximum);
					_gaugeReadout.Text = _gauge.Value.ToString();
				});
		}

		private void _decayCoefficient_ValueChanged(object sender, EventArgs e) {
			this.InvokeEx(
				() => {
					DecayGauge.DecayCoefficient = _decayCoefficient.GetValueDouble();
				});
		}

		private void _decayExponent_ValueChanged(object sender, EventArgs e) {
			this.InvokeEx(
				() => {
					DecayGauge.DecayExponent = _decayExponent.GetValueDouble();
				}
				);
		}

		private void _decayOffset_ValueChanged(object sender, EventArgs e) {
			this.InvokeEx(
				() => {
					DecayGauge.DecayOffset = _decayOffset.GetValueDouble();
				});
		}

		private void _gaugeMax_ValueChanged(object sender, EventArgs e) {
			this.InvokeEx(
				() => {
					_gauge.Maximum = _gaugeMax.GetValueInt();
					DecayGauge.MaxValue = _gaugeMax.GetValueDouble();
				});
		}

		private void _gaugeStart_ValueChanged(object sender, EventArgs e) {
			this.InvokeEx(
				() => {
					_gauge.Minimum = _gaugeStart.GetValueInt();

				});
		}

		private void _mouseKeyboardEventTriggers_CheckedChanged(object sender, EventArgs e) {
			this.InvokeEx(
				() => {
					if (_mouseKeyboardEventTriggers.Checked) {
						_mouseHook.StartHook();
						_keyboardHook.StartHook();
					} else {
						_mouseHook.StopHook();
						_keyboardHook.StopHook();
					}
				});
		}


		private void _keyboardHook_KeyActivity(object sender, KeyEvent e) {
			this.InvokeEx(
				() => DecayGauge.RegisterEvent(_keyboardEventStrength.GetValueDouble()));
		}

		private void _mouseHook_Activity(object sender, MouseEvent e) {
			this.InvokeEx(
				() => DecayGauge.RegisterEvent(_mouseEventStrength.GetValueDouble()));
		}

	}
}
