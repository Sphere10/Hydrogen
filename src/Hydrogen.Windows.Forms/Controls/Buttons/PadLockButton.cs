//-----------------------------------------------------------------------
// <copyright file="PadLockButton.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework;

namespace Sphere10.Framework.Windows.Forms {
	
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
			get {
				return string.Empty;
			}
			set {
				base.Text = value;
			}
		}

	

		[Category("Behavior")]
		[DefaultValue(PadLockState.Locked)]
		public PadLockState LockState {
			get {
				return _lockState;
			}
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
			switch(LockState) {
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
}
