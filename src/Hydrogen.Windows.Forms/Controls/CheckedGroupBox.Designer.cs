//-----------------------------------------------------------------------
// <copyright file="CheckedGroupBox.Designer.cs" company="Sphere 10 Software">
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
	partial class CheckedGroupBox {
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

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.m_checkBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// m_checkBox
			// 
			this.m_checkBox.AutoSize = true;
			this.m_checkBox.Checked = true;
			this.m_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_checkBox.Location = new System.Drawing.Point(0, 0);
			this.m_checkBox.Name = "m_checkBox";
			this.m_checkBox.Size = new System.Drawing.Size(104, 24);
			this.m_checkBox.TabIndex = 0;
			this.m_checkBox.Text = "checkBox";
			this.m_checkBox.UseVisualStyleBackColor = true;
			this.m_checkBox.CheckStateChanged += new System.EventHandler(this.checkBox_CheckStateChanged);
			this.m_checkBox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// CheckGroupBox
			// 
			this.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.CheckGroupBox_ControlAdded);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.CheckBox m_checkBox;
	}
}
