//-----------------------------------------------------------------------
// <copyright file="LiteMainForm.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using Hydrogen;
using Hydrogen.Application;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Windows.Forms {

	public partial class LiteMainForm : ApplicationForm, IApplicationIconProvider, IUserInterfaceServices, IUserNotificationServices, IMainForm {
		private volatile INagDialog _nagDialogInstance;
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
			_nagDialogInstance = null;
			NumberActivations = 0;
		}

		#region Form Methods


		protected virtual void OnFirstActivated() {
			if (!Tools.Runtime.IsDesignMode) {
				// This is a blocking call (will show a nag if necessary)
				ApplicationServices.ApplyLicense();
			}
		}

		// Shown every time window becomes active window
		protected override void OnActivated(EventArgs e) {
			NumberActivations++;
			base.OnActivated(e);
			if (!Nagged) {
                Nagged = true;
				FireFirstActivatedEvent();
			}
		}


		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (!Tools.Runtime.IsDesignMode) {

				//// initialize local members
				#region Fire First Time Use Events

				if (ApplicationServices.ProductUsageInformation.NumberOfUsesBySystem == 1) {
					FireFirstTimeExecutedBySystemEvent();
				} else {
					FireNotFirstTimeExecutedBySystemEvent();
				}

				if (ApplicationServices.ProductUsageInformation.NumberOfUsesByUser == 1) {
					FireFirstTimeExecutedByUserEvent();
				} else {
					FireNotFirstTimeExecutedByUserEvent();
				}

				#endregion

				// trigger license verify 
				ComponentRegistry.Instance.Resolve<IBackgroundLicenseVerifier>().VerifyLicense();

				if ((WindowState == FormWindowState.Minimized || !Visible) && !Nagged) {
					Nagged = true;
					// This is a blocking call (will show a nag if necessary)
					ApplicationServices.ApplyLicense();
				}
			}
		}

		protected sealed override void OnClosing(CancelEventArgs cancelArgs) {
			base.OnClosing(cancelArgs);
			if (cancelArgs.Cancel) {
				// Base canceled closing because it probably hid and/or minimized the form
				ApplicationExiting = true;
			} else {

				bool cancelExit = false;
				try {
					// Set application exiting if not already done so
					if (!ApplicationExiting) {
						ApplicationExiting = true;
					}

					string cancelReason = string.Empty;

					// Ask user to confirm exit
					if (ApplicationServices.AskYN("Are you sure you want to exit?")) {
						// Now ask form observers to confirm exit
						FireApplicationExitingEvent(cancelArgs);
						cancelExit = cancelArgs.Cancel;
						// If no aborts, ask framework to confirm exit
						if (!cancelExit) {
							Sphere10Framework.Instance.EndWinFormsApplication(out cancelExit, out cancelReason);
						}


						if (cancelExit) {
							// An observer or framework exit task somewhere has cancelled the exit.
							// Ask user to exit anyway
							if (ApplicationServices.AskYN(ParagraphBuilder.Combine("The application failed to exit properly",
							                                                       cancelReason ?? string.Empty,
							                                                       "You may lose unsaved data if you exit", "Exit anyway?"))) {
								// This will force an exit
								Exit(true);
							} else {
								cancelArgs.Cancel = true;
							}
						}
					} else {
						cancelArgs.Cancel = true;
					}
				} finally {
					ApplicationExiting = cancelExit;
				}
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
			get {
				return Icon;
			}
		}

		#endregion

		#region IUserInterfaceServices Implementation 

		public virtual void Exit(bool force = false) {
            try {
                if (force || this.Disposing) {
					try {
						ApplicationExiting = true;
					} finally {
						System.Windows.Forms.Application.Exit();
					}
                    return;
                }
                ExecuteInUIFriendlyContext(
                    () => {
                        var oldAction = this.CloseAction;
                        try {
                            CloseAction = FormCloseAction.Close;
                            Close();
                        } catch {
                            Exit(true);
                        }
                        finally {
                            // This runs if close is aborted
                            oldAction = oldAction;
                        } 
                    });
            } catch {
                Exit(true);
            }
		}

		public virtual bool ApplicationExiting { get; set; }
			
		public virtual string Status { get; set; }

		public virtual void ExecuteInUIFriendlyContext(Action function, bool executeAsync = false) {
			if (executeAsync) {
				BeginInvoke(function);
			} else {
				Invoke(function);
			}
		}

		public virtual void ShowNagScreen(bool modal = false, string nagMessage = null) {
			ExecuteInUIFriendlyContext(
				() => {
					if (_nagDialogInstance == null) {
						_nagDialogInstance = ComponentRegistry.Instance.Resolve<INagDialog>();
						var parent = ApplicationServices.PrimaryUIController as IWin32Window;
						if (parent != null && parent is Form && ((Form)parent).WindowState == FormWindowState.Minimized) {
							_nagDialogInstance.StartPosition = FormStartPosition.CenterScreen;
						}
						_nagDialogInstance.ShowDialog(parent);
						_nagDialogInstance = null;
					} else {
						// this is being entered by non-UI thread, what if closed at this point?
						if (_nagDialogInstance.Visible) {
							_nagDialogInstance.Refresh();
						}
					}
				},
				executeAsync: !modal
				);
		}

		public virtual object PrimaryUIController {
			get { return this; }
		}

		#endregion

		#region IUserNotificationServices Implementation

		public virtual void ShowSendCommentDialog() {
			var dialog = ComponentRegistry.Instance.Resolve<ISendCommentsDialog>();
			dialog.ShowDialog();
		}

		public virtual void ShowSubmitBugReportDialog() {
			var dialog = ComponentRegistry.Instance.Resolve<IReportBugDialog>();
			dialog.ShowDialog();
		}

		public virtual void ShowRequestFeatureDialog() {
			var dialog = ComponentRegistry.Instance.Resolve<IRequestFeatureDialog>();
			dialog.ShowDialog();
		}

		public virtual void ShowAboutBox() {
			var dialog = ComponentRegistry.Instance.Resolve<IAboutBox>();
			dialog.ShowDialog();
		}

		public virtual void ReportError(Exception error) {
		    ApplicationServices.ExecuteInUIFriendlyContext(() => ExceptionDialog.Show(this, error));
		}

		public virtual void ReportError(string msg) {
			ReportError("Unexpected Error", msg);
		}

		public virtual void ReportError(string title, string msg) {
			ApplicationServices.ExecuteInUIFriendlyContext(
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
			ApplicationServices.ExecuteInUIFriendlyContext(
				() => {
					ReportError(title, msg);
					ApplicationServices.Exit(true);
				}
			);
		}

		public virtual void ReportInfo(string title, string msg) {
			ApplicationServices.ExecuteInUIFriendlyContext(
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
			   ) == DialogResult.Yes ? true : false;
		}

		#endregion

		#region Auxillary Methods

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
			}
		}

		#endregion

	}
}
