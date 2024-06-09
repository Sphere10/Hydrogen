// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class BaseKeyboardHook : BaseDeviceHook, IKeyboardHook {
	public event EventHandler<KeyEvent> KeyDown;
	public event EventHandler<KeyEvent> KeyUp;
	public event EventHandler<KeyEvent> KeyActivity;


	protected IActiveApplicationMonitor ActiveApplicationMonitor { get; private set; }
	protected BaseKeyboardHook(IActiveApplicationMonitor activeApplicationMonitor) {
		ActiveApplicationMonitor = activeApplicationMonitor;
	}

	public Func<Key, KeyState, bool> ShouldIntercept { get; set; } = null;

	protected virtual void ProcessKeyEvent(char asciiChar, Key key, ScanCode scanCode, KeyState keyState, bool shiftPressed, bool ctrlPressed, bool altPressed) {
		var keyEvent = new KeyEvent(
			ActiveApplicationMonitor.GetActiveApplicationName(),
			asciiChar,
			scanCode,
			key,
			keyState,
			shiftPressed,
			altPressed,
			ctrlPressed,
			DateTime.Now
		);

		switch (keyState) {
			case KeyState.Up:
				FireKeyUpEvent(keyEvent);
				break;
			case KeyState.Down:
				FireKeyDownEvent(keyEvent);
				break;
		}
		FireKeyActivityEvent(keyEvent);
	}

	protected virtual void OnKeyDown(KeyEvent keyEvent) {
	}

	protected virtual void OnKeyActivity(KeyEvent keyEvent) {
	}

	protected virtual void OnKeyUp(KeyEvent keyEvent) {
	}

	protected void FireKeyDownEvent(KeyEvent keyEvent) {
		OnKeyDown(keyEvent);
		if (KeyDown != null) {
			KeyDown(this, keyEvent);
		}
	}
	protected void FireKeyActivityEvent(KeyEvent keyEvent) {
		OnKeyActivity(keyEvent);
		if (KeyActivity != null) {
			KeyActivity(this, keyEvent);
		}
	}
	protected void FireKeyUpEvent(KeyEvent keyEvent) {
		OnKeyUp(keyEvent);
		if (KeyUp != null) {
			KeyUp(this, keyEvent);
		}
	}
}
