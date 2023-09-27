// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class PadLockButton : Button {
	private PadLockState _lockState;

	[Description("Raised when the pad lock changes from closed to locked states")]
	[Category("Action")]
	public event EventHandlerEx<PadLockButton, PadLockState> PadLockStateChanged;

	public PadLockButton() {
		this.SetStyle(ControlStyles.Selectable, false);
		this.Text = string.Empty;
		this.Size = new System.Drawing.Size(20, 20);
		this.MinimumSize = new System.Drawing.Size(20, 20);
		this.MaximumSize = new System.Drawing.Size(20, 20);
		LockState = PadLockState.Locked;
		this.Click += new EventHandler(PadLockButton_Click);
		this.BackgroundImageLayout = ImageLayout.Center;
	}

	public override string Text {
		get { return string.Empty; }
		set { base.Text = value; }
	}


	[Category("Behavior")]
	[DefaultValue(PadLockState.Locked)]
	public PadLockState LockState {
		get { return _lockState; }
		set {
			var valueChanged = _lockState != value;
			_lockState = value;
			if (valueChanged) {
				switch (_lockState) {
					case PadLockState.Unlocked:
						BackgroundImage = Resources.PadLockOpen;
						break;
					case PadLockState.Locked:
						BackgroundImage = Resources.PadLockClosed;
						break;
				}
				RaisePadLockStateChangedEvent();
			}
		}
	}

	public void ToggleLockState() {
		switch (LockState) {
			case PadLockState.Unlocked:
				LockState = PadLockState.Locked;
				break;
			case PadLockState.Locked:
				LockState = PadLockState.Unlocked;
				break;
		}
	}

	protected virtual void OnPadLockStateChanged() {
	}

	private void RaisePadLockStateChangedEvent() {
		OnPadLockStateChanged();
		if (PadLockStateChanged != null)
			PadLockStateChanged(this, _lockState);
	}

	private void PadLockButton_Click(object sender, EventArgs e) {
		try {
			ToggleLockState();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}


	public enum PadLockState {
		Unlocked,
		Locked
	}

}
