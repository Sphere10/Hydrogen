//-----------------------------------------------------------------------
// <copyright file="ApplicationServicesTester.Designer.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Utils.WinFormsTester {
	partial class ApplicationServicesTestScreen {
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
			this._callTestMethodButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _callTestMethodButton
			// 
			this._callTestMethodButton.Location = new System.Drawing.Point(52, 40);
			this._callTestMethodButton.Name = "_callTestMethodButton";
			this._callTestMethodButton.Size = new System.Drawing.Size(125, 23);
			this._callTestMethodButton.TabIndex = 0;
			this._callTestMethodButton.Text = "Call Test Method";
			this._callTestMethodButton.UseVisualStyleBackColor = true;
			this._callTestMethodButton.Click += new System.EventHandler(this._callTestMethodButton_Click);
			// 
			// OnlineServicesTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this._callTestMethodButton);
			this.Name = "OnlineServicesTester";
			this.Text = "OnlineServicesTester";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _callTestMethodButton;
	}
}
