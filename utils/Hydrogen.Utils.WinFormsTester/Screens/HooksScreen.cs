// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Windows;
using Hydrogen.Windows.Forms;


namespace Hydrogen.Utils.WinFormsTester;

public partial class HooksScreen : ApplicationScreen {
	private WindowsKeyboardHook _keyHook;
	private WindowsMouseHook _mouseHook;
	private long keyActivity = 0;
	private long keyDown = 0;
	private long keyUp = 0;
	private long mouseActivity = 0;
	private long mouseMotion = 0;
	private long mouseMotionStart = 0;
	private long mouseMotionStop = 0;
	private long mouseClickDown = 0;
	private long mouseClickUp = 0;
	private long mouseWheel = 0;


	public HooksScreen() {
		InitializeComponent();
		_keyHook = new WindowsKeyboardHook();
		_keyHook.KeyActivity += new EventHandler<KeyEvent>(_keyHook_KeyActivity);
		_keyHook.KeyDown += new EventHandler<KeyEvent>(_keyHook_KeyDown);
		_keyHook.KeyUp += new EventHandler<KeyEvent>(_keyHook_KeyUp);

		_mouseHook = new WindowsMouseHook();
		_mouseHook.Activity += new EventHandler<MouseEvent>(_mouseHook_Activity);
		_mouseHook.Motion += new EventHandler<MouseMoveEvent>(_mouseHook_Motion);
		_mouseHook.MotionStart += new EventHandler<MouseMoveEvent>(_mouseHook_MotionStart);
		_mouseHook.MotionStop += new EventHandler<MouseMoveEvent>(_mouseHook_MotionStop);
		_mouseHook.Click += new EventHandler<MouseClickEvent>(_mouseHook_Click);
		_mouseHook.Scroll += new EventHandler<MouseWheelEvent>(_mouseHook_Scroll);

		RefreshUI();
	}

	void _mouseHook_Scroll(object sender, MouseWheelEvent e) {
		mouseWheel++;
		AppendText("Wheel Delta = {0}{1}", e.Delta, Environment.NewLine);
	}

	private void _mouseHook_Click(object sender, MouseClickEvent e) {
		string textToAdd = string.Empty;
		switch (e.ButtonState) {
			case MouseButtonState.Down:
				mouseClickDown++;
				break;
			case MouseButtonState.Up:
				mouseClickUp++;
				break;
		}
		textToAdd += string.Format("Clicked {0} {1} ({2})", e.Buttons, e.ButtonState, e.ClickType);
		AppendText("{0}{1}", textToAdd, Environment.NewLine);
		RefreshUI();
	}

	private void _mouseHook_Activity(object sender, MouseEvent e) {
		mouseActivity++;
		RefreshUI();
	}

	private void _mouseHook_MotionStop(object sender, MouseMoveEvent e) {
		mouseMotionStop++;
		RefreshUI();
	}

	private void _mouseHook_MotionStart(object sender, MouseMoveEvent e) {
		mouseMotionStart++;
		RefreshUI();
	}

	private void _mouseHook_Motion(object sender, MouseMoveEvent e) {
		mouseMotion++;
		RefreshUI();
	}

	private void _keyHook_KeyUp(object sender, KeyEvent e) {
		keyUp++;
		AppendText("Key Up = {0}{1}", e.Key, Environment.NewLine);
		RefreshUI();
	}

	private void _keyHook_KeyDown(object sender, KeyEvent e) {
		keyDown++;
		AppendText("Key Down = {0}{1}", e.Key, Environment.NewLine);
		RefreshUI();
	}

	private void _keyHook_KeyActivity(object sender, KeyEvent e) {
		keyActivity++;
		RefreshUI();
	}

	private void AppendText(string text, params object[] formatArgs) {
		this.InvokeEx(() => textBox1.AppendText(string.Format(text, formatArgs)));
	}

	private void RefreshUI() {
		if (!this.IsDisposed) {
			this.InvokeEx(
				() => {
					if (!this.IsDisposed) {
						_keyActivityEventsLabel.Text = string.Format("{0}", keyActivity);
						_keyDownEventsLabel.Text = string.Format("{0}", keyDown);
						_keyUpEventsLabel.Text = string.Format("{0}", keyUp);
						_mouseActivityEventsLabel.Text = string.Format("{0}", mouseActivity);
						_mouseMoveEventsLabel.Text = string.Format("{0}", mouseMotion);
						_mouseStartEventsLabel.Text = string.Format("{0}", mouseMotionStart);
						_mouseStopEventsLabel.Text = string.Format("{0}", mouseMotionStop);
						_clickDownEventsLabel.Text = string.Format("{0}", mouseClickDown);
						_clickUpEventLabel.Text = string.Format("{0}", mouseClickUp);
						_wheelEvents.Text = string.Format("{0}", mouseWheel);
					}
				}
			);
		}
	}

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		_keyHook.InstallHook();
		_keyHook.StartHook();
		_mouseHook.InstallHook();
		_mouseHook.StartHook();

	}

	protected override void OnDestroyScreen() {
		base.OnDestroyScreen();
		if (_keyHook.Status != DeviceHookStatus.Uninstalled) {
			_keyHook.StopHook();
			_keyHook.UninstallHook();
			_mouseHook.StopHook();
			_mouseHook.UninstallHook();
		}
	}


}
