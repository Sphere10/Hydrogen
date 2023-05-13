// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class ConnectionBarBase {
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
            this.components = new System.ComponentModel.Container();
            this.OptionsButton = new System.Windows.Forms.Button();
            this.DatabaseOptionsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._artificialKeysMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DatabaseOptionsContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // OptionsButton
            // 
            this.OptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OptionsButton.Image = global::Hydrogen.Windows.Forms.Resources.NORMALGROUPEXPAND;
            this.OptionsButton.Location = new System.Drawing.Point(463, 15);
            this.OptionsButton.Name = "OptionsButton";
            this.OptionsButton.Size = new System.Drawing.Size(28, 23);
            this.OptionsButton.TabIndex = 0;
            this.OptionsButton.UseVisualStyleBackColor = true;
            this.OptionsButton.Click += new System.EventHandler(this.OptionsButton_Click);
            // 
            // DatabaseOptionsContextMenu
            // 
            this.DatabaseOptionsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._artificialKeysMenuItem});
            this.DatabaseOptionsContextMenu.Name = "DatabaseOptionsContextMenu";
            this.DatabaseOptionsContextMenu.Size = new System.Drawing.Size(146, 26);
            // 
            // _artificialKeysMenuItem
            // 
            this._artificialKeysMenuItem.Name = "_artificialKeysMenuItem";
            this._artificialKeysMenuItem.Size = new System.Drawing.Size(145, 22);
            this._artificialKeysMenuItem.Text = "Artificial Keys";
            this._artificialKeysMenuItem.Click += new System.EventHandler(this._artificialKeysMenuItem_Click);
            // 
            // ConnectionBarBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.OptionsButton);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ConnectionBarBase";
            this.Size = new System.Drawing.Size(491, 37);
            this.DatabaseOptionsContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.Button OptionsButton;
		private System.Windows.Forms.ToolStripMenuItem _artificialKeysMenuItem;
		protected System.Windows.Forms.ContextMenuStrip DatabaseOptionsContextMenu;


	}
}
