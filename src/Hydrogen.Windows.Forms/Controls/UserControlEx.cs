// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class UserControlEx : UserControl, IUpdatable {
	private bool _updating;
	private bool _changedDuringUpdateScope = false;
	private readonly IControlStateEventProvider _stateEventProvider;

	public event EventHandlerEx StateChanged;

	public UserControlEx() {
		Loaded = false;
		_updating = false;
		EnableStateChangeEvent = true;
		Disposables = new Disposables();
		_updating = false;
		UpdateModelOnStateChanged = true;
		_stateEventProvider = !Tools.Runtime.IsDesignMode ? new ContainerControlStateEventProvider() : new NoOpControlStateProvider();
		_stateEventProvider.StateChanged += NotifyStateChanged;
		_stateEventProvider.SetControl(this);
	}

	public Disposables Disposables { get; }

	[Category("Behavior")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[DefaultValue(true)]
	public bool EnableStateChangeEvent { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool Updating => _updating;

	[Category("Behavior")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[DefaultValue(true)]
	public bool UpdateModelOnStateChanged { get; set; }


	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	protected bool Loaded { get; private set; }

	public override void Refresh() {
		base.Refresh();
		using (this.EnterUpdateScope(FinishedUpdateBehaviour.DoNothing)) {
			CopyModelToUI();
		}
	}

	public virtual void SetLocalizedText(CultureInfo culture = null) {
	}

	public IDisposable EnterUpdateScope(FinishedUpdateBehaviour behaviour = FinishedUpdateBehaviour.Default) {
		return new UpdateScope(this, behaviour);
	}

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		if (!Tools.Runtime.IsDesignMode) {
			using (this.EnterUpdateScope(FinishedUpdateBehaviour.DoNothing)) {
				InitializeUIPrimingData();
				CopyModelToUI();
				SetLocalizedText();
			}
			Loaded = true;
		}
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		if (disposing)
			Disposables.Dispose();
	}

	protected virtual void InitializeUIPrimingData() {
	}

	protected virtual void CopyUIToModel() {
	}

	protected virtual void CopyModelToUI() {
	}

	protected virtual void OnStateChanged() {
	}

	private void NotifyStateChanged() {
		if (!Loaded || !EnableStateChangeEvent)
			return;
		if (!Updating) {
			if (UpdateModelOnStateChanged)
				CopyUIToModel();
			OnStateChanged();
			StateChanged?.Invoke();
		} else {
			_changedDuringUpdateScope = true;
		}
	}

	void IUpdatable.BeginUpdate() {
		if (_updating == true)
			return;
		_updating = true;
		_changedDuringUpdateScope = false;
	}

	void IUpdatable.FinishUpdate(FinishedUpdateBehaviour behaviour) {
		if (_updating == false)
			return;
		_updating = false;
		if (_changedDuringUpdateScope) {
			if (behaviour.HasFlag(FinishedUpdateBehaviour.ForceCopyUIToModel)) {
				CopyUIToModel();
			}
			if (behaviour.HasFlag(FinishedUpdateBehaviour.CopyModelToUI)) {
				CopyModelToUI();
			}
			if (behaviour.HasFlag(FinishedUpdateBehaviour.NotifyStateChanged)) {
				NotifyStateChanged();
			}
		}
		_changedDuringUpdateScope = false;
	}


	public class StateEventProvider : ControlStateEventProviderBase<UserControlEx> {

		protected override void RegisterStateChangedListener(UserControlEx control, EventHandlerEx eventHandler) {
			control.StateChanged += eventHandler;
		}

		protected override void DeregisterStateChangedListener(UserControlEx control, EventHandlerEx eventHandler) {
			control.StateChanged -= eventHandler;
		}
	}
}
