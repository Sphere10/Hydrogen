// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
			_menuStrip = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			_userguideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			_contextHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			_requestAFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			sendACommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			reportABugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			_aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			_statusStrip = new System.Windows.Forms.StatusStrip();
			_statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			_toolStrip = new System.Windows.Forms.ToolStrip();
			_menuStrip.SuspendLayout();
			_statusStrip.SuspendLayout();
			SuspendLayout();
			// 
			// _menuStrip
			// 
			_menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem });
			_menuStrip.Location = new System.Drawing.Point(0, 0);
			_menuStrip.Name = "_menuStrip";
			_menuStrip.Size = new System.Drawing.Size(664, 24);
			_menuStrip.TabIndex = 3;
			_menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
			exitToolStripMenuItem.Text = "E&xit";
			exitToolStripMenuItem.Click += Exit_Click;
			// 
			// _userguideToolStripMenuItem
			// 
			_userguideToolStripMenuItem.Name = "_userguideToolStripMenuItem";
			_userguideToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			_userguideToolStripMenuItem.Text = "User Guide";
			_userguideToolStripMenuItem.Click += UserGuide_Click;
			// 
			// _contextHelpToolStripMenuItem
			// 
			_contextHelpToolStripMenuItem.Image = Resources.Help_16x16x32;
			_contextHelpToolStripMenuItem.Name = "_contextHelpToolStripMenuItem";
			_contextHelpToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			_contextHelpToolStripMenuItem.Text = "Context Help";
			_contextHelpToolStripMenuItem.Click += ContextHelp_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
			// 
			// _requestAFeatureToolStripMenuItem
			// 
			_requestAFeatureToolStripMenuItem.Name = "_requestAFeatureToolStripMenuItem";
			_requestAFeatureToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			_requestAFeatureToolStripMenuItem.Text = "Request a feature";
			_requestAFeatureToolStripMenuItem.Click += RequestAFeature_Click;
			// 
			// sendACommentToolStripMenuItem
			// 
			sendACommentToolStripMenuItem.Name = "sendACommentToolStripMenuItem";
			sendACommentToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			sendACommentToolStripMenuItem.Text = "Send a comment";
			sendACommentToolStripMenuItem.Click += SendComment_Click;
			// 
			// reportABugToolStripMenuItem
			// 
			reportABugToolStripMenuItem.Name = "reportABugToolStripMenuItem";
			reportABugToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			reportABugToolStripMenuItem.Text = "Report a bug";
			reportABugToolStripMenuItem.Click += ReportABug_Click;
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(183, 6);
			// 
			// _aboutToolStripMenuItem
			// 
			_aboutToolStripMenuItem.Name = "_aboutToolStripMenuItem";
			_aboutToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			_aboutToolStripMenuItem.Text = "About";
			_aboutToolStripMenuItem.Click += About_Click;
			// 
			// _statusStrip
			// 
			_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _statusLabel });
			_statusStrip.Location = new System.Drawing.Point(0, 393);
			_statusStrip.Name = "_statusStrip";
			_statusStrip.Size = new System.Drawing.Size(664, 22);
			_statusStrip.TabIndex = 4;
			_statusStrip.Text = "statusStrip";
			// 
			// _statusLabel
			// 
			_statusLabel.Name = "_statusLabel";
			_statusLabel.Size = new System.Drawing.Size(81, 17);
			_statusLabel.Text = "Design Time...";
			// 
			// _toolStrip
			// 
			_toolStrip.Location = new System.Drawing.Point(0, 24);
			_toolStrip.Name = "_toolStrip";
			_toolStrip.Size = new System.Drawing.Size(664, 25);
			_toolStrip.TabIndex = 5;
			_toolStrip.Text = "toolStrip1";
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			ClientSize = new System.Drawing.Size(664, 415);
			Controls.Add(_toolStrip);
			Controls.Add(_statusStrip);
			Controls.Add(_menuStrip);
			Name = "MainForm";
			_menuStrip.ResumeLayout(false);
			_menuStrip.PerformLayout();
			_statusStrip.ResumeLayout(false);
			_statusStrip.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _userguideToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _contextHelpToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem _requestAFeatureToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sendACommentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reportABugToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem _aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
		protected System.Windows.Forms.MenuStrip _menuStrip;
		protected System.Windows.Forms.StatusStrip _statusStrip;
		protected System.Windows.Forms.ToolStrip _toolStrip;
		
	}
}
