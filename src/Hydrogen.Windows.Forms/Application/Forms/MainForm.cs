//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.Forms {

	public partial class MainForm : LiteMainForm {

		public MainForm() {
			InitializeComponent();
		}

		#region Form Properties

		public ToolStrip ToolStrip { get { return _toolStrip; } }

		public MenuStrip MenuStrip { get { return _menuStrip; } }

		public ToolStripStatusLabel StatusLabel { get { return _statusLabel; } }

		public ToolStripMenuItem PurchaseFullVersionToolStripMenuItem { get { return _purchaseFullVersionToolStripMenuItem; } }

		public ToolStripMenuItem HelpToolStripMenuItem { get { return _helpToolStripMenuItem; } }

		#endregion

		#region Form Methods

		protected override void OnFirstActivated() {
			base.OnFirstActivated();
			if (!Tools.Runtime.IsDesignMode) {
				// Show/Hide register menu item based on what's happened with the user nag screen
				if (WinFormsApplicationServices.Rights.FeatureRights != ProductFeatureRights.Full) {
					PurchaseFullVersionToolStripMenuItem.Visible = true;
				} else {
					PurchaseFullVersionToolStripMenuItem.Visible = false;
				}
			}
		}

		#endregion

		#region IUserInterfaceServices Overrides

		public override string Status {
			get {
				return _statusLabel.Text;
			}
			set {
				ExecuteInUIFriendlyContext( () => _statusLabel.Text = value );
			}
		}

		#endregion

		#region Event Handlers

		protected virtual void RequestAFeature_Click(object sender, EventArgs e) {
			WinFormsApplicationServices.ShowRequestFeatureDialog();
		}

		protected virtual void SendComment_Click(object sender, EventArgs e) {
			WinFormsApplicationServices.ShowSendCommentDialog();
		}

		protected virtual void ReportABug_Click(object sender, EventArgs e) {
			WinFormsApplicationServices.ShowSubmitBugReportDialog();
		}

		protected virtual void About_Click(object sender, EventArgs e) {
			WinFormsApplicationServices.ShowAboutBox();
		}

		protected virtual void ContextHelp_Click(object sender, EventArgs e) {
			WinFormsApplicationServices.ShowHelp();
		}

		protected virtual void UserGuide_Click(object sender, EventArgs e) {
			WinFormsApplicationServices.ShowHelp();
		}

		protected virtual void PurchaseFullVersion_Click(object sender, EventArgs e) {
			WinFormsApplicationServices.ShowNagScreen(true);
		}

		protected virtual void Exit_Click(object sender, EventArgs e) {
			WinFormsApplicationServices.Exit(false);
		}

		protected virtual void MainForm_HelpRequested(object sender, HelpEventArgs hlpevent) {
			WinFormsApplicationServices.ShowHelp();
		}

		#endregion


	
	}
}
