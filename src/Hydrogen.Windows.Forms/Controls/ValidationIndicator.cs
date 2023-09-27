// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class ValidationIndicator : UserControl {
	private ValidationState _validationState;
	private string _validationMessage;
	private int _validationRequests;

	[Category("Action")]
	[Description("Performs the validation tasks which result in the indicator icon changing.")]
	public event EventHandlerEx<ValidationIndicator, ValidationIndicatorEvent> PerformValidation;

	[Category("Behavior")]
	[Description("When validation state changes")]
	public event EventHandlerEx<ValidationIndicator, ValidationState, ValidationState> ValidationStateChanged;


	public ValidationIndicator() {
		SetStyle(ControlStyles.UserPaint, true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		SetStyle(ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		_validationMessage = string.Empty;
		_validationRequests = 0;
		new object();
		State = ValidationState.Valid;
		ShowValidationMessageToolTip = true;
		ToolTipDurationMS = 500;
		this.EnabledChanged += (o, e) => {
			this.Visible = this.Enabled;
			if (this.Enabled && RunValidatorWhenEnabled) {
				RunValidation();
				return;
			}
		};
		InitializeComponent();
	}


	[Category("Behavior")]
	[DefaultValue(ValidationState.Valid)]
	public ValidationState State {
		get { return _validationState; }
		set {
			switch (value) {
				case ValidationState.Disabled:
					if (_loadingCircle != null) {
						_loadingCircle.Visible = false;
						_loadingCircle.Active = false;
					}
					_validationMessage = string.Empty;
					if (_toolTip != null)
						_toolTip.ToolTipTitle = string.Empty;
					SetBackgroundImage(!DesignMode ? null : Resources.Tick);
					break;
				case ValidationState.Error:
					if (_loadingCircle != null) {
						_loadingCircle.Visible = false;
						_loadingCircle.Active = false;
					}
					if (_toolTip != null)
						_toolTip.ToolTipTitle = "Error";
					SetBackgroundImage(Hydrogen.Windows.Forms.Resources.Cross);
					break;
				case ValidationState.Valid:
					if (_loadingCircle != null) {
						_loadingCircle.Visible = false;
						_loadingCircle.Active = false;
					}
					if (_toolTip != null)
						_toolTip.ToolTipTitle = "Success";
					SetBackgroundImage(Hydrogen.Windows.Forms.Resources.Tick);
					break;
				case ValidationState.Validating:
					SetBackgroundImage(null);
					if (_loadingCircle != null) {
						_loadingCircle.Visible = true;
						_loadingCircle.Active = true;
					}
					if (_toolTip != null)
						_toolTip.ToolTipTitle = "Validating";
					_validationMessage = "Validation is running";
					break;
			}
			var previousState = _validationState;
			_validationState = value;
			if (previousState != _validationState)
				RaiseValidationStateChangedEvent(previousState, _validationState);
		}

	}

	[Category("Behavior")]
	[DefaultValue(true)]
	public bool ShowValidationMessageToolTip { get; set; }

	[Category("Behavior")]
	[DefaultValue(500)]
	public int ToolTipDurationMS { get; set; }

	[Category("Behavior")]
	[DefaultValue(true)]
	public bool RunValidatorWhenEnabled { get; set; }


	public async void RunValidation() {
		if (!this.Enabled)
			return;

		if (Interlocked.Increment(ref _validationRequests) == 1) {
			while (Volatile.Read(ref _validationRequests) > 0) {
				Volatile.Write(ref _validationRequests, 1);
				var results = new Queue<ValidationIndicatorEvent>();
				State = ValidationState.Validating;
				var superClassResult = new ValidationIndicatorEvent();
				await Task.Run(() => OnPerformValidation(superClassResult));
				results.Enqueue(superClassResult);
				if (PerformValidation != null) {
					foreach (var handler in PerformValidation.GetInvocationList()) {
						var handlerEventArgs = new ValidationIndicatorEvent();
						await Task.Run(() => handler.DynamicInvoke(this, handlerEventArgs));
						results.Enqueue(handlerEventArgs);
					}
				}

				// Only show result on last validation cycle
				if (Interlocked.Decrement(ref _validationRequests) == 0) {
					var valid = results.All(r => r.ValidationResult == true);
					State = valid ? ValidationState.Valid : ValidationState.Error;
					_validationMessage = results.Where(r => r.ValidationResult == valid).Select(r => r.ValidationMessage).ToParagraphCase();
				}
			}
		}

	}


	protected virtual void OnPerformValidation(ValidationIndicatorEvent @event) {
	}

	protected virtual void OnRaiseValidationStateChanged(ValidationState previousState, ValidationState newState) {
	}

	private void SetBackgroundImage(Image image) {
		this.BackgroundImage = image;
		this.BackgroundImageLayout = ImageLayout.Stretch;
		this.Invalidate();
	}


	private void RaiseValidationStateChangedEvent(ValidationState previousState, ValidationState newState) {
		OnRaiseValidationStateChanged(previousState, newState);
		if (ValidationStateChanged != null)
			ValidationStateChanged(this, previousState, newState);
	}

	#region Event Handlers

	private void _MouseHover(object sender, EventArgs e) {
		if (ShowValidationMessageToolTip) {
			var displayMsg = string.IsNullOrWhiteSpace(_validationMessage) ? "Valid" : _validationMessage;
			_toolTip.Show(displayMsg, this);
		}
	}

	#endregion

}
