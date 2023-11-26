// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms;

public partial class LiteMainForm : ApplicationForm, IMainForm {
	public event EventHandler FirstActivation;
	public event EventHandler FirstTimeExecutedBySystemEvent;
	public event EventHandler NotFirstTimeExecutedBySystemEvent;
	public event EventHandler FirstTimeExecutedByUserEvent;
	public event EventHandler NotFirstTimeExecutedByUserEvent;
	public event EventHandler<CancelEventArgs> ApplicationExitingEvent;

	public LiteMainForm() {
		System.Windows.Forms.Application.ThreadException += ApplicationOnThreadException;
		InitializeComponent();
		Nagged = false;
		NumberActivations = 0;
	}

	protected bool SuppressExitConfirmation { get; set; }

	#region Form Methods

	protected virtual void OnFirstActivated() {
		if (!Tools.Runtime.IsDesignMode)
			EnforceLicense();
	}

	// Shown every time window becomes active window
	protected override void OnActivated(EventArgs e) {
		NumberActivations++;
		base.OnActivated(e);
		if (!Nagged) {
			Nagged = true;
			if (!ApplicationExiting)
				FireFirstActivatedEvent();
		}
	}


	protected override async void OnLoad(EventArgs e) {
		base.OnLoad(e);
		if (!Tools.Runtime.IsDesignMode) {

			//// initialize local members

			#region Fire First Time Use Events

			var productUsageServices = HydrogenFramework.Instance.ServiceProvider.GetService<IProductUsageServices>();
			var usageInfo = productUsageServices.ProductUsageInformation;
			if (usageInfo.NumberOfUsesBySystem == 1) {
				FireFirstTimeExecutedBySystemEvent();
			} else {
				FireNotFirstTimeExecutedBySystemEvent();
			}

			if (usageInfo.NumberOfUsesByUser == 1) {
				FireFirstTimeExecutedByUserEvent();
			} else {
				FireNotFirstTimeExecutedByUserEvent();
			}

			#endregion

		}
	}

	protected sealed override void OnClosing(CancelEventArgs cancelArgs) {
		base.OnClosing(cancelArgs);
		if (cancelArgs.Cancel) {
			// Base canceled closing because it probably hid and/or minimized the form
			ApplicationExiting = false;
			return;
		}

		try {
			// Set application exiting if not already done so
			if (!ApplicationExiting)
				ApplicationExiting = true;

			// Ask user to confirm exit
			if (SuppressExitConfirmation || AskYN("Are you sure you want to exit?")) {
				// Now ask form observers to confirm exit
				FireApplicationExitingEvent(cancelArgs);
				// If no aborts, ask framework to confirm exit
				if (!cancelArgs.Cancel)
					HydrogenFramework.Instance.TerminateApplication(0);
			} else {
				cancelArgs.Cancel = true;
			}
		} finally {
			ApplicationExiting = !cancelArgs.Cancel;
		}
	}

	protected virtual void OnApplicationExiting(CancelEventArgs cancelEventArgs) {
	}

	protected virtual void OnFirstTimeExecutedBySystem() {
	}

	protected virtual void OnFirstTimeExecutedByUser() {
	}

	protected virtual void OnNotFirstTimeExecutedBySystem() {
	}

	protected virtual void OnNotFirstTimeExecutedByUser() {
	}

	#endregion

	#region Form Properties

	private int NumberActivations { get; set; }

	private bool Nagged { get; set; }

	#endregion

	#region IApplicationIcon Implementation

	public virtual Icon ApplicationIcon {
		get { return new Icon(this.Icon, 128, 128); }
	}

	#endregion

	#region IUserInterfaceServices Implementation

	protected override void WndProc(ref Message m) {
		const int WM_QUERYENDSESSION = 0x11;
		if (m.Msg == WM_QUERYENDSESSION) {
			// CloseActions Hide | Minimize will hold up session shutdown, and SystemEvents doesn't get fired!
			CloseAction = FormCloseAction.Close;
			SuppressExitConfirmation = true;
		}
		base.WndProc(ref m);
	}

	public virtual void Exit(bool force = false) {
		if (!this.IsDisposed && this.IsHandleCreated) {
			ExecuteInUIFriendlyContext(ExitInternal);
		} else {
			ExitInternal();
		}

		void ExitInternal() {
			var oldSuppressExitConfirmation = SuppressExitConfirmation;
			var oldAction = CloseAction;
			try {
				SuppressExitConfirmation = force;
				//System.Environment.Exit(-1);
				if (SuppressExitConfirmation) 
					HydrogenFramework.Instance.TerminateApplication(0);

				CloseAction = FormCloseAction.Close;
				Close();
				System.Windows.Forms.Application.Exit();
			} catch {
				try {
					System.Windows.Forms.Application.Exit();
				} catch {
					HydrogenFramework.Instance.TerminateApplication(0);
				}
			} finally {
				// This runs if close is aborted
				CloseAction = oldAction;
				SuppressExitConfirmation = oldSuppressExitConfirmation;
			}
		}
	}

	public virtual bool ApplicationExiting { get; set; }

	public virtual string Status { get; set; }

	public virtual async void ExecuteInUIFriendlyContext(Action function, bool executeAsync = false) {
		if (executeAsync) {
			BeginInvoke(function);
		} else {
			Invoke(function);
		}
	}

	public virtual void ShowNagScreen(string nagMessage) {
		ExecuteInUIFriendlyContext(() => {
			var nagDialogInstance = HydrogenFramework.Instance.ServiceProvider.GetService<INagDialog>();
			if (WindowState == FormWindowState.Minimized) {
				nagDialogInstance.StartPosition = FormStartPosition.CenterScreen;
			}
			nagDialogInstance.NagMessage = nagMessage;
			nagDialogInstance.ShowDialog(this);
		});
	}


	public virtual object PrimaryUIController {
		get { return this; }
	}

	#endregion

	#region IUserNotificationServices Implementation

	public virtual void ShowSendCommentDialog() {
		var dialog = HydrogenFramework.Instance.ServiceProvider.GetService<IProductSendCommentsDialog>();
		dialog.ShowDialog();
	}

	public virtual void ShowSubmitBugReportDialog() {
		var dialog = HydrogenFramework.Instance.ServiceProvider.GetService<IProductReportBugDialog>();
		dialog.ShowDialog();
	}

	public virtual void ShowRequestFeatureDialog() {
		var dialog = HydrogenFramework.Instance.ServiceProvider.GetService<IProductRequestFeatureDialog>();
		dialog.ShowDialog();
	}

	public virtual void ShowAboutBox() {
		var dialog = HydrogenFramework.Instance.ServiceProvider.GetService<IAboutBox>();
		dialog.ShowDialog();
	}

	public virtual void ReportError(Exception error) {
		ExecuteInUIFriendlyContext(() => ExceptionDialog.Show(this, error));
	}

	public virtual void ReportError(string msg) {
		ReportError("Unexpected Error", msg);
	}

	public virtual void ReportError(string title, string msg) {
		ExecuteInUIFriendlyContext(
			() =>
				MessageBox.Show(
					msg,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error,
					MessageBoxDefaultButton.Button1
				)
		);
	}

	public virtual void ReportFatalError(string title, string msg) {
		ExecuteInUIFriendlyContext(
			() => {
				ReportError(title, msg);
				Exit(true);
			}
		);
	}

	public virtual void ReportInfo(string title, string msg) {
		ExecuteInUIFriendlyContext(
			() => DialogEx.Show(
				this,
				SystemIconType.Information,
				msg,
				title,
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			)
		);
	}

	public bool AskYN(string question) {
		return
			DialogEx.Show(
				this,
				SystemIconType.Question,
				question,
				"Confirm",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
				//MessageBoxDefaultButton.Button2
			) == DialogResult.Yes
				? true
				: false;
	}

	#endregion

	#region Auxillary Methods

	private void EnforceLicense() {
		var licenseEnforcer = HydrogenFramework.Instance.ServiceProvider.GetService<IProductLicenseEnforcer>();
		licenseEnforcer.EnforceLicense(false);
	}

	private void FireFirstActivatedEvent() {
		OnFirstActivated();
		if (FirstActivation != null) {
			FirstActivation(this, EventArgs.Empty);
		}
	}

	private void FireApplicationExitingEvent(CancelEventArgs cancelEvent) {
		OnApplicationExiting(cancelEvent);
		// Call each observer, if any one decides to cancel abort do not notify remaining observers
		if (ApplicationExitingEvent != null) {
			foreach (EventHandler<CancelEventArgs> exitHandler in ApplicationExitingEvent.GetInvocationList()) {
				exitHandler(this, cancelEvent);
				if (cancelEvent.Cancel) {
					break;
				}
			}
		}
	}

	private void FireFirstTimeExecutedBySystemEvent() {
		OnFirstTimeExecutedBySystem();
		if (FirstTimeExecutedBySystemEvent != null) {
			FirstTimeExecutedBySystemEvent(this, EventArgs.Empty);
		}
	}

	private void FireFirstTimeExecutedByUserEvent() {
		OnFirstTimeExecutedByUser();
		if (FirstTimeExecutedByUserEvent != null) {
			FirstTimeExecutedByUserEvent(this, EventArgs.Empty);
		}
	}

	private void FireNotFirstTimeExecutedBySystemEvent() {
		OnNotFirstTimeExecutedBySystem();
		if (NotFirstTimeExecutedBySystemEvent != null) {
			NotFirstTimeExecutedBySystemEvent(this, EventArgs.Empty);
		}
	}

	private void FireNotFirstTimeExecutedByUserEvent() {
		OnNotFirstTimeExecutedByUser();
		if (NotFirstTimeExecutedByUserEvent != null) {
			NotFirstTimeExecutedByUserEvent(this, EventArgs.Empty);
		}
	}

	private void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs threadExceptionEventArgs) {
		try {
			this.ReportError(threadExceptionEventArgs.Exception);
		} catch {
			// ignored
		}
	}

	#endregion

}
