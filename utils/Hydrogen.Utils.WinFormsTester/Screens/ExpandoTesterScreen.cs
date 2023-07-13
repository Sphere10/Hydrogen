// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Windows.Forms;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class ExpandoTesterScreen : ApplicationScreen {
	private TaskPane taskPane1;
	private Expando expando1;
	private TaskItem taskItem1;
	private TaskItem taskItem2;
	private Expando expando2;
	private TaskItem taskItem5;
	private CheckBox checkBox1;
	private TaskItem taskItem3;
	private TreeView treeView1;
	private Expando expando3;


	public ExpandoTesterScreen() {
		InitializeComponent();
	}

	private void InitializeComponent() {
		this.taskPane1 = new TaskPane();
		this.expando1 = new Expando();
		this.taskItem1 = new TaskItem();
		this.taskItem2 = new TaskItem();
		this.expando2 = new Expando();
		this.taskItem5 = new TaskItem();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.taskItem3 = new TaskItem();
		this.treeView1 = new System.Windows.Forms.TreeView();
		this.expando3 = new Expando();
		((System.ComponentModel.ISupportInitialize)(this.taskPane1)).BeginInit();
		this.taskPane1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this.expando1)).BeginInit();
		this.expando1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this.expando2)).BeginInit();
		this.expando2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this.expando3)).BeginInit();
		this.SuspendLayout();
		// 
		// taskPane1
		// 
		this.taskPane1.AutoScrollMargin = new System.Drawing.Size(12, 12);
		this.taskPane1.Expandos.AddRange(new Expando[] {
			this.expando1,
			this.expando2,
			this.expando3
		});
		this.taskPane1.Location = new System.Drawing.Point(12, 12);
		this.taskPane1.Name = "taskPane1";
		this.taskPane1.Size = new System.Drawing.Size(212, 470);
		this.taskPane1.TabIndex = 0;
		this.taskPane1.Text = "taskPane1";
		// 
		// expando1
		// 
		this.expando1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
		                                                             | System.Windows.Forms.AnchorStyles.Right)));
		this.expando1.Animate = true;
		this.expando1.Font = new System.Drawing.Font("Tahoma", 8.25F);
		this.expando1.Items.AddRange(new System.Windows.Forms.Control[] {
			this.taskItem1,
			this.taskItem2
		});
		this.expando1.Location = new System.Drawing.Point(12, 12);
		this.expando1.Name = "expando1";
		this.expando1.Size = new System.Drawing.Size(188, 100);
		this.expando1.TabIndex = 0;
		this.expando1.Text = "expando1";
		// 
		// taskItem1
		// 
		this.taskItem1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
		                                                              | System.Windows.Forms.AnchorStyles.Right)));
		this.taskItem1.BackColor = System.Drawing.Color.Transparent;
		this.taskItem1.Image = null;
		this.taskItem1.Location = new System.Drawing.Point(20, 26);
		this.taskItem1.Name = "taskItem1";
		this.taskItem1.Size = new System.Drawing.Size(164, 16);
		this.taskItem1.TabIndex = 0;
		this.taskItem1.Text = "taskItem1";
		this.taskItem1.TextAlign = System.Drawing.ContentAlignment.TopLeft;
		this.taskItem1.UseVisualStyleBackColor = false;
		// 
		// taskItem2
		// 
		this.taskItem2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
		                                                              | System.Windows.Forms.AnchorStyles.Right)));
		this.taskItem2.BackColor = System.Drawing.Color.Transparent;
		this.taskItem2.Image = null;
		this.taskItem2.Location = new System.Drawing.Point(20, 48);
		this.taskItem2.Name = "taskItem2";
		this.taskItem2.Size = new System.Drawing.Size(164, 16);
		this.taskItem2.TabIndex = 1;
		this.taskItem2.Text = "taskItem2";
		this.taskItem2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
		this.taskItem2.UseVisualStyleBackColor = false;
		// 
		// expando2
		// 
		this.expando2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
		                                                             | System.Windows.Forms.AnchorStyles.Right)));
		this.expando2.Animate = true;
		this.expando2.AutoLayout = true;
		this.expando2.ExpandedHeight = 209;
		this.expando2.Font = new System.Drawing.Font("Tahoma", 8.25F);
		this.expando2.Items.AddRange(new System.Windows.Forms.Control[] {
			this.taskItem5,
			this.checkBox1,
			this.taskItem3,
			this.treeView1
		});
		this.expando2.Location = new System.Drawing.Point(12, 124);
		this.expando2.Name = "expando2";
		this.expando2.Size = new System.Drawing.Size(188, 209);
		this.expando2.TabIndex = 1;
		this.expando2.Text = "expando2";
		// 
		// taskItem5
		// 
		this.taskItem5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
		                                                              | System.Windows.Forms.AnchorStyles.Right)));
		this.taskItem5.BackColor = System.Drawing.Color.Transparent;
		this.taskItem5.Image = null;
		this.taskItem5.Location = new System.Drawing.Point(12, 33);
		this.taskItem5.Name = "taskItem5";
		this.taskItem5.Size = new System.Drawing.Size(162, 16);
		this.taskItem5.TabIndex = 2;
		this.taskItem5.Text = "taskItem5";
		this.taskItem5.TextAlign = System.Drawing.ContentAlignment.TopLeft;
		this.taskItem5.UseVisualStyleBackColor = false;
		// 
		// checkBox1
		// 
		this.checkBox1.Location = new System.Drawing.Point(12, 53);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(104, 24);
		this.checkBox1.TabIndex = 3;
		this.checkBox1.Text = "checkBox1";
		this.checkBox1.UseVisualStyleBackColor = true;
		// 
		// taskItem3
		// 
		this.taskItem3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
		                                                              | System.Windows.Forms.AnchorStyles.Right)));
		this.taskItem3.BackColor = System.Drawing.Color.Transparent;
		this.taskItem3.Image = null;
		this.taskItem3.Location = new System.Drawing.Point(12, 81);
		this.taskItem3.Name = "taskItem3";
		this.taskItem3.Size = new System.Drawing.Size(162, 16);
		this.taskItem3.TabIndex = 4;
		this.taskItem3.Text = "taskItem3";
		this.taskItem3.TextAlign = System.Drawing.ContentAlignment.TopLeft;
		this.taskItem3.UseVisualStyleBackColor = false;
		// 
		// treeView1
		// 
		this.treeView1.Location = new System.Drawing.Point(12, 101);
		this.treeView1.Name = "treeView1";
		this.treeView1.Size = new System.Drawing.Size(121, 97);
		this.treeView1.TabIndex = 5;
		// 
		// expando3
		// 
		this.expando3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
		                                                             | System.Windows.Forms.AnchorStyles.Right)));
		this.expando3.Animate = true;
		this.expando3.Font = new System.Drawing.Font("Tahoma", 8.25F);
		this.expando3.Location = new System.Drawing.Point(12, 345);
		this.expando3.Name = "expando3";
		this.expando3.Size = new System.Drawing.Size(188, 100);
		this.expando3.TabIndex = 2;
		this.expando3.Text = "expando3";
		// 
		// Form1
		// 
		this.ClientSize = new System.Drawing.Size(615, 558);
		this.Controls.Add(this.taskPane1);
		this.Name = "Form1";
		((System.ComponentModel.ISupportInitialize)(this.taskPane1)).EndInit();
		this.taskPane1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this.expando1)).EndInit();
		this.expando1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this.expando2)).EndInit();
		this.expando2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this.expando3)).EndInit();
		this.ResumeLayout(false);

	}


}
