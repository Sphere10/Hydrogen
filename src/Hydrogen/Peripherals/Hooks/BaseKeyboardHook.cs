//-----------------------------------------------------------------------
// <copyright file="BaseKeyboardHook.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	public abstract class BaseKeyboardHook : BaseDeviceHook, IKeyboardHook {
		public event EventHandler<KeyEvent>  KeyDown;
		public event EventHandler<KeyEvent>  KeyUp;
		public event EventHandler<KeyEvent>  KeyActivity;


		public IList<Key> InterceptKeys { get; private set; }

		protected IActiveApplicationMonitor ActiveApplicationMonitor { get; private set; }
		public BaseKeyboardHook(IActiveApplicationMonitor activeApplicationMonitor) {
			ActiveApplicationMonitor = activeApplicationMonitor;
			InterceptKeys = new SynchronizedList<Key>();
		}

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
}
