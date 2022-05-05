//-----------------------------------------------------------------------
// <copyright file="MainForm.Designer.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.Forms {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this._menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._userguideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._contextHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this._requestAFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sendACommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reportABugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this._purchaseFullVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._menuStrip.SuspendLayout();
			this._statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _menuStrip
			// 
			this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this._helpToolStripMenuItem});
			this._menuStrip.Location = new System.Drawing.Point(0, 0);
			this._menuStrip.Name = "_menuStrip";
			this._menuStrip.Size = new System.Drawing.Size(664, 24);
			this._menuStrip.TabIndex = 3;
			this._menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit_Click);
			// 
			// _helpToolStripMenuItem
			// 
			this._helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._userguideToolStripMenuItem,
            this._contextHelpToolStripMenuItem,
            this.toolStripSeparator1,
            this._requestAFeatureToolStripMenuItem,
            this.sendACommentToolStripMenuItem,
            this.reportABugToolStripMenuItem,
            this.toolStripSeparator4,
            this._purchaseFullVersionToolStripMenuItem,
            this._aboutToolStripMenuItem});
			this._helpToolStripMenuItem.Name = "_helpToolStripMenuItem";
			this._helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this._helpToolStripMenuItem.Text = "Help";
			// 
			// _userguideToolStripMenuItem
			// 
			this._userguideToolStripMenuItem.Name = "_userguideToolStripMenuItem";
			this._userguideToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this._userguideToolStripMenuItem.Text = "User Guide";
			this._userguideToolStripMenuItem.Click += new System.EventHandler(this.UserGuide_Click);
			// 
			// _contextHelpToolStripMenuItem
			// 
			this._contextHelpToolStripMenuItem.Image = global::Hydrogen.Windows.Forms.Resources.Help_16x16x32;
			this._contextHelpToolStripMenuItem.Name = "_contextHelpToolStripMenuItem";
			this._contextHelpToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this._contextHelpToolStripMenuItem.Text = "Context Help";
			this._contextHelpToolStripMenuItem.Click += new System.EventHandler(this.ContextHelp_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
			// 
			// _requestAFeatureToolStripMenuItem
			// 
			this._requestAFeatureToolStripMenuItem.Name = "_requestAFeatureToolStripMenuItem";
			this._requestAFeatureToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this._requestAFeatureToolStripMenuItem.Text = "Request a feature";
			this._requestAFeatureToolStripMenuItem.Click += new System.EventHandler(this.RequestAFeature_Click);
			// 
			// sendACommentToolStripMenuItem
			// 
			this.sendACommentToolStripMenuItem.Name = "sendACommentToolStripMenuItem";
			this.sendACommentToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.sendACommentToolStripMenuItem.Text = "Send a comment";
			this.sendACommentToolStripMenuItem.Click += new System.EventHandler(this.SendComment_Click);
			// 
			// reportABugToolStripMenuItem
			// 
			this.reportABugToolStripMenuItem.Name = "reportABugToolStripMenuItem";
			this.reportABugToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.reportABugToolStripMenuItem.Text = "Report a bug";
			this.reportABugToolStripMenuItem.Click += new System.EventHandler(this.ReportABug_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(183, 6);
			// 
			// _purchaseFullVersionToolStripMenuItem
			// 
			this._purchaseFullVersionToolStripMenuItem.Name = "_purchaseFullVersionToolStripMenuItem";
			this._purchaseFullVersionToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this._purchaseFullVersionToolStripMenuItem.Text = "Purchase Full Version";
			this._purchaseFullVersionToolStripMenuItem.Click += new System.EventHandler(this.PurchaseFullVersion_Click);
			// 
			// _aboutToolStripMenuItem
			// 
			this._aboutToolStripMenuItem.Name = "_aboutToolStripMenuItem";
			this._aboutToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this._aboutToolStripMenuItem.Text = "About";
			this._aboutToolStripMenuItem.Click += new System.EventHandler(this.About_Click);
			// 
			// _statusStrip
			// 
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel});
			this._statusStrip.Location = new System.Drawing.Point(0, 393);
			this._statusStrip.Name = "_statusStrip";
			this._statusStrip.Size = new System.Drawing.Size(664, 22);
			this._statusStrip.TabIndex = 4;
			this._statusStrip.Text = "statusStrip";
			// 
			// _statusLabel
			// 
			this._statusLabel.Name = "_statusLabel";
			this._statusLabel.Size = new System.Drawing.Size(82, 17);
			this._statusLabel.Text = "Design Time...";
			// 
			// _toolStrip
			// 
			this._toolStrip.Location = new System.Drawing.Point(0, 24);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Size = new System.Drawing.Size(664, 25);
			this._toolStrip.TabIndex = 5;
			this._toolStrip.Text = "toolStrip1";
			// 
			// MainForm
			// 
			this.ClientSize = new System.Drawing.Size(664, 415);
			this.Controls.Add(this._toolStrip);
			this.Controls.Add(this._statusStrip);
			this.Controls.Add(this._menuStrip);
			this.Name = "MainForm";
			this._menuStrip.ResumeLayout(false);
			this._menuStrip.PerformLayout();
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _userguideToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _contextHelpToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem _requestAFeatureToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sendACommentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reportABugToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem _purchaseFullVersionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
		protected System.Windows.Forms.MenuStrip _menuStrip;
		protected System.Windows.Forms.StatusStrip _statusStrip;
		protected System.Windows.Forms.ToolStrip _toolStrip;

	



	}
}
