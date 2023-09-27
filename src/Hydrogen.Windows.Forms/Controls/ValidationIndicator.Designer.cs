// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
	partial class ValidationIndicator {
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
            this._loadingCircle = new Hydrogen.Windows.Forms.LoadingCircle();
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // _loadingCircle
            // 
            this._loadingCircle.Active = false;
            this._loadingCircle.BackColor = System.Drawing.Color.Transparent;
            this._loadingCircle.Color = System.Drawing.Color.DarkGray;
            this._loadingCircle.Dock = System.Windows.Forms.DockStyle.Fill;
            this._loadingCircle.InnerCircleRadius = 7;
            this._loadingCircle.Location = new System.Drawing.Point(0, 0);
            this._loadingCircle.Margin = new System.Windows.Forms.Padding(0);
            this._loadingCircle.Name = "_loadingCircle";
            this._loadingCircle.NumberSpoke = 10;
            this._loadingCircle.OuterCircleRadius = 9;
            this._loadingCircle.RotationSpeed = 100;
            this._loadingCircle.Size = new System.Drawing.Size(20, 20);
            this._loadingCircle.SpokeThickness = 4;
            this._loadingCircle.TabIndex = 0;
            this._loadingCircle.Visible = false;
            this._loadingCircle.MouseHover += new System.EventHandler(this._MouseHover);
            // 
            // ValidationIndicator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Hydrogen.Windows.Forms.Resources.Tick;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this._loadingCircle);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ValidationIndicator";
            this.Size = new System.Drawing.Size(20, 20);
            this.MouseHover += new System.EventHandler(this._MouseHover);
            this.ResumeLayout(false);

		}

		#endregion

		private LoadingCircle _loadingCircle;
		private System.Windows.Forms.ToolTip _toolTip;
	}
}
