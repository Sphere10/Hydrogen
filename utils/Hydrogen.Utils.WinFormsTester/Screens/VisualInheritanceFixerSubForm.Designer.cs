//-----------------------------------------------------------------------
// <copyright file="VisualInheritanceFixerSub.Designer.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using Hydrogen.Windows.Forms;


namespace Hydrogen.Utils.WinFormsTester {
	partial class VisualInheritanceFixerSubForm {
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
			this.cToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mergeableMenuStrip2 = new Hydrogen.Windows.Forms.MergeableMenuStrip();
			this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.aAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.mergeableStatusStrip1 = new Hydrogen.Windows.Forms.MergeableStatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.mergeableToolStrip1 = new Hydrogen.Windows.Forms.MergeableToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.mergeableMenuStrip2.SuspendLayout();
			this.mergeableStatusStrip1.SuspendLayout();
			this.mergeableToolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cToolStripMenuItem
			// 
			this.cToolStripMenuItem.Name = "cToolStripMenuItem";
			this.cToolStripMenuItem.Size = new System.Drawing.Size(27, 20);
			this.cToolStripMenuItem.Text = "C";
			// 
			// mergeableMenuStrip2
			// 
			this.mergeableMenuStrip2.InheritedToolStrip = this.menuStrip1;
			this.mergeableMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aToolStripMenuItem,
            this.aAToolStripMenuItem,
            this.cToolStripMenuItem1,
            this.bToolStripMenuItem});
			this.mergeableMenuStrip2.Location = new System.Drawing.Point(0, 49);
			this.mergeableMenuStrip2.Name = "mergeableMenuStrip2";
			this.mergeableMenuStrip2.Size = new System.Drawing.Size(531, 24);
			this.mergeableMenuStrip2.TabIndex = 3;
			this.mergeableMenuStrip2.Text = "mergeableMenuStrip2";
			this.mergeableMenuStrip2.Visible = false;
			// 
			// aToolStripMenuItem
			// 
			this.aToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
			this.aToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.aToolStripMenuItem.Name = "aToolStripMenuItem";
			this.aToolStripMenuItem.Size = new System.Drawing.Size(27, 20);
			this.aToolStripMenuItem.Text = "A";
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.MergeAction = System.Windows.Forms.MergeAction.Remove;
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem2.Text = "22";
			// 
			// aAToolStripMenuItem
			// 
			this.aAToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Remove;
			this.aAToolStripMenuItem.Name = "aAToolStripMenuItem";
			this.aAToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.aAToolStripMenuItem.Text = "AA";
			// 
			// cToolStripMenuItem1
			// 
			this.cToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
			this.cToolStripMenuItem1.Name = "cToolStripMenuItem1";
			this.cToolStripMenuItem1.Size = new System.Drawing.Size(27, 20);
			this.cToolStripMenuItem1.Text = "C";
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem3.Text = "1";
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem4.Text = "2";
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem5.Text = "3";
			// 
			// bToolStripMenuItem
			// 
			this.bToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem7});
			this.bToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.bToolStripMenuItem.Name = "bToolStripMenuItem";
			this.bToolStripMenuItem.Size = new System.Drawing.Size(26, 20);
			this.bToolStripMenuItem.Text = "B";
			// 
			// bToolStripMenuItem1
			// 
			this.bToolStripMenuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.bToolStripMenuItem1.MergeIndex = 1;
			this.bToolStripMenuItem1.Name = "bToolStripMenuItem1";
			this.bToolStripMenuItem1.Size = new System.Drawing.Size(81, 22);
			this.bToolStripMenuItem1.Text = "B";
			// 
			// mergeableStatusStrip1
			// 
			this.mergeableStatusStrip1.InheritedToolStrip = this.statusStrip1;
			this.mergeableStatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.mergeableStatusStrip1.Location = new System.Drawing.Point(0, 234);
			this.mergeableStatusStrip1.Name = "mergeableStatusStrip1";
			this.mergeableStatusStrip1.Size = new System.Drawing.Size(531, 22);
			this.mergeableStatusStrip1.TabIndex = 4;
			this.mergeableStatusStrip1.Text = "mergeableStatusStrip1";
			this.mergeableStatusStrip1.Visible = false;
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripStatusLabel1.MergeIndex = 0;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(13, 17);
			this.toolStripStatusLabel1.Text = "1";
			// 
			// mergeableToolStrip1
			// 
			this.mergeableToolStrip1.InheritedToolStrip = this.toolStrip1;
			this.mergeableToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripLabel2});
			this.mergeableToolStrip1.Location = new System.Drawing.Point(0, 49);
			this.mergeableToolStrip1.Name = "mergeableToolStrip1";
			this.mergeableToolStrip1.Size = new System.Drawing.Size(531, 25);
			this.mergeableToolStrip1.TabIndex = 5;
			this.mergeableToolStrip1.Text = "mergeableToolStrip1";
			this.mergeableToolStrip1.Visible = false;
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.MergeAction = System.Windows.Forms.MergeAction.Remove;
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(19, 22);
			this.toolStripLabel1.Text = "11";
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripLabel2.MergeIndex = 1;
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(13, 22);
			this.toolStripLabel2.Text = "2";
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(81, 22);
			this.toolStripMenuItem6.Text = "2";
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem7.MergeIndex = 1;
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem7.Text = "2";
			// 
			// VisualInheritanceFixerSub
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(531, 278);
			this.Controls.Add(this.mergeableMenuStrip2);
			this.Controls.Add(this.mergeableToolStrip1);
			this.Controls.Add(this.mergeableStatusStrip1);
			this.Name = "VisualInheritanceFixerSub";
			this.Text = "VisualInheritanceFixerSub";
			this.Controls.SetChildIndex(this.mergeableStatusStrip1, 0);
			this.Controls.SetChildIndex(this.mergeableToolStrip1, 0);
			this.Controls.SetChildIndex(this.mergeableMenuStrip2, 0);
			this.mergeableMenuStrip2.ResumeLayout(false);
			this.mergeableMenuStrip2.PerformLayout();
			this.mergeableStatusStrip1.ResumeLayout(false);
			this.mergeableStatusStrip1.PerformLayout();
			this.mergeableToolStrip1.ResumeLayout(false);
			this.mergeableToolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MergeableMenuStrip mergeableMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem cToolStripMenuItem;
		private MergeableMenuStrip mergeableMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem aAToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cToolStripMenuItem1;
		private MergeableStatusStrip mergeableStatusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private MergeableToolStrip mergeableToolStrip1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel2;
		private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;


	}
}
