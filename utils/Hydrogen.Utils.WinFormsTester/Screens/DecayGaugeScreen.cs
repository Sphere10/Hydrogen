// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;
using Hydrogen.Windows;
using Hydrogen.Windows.Forms;


namespace Hydrogen.Utils.WinFormsTester;

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

	private void RegisterEvent() {
		DecayGauge.RegisterEvent(_eventStrength.GetValueDouble());
	}

	private void DecayGaugeForm_KeyPress(object sender, KeyPressEventArgs e) {
		this.InvokeEx(
			() => {
				if (_mouseKeyboardEventTriggers.Checked) {
					RegisterEvent();
				}
			});
	}

	private void _triggerEventButton_Click(object sender, EventArgs e) {
		this.InvokeEx(RegisterEvent);
	}

	private void _resetButton_Click(object sender, EventArgs e) {
		this.InvokeEx(Reset);
	}

	private void _timer_Tick(object sender, EventArgs e) {
		this.InvokeEx(
			() => {
				_gauge.Value = (int)Math.Round(DecayGauge.Level, 0).ClipTo(_gauge.Minimum, _gauge.Maximum);
				_gaugeReadout.Text = _gauge.Value.ToString();
			});
	}

	private void _decayCoefficient_ValueChanged(object sender, EventArgs e) {
		this.InvokeEx(
			() => { DecayGauge.DecayCoefficient = _decayCoefficient.GetValueDouble(); });
	}

	private void _decayExponent_ValueChanged(object sender, EventArgs e) {
		this.InvokeEx(
			() => { DecayGauge.DecayExponent = _decayExponent.GetValueDouble(); }
		);
	}

	private void _decayOffset_ValueChanged(object sender, EventArgs e) {
		this.InvokeEx(
			() => { DecayGauge.DecayOffset = _decayOffset.GetValueDouble(); });
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
			() => { _gauge.Minimum = _gaugeStart.GetValueInt(); });
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
