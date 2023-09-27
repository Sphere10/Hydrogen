//-----------------------------------------------------------------------
// <copyright file="ConnectionBarTestForm.Designer.cs" company="Sphere 10 Software">
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
    partial class ConnectionBarTestScreen {
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
            this._testConnectionButton = new System.Windows.Forms.Button();
            this._loadingCircle = new Hydrogen.Windows.Forms.LoadingCircle();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this._databaseConnectionBar = new Hydrogen.Windows.Forms.DatabaseConnectionBar();
            this.SuspendLayout();
            // 
            // _testConnectionButton
            // 
            this._testConnectionButton.Location = new System.Drawing.Point(83, 169);
            this._testConnectionButton.Name = "_testConnectionButton";
            this._testConnectionButton.Size = new System.Drawing.Size(124, 23);
            this._testConnectionButton.TabIndex = 1;
            this._testConnectionButton.Text = "Test Connection";
            this._testConnectionButton.UseVisualStyleBackColor = true;
            this._testConnectionButton.Click += new System.EventHandler(this._testConnectionButton_Click);
            // 
            // _loadingCircle
            // 
            this._loadingCircle.Active = false;
            this._loadingCircle.BackColor = System.Drawing.Color.Transparent;
            this._loadingCircle.Color = System.Drawing.Color.DarkGray;
            this._loadingCircle.InnerCircleRadius = 8;
            this._loadingCircle.Location = new System.Drawing.Point(213, 169);
            this._loadingCircle.Name = "_loadingCircle";
            this._loadingCircle.NumberSpoke = 10;
            this._loadingCircle.OuterCircleRadius = 10;
            this._loadingCircle.RotationSpeed = 100;
            this._loadingCircle.Size = new System.Drawing.Size(41, 31);
            this._loadingCircle.SpokeThickness = 4;
            this._loadingCircle.TabIndex = 2;
            this._loadingCircle.Text = "loadingCircle1";
            this._loadingCircle.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(689, 223);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "&Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(589, 223);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "&Save";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // _databaseConnectionBar
            // 
            this._databaseConnectionBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._databaseConnectionBar.ArtificialKeysFile = null;
            this._databaseConnectionBar.Location = new System.Drawing.Point(12, 12);
            this._databaseConnectionBar.Margin = new System.Windows.Forms.Padding(0);
            this._databaseConnectionBar.MinimumSize = new System.Drawing.Size(500, 40);
            this._databaseConnectionBar.Name = "_databaseConnectionBar";
            this._databaseConnectionBar.SelectedDBMSType = Hydrogen.Data.DBMSType.SQLServer;
            this._databaseConnectionBar.Size = new System.Drawing.Size(771, 40);
            this._databaseConnectionBar.TabIndex = 5;
            // 
            // ConnectionBarTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 258);
            this.Controls.Add(this._databaseConnectionBar);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._loadingCircle);
            this.Controls.Add(this._testConnectionButton);
            this.Name = "ConnectionBarTestScreen";
            this.Text = "Connection Settings";
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Button _testConnectionButton;
		private Hydrogen.Windows.Forms.LoadingCircle _loadingCircle;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
        private Hydrogen.Windows.Forms.DatabaseConnectionBar _databaseConnectionBar;
	}
}
