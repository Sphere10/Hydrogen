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

public partial class FormEx : Form, IUpdatable {
	private bool _updating;
	private bool _changedDuringUpdateScope;
	private bool _autoSave;
	private readonly IControlStateEventProvider _stateEventProvider;

	public event EventHandlerEx StateChanged;

	public FormEx() {
		InitializeComponent();
		CloseAction = FormCloseAction.Close;
		_changedDuringUpdateScope = false;
		Loaded = false;
		_updating = false;
		EnableStateChangeEvent = true;
		UpdateModelOnStateChanged = true;
		OpacityBeforeHide = 1;
		_stateEventProvider = !Tools.Runtime.IsDesignMode ? new ContainerControlStateEventProvider() : new NoOpControlStateProvider();
		_stateEventProvider.StateChanged += NotifyStateChanged;
		_stateEventProvider.SetControl(this);
	}

	[Category("Behavior")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[DefaultValue(FormCloseAction.Close)]
	public FormCloseAction CloseAction { get; set; }

	[Category("Behavior")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[DefaultValue(true)]
	public bool EnableStateChangeEvent { get; set; }

	[Category("Behavior")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[DefaultValue(true)]
	public bool UpdateModelOnStateChanged { get; set; }

	[Category("Behavior")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[DefaultValue(true)]
	public bool AutoSave {
		get => _autoSave;
		set => _autoSave = value;
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool Updating => _updating;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool Loaded { get; private set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	private double OpacityBeforeHide { get; set; }

	public virtual void SetLocalizedText(CultureInfo culture = null) {
	}

	public new virtual void Refresh() {
		using (this.EnterUpdateScope()) {
			CopyModelToUI();
		}
		base.Refresh();
	}

	public virtual new void Show() {
		if (WindowState == FormWindowState.Minimized) {
			TopMost = true;
			TopMost = false;
			Visible = true;
			WindowState = FormWindowState.Normal;
		} else {
			TopMost = true;
			TopMost = false;
			Visible = true;
		}
		if (Math.Abs(Opacity) < 0.00001D) // if Opacity == 0
			Opacity = OpacityBeforeHide;
	}

	public virtual new void Hide() {
		OpacityBeforeHide = Opacity;
		Opacity = 0;
		Visible = false;
	}

	public IDisposable EnterUpdateScope(FinishedUpdateBehaviour behaviour = FinishedUpdateBehaviour.Default) {
		return new UpdateScope(this, behaviour);
	}

	protected override void OnLoad(EventArgs e) {
		try {
			base.OnLoad(e);
			if (Loaded || DesignMode || Tools.Runtime.IsDesignMode)
				return;

			using (this.EnterUpdateScope()) {
				PopulatePrimingData();
				CopyModelToUI();
			}
			Loaded = true;
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	protected virtual void PopulatePrimingData() {
	}

	protected virtual void CopyUIToModel() {
	}

	protected virtual void CopyModelToUI() {
	}

	protected virtual void Save() {
	}

	protected virtual void OnStateChanged() {
	}

	protected override void OnClosing(CancelEventArgs e) {
		if (!CloseAction.HasFlag(FormCloseAction.Close)) {
			e.Cancel = true;
			if (CloseAction.HasFlag(FormCloseAction.Hide)) {
				Hide();
			}
			if (CloseAction.HasFlag(FormCloseAction.Minimize)) {
				WindowState = FormWindowState.Minimized;
			}
		}
	}

	private void NotifyStateChanged() {
		if (!EnableStateChangeEvent)
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


	public class StateEventProvider : ControlStateEventProviderBase<FormEx> {

		protected override void RegisterStateChangedListener(FormEx control, EventHandlerEx eventHandler) {
			control.StateChanged += eventHandler;
		}

		protected override void DeregisterStateChangedListener(FormEx control, EventHandlerEx eventHandler) {
			control.StateChanged -= eventHandler;
		}

	}
}
