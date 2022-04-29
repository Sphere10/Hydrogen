//-----------------------------------------------------------------------
// <copyright file="MiscTestForm.Designer.cs" company="Sphere 10 Software">
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

namespace Sphere10.FrameworkTester.WinForms {
	partial class MiscTestScreen {
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
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.button1 = new System.Windows.Forms.Button();
			this._clipTestButton = new System.Windows.Forms.Button();
			this._scopedContextTestButton = new System.Windows.Forms.Button();
			this._clipTestTextBox = new Sphere10.Framework.Windows.Forms.TextBoxEx();
			this.listMerger1 = new Sphere10.Framework.Windows.Forms.ListMerger();
			this._compressTestButton = new System.Windows.Forms.Button();
			this._pathTemplatesButton = new System.Windows.Forms.Button();
			this._sqliteTestButton = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this._sqlServerTest = new System.Windows.Forms.Button();
			this.progressBarEx1 = new Sphere10.Framework.Windows.Forms.ProgressBarEx();
			this.button3 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(12, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 21);
			this.comboBox1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(282, 215);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// _clipTestButton
			// 
			this._clipTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._clipTestButton.Location = new System.Drawing.Point(33, 476);
			this._clipTestButton.Name = "_clipTestButton";
			this._clipTestButton.Size = new System.Drawing.Size(75, 23);
			this._clipTestButton.TabIndex = 4;
			this._clipTestButton.Text = "Clip Test";
			this._clipTestButton.UseVisualStyleBackColor = true;
			this._clipTestButton.Click += new System.EventHandler(this._clipTestButton_Click);
			// 
			// _scopedContextTestButton
			// 
			this._scopedContextTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._scopedContextTestButton.Location = new System.Drawing.Point(114, 476);
			this._scopedContextTestButton.Name = "_scopedContextTestButton";
			this._scopedContextTestButton.Size = new System.Drawing.Size(148, 23);
			this._scopedContextTestButton.TabIndex = 5;
			this._scopedContextTestButton.Text = "Scoped Context Test";
			this._scopedContextTestButton.UseVisualStyleBackColor = true;
			this._scopedContextTestButton.Click += new System.EventHandler(this._scopedContextTestButton_Click);
			// 
			// _clipTestTextBox
			// 
			this._clipTestTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._clipTestTextBox.Location = new System.Drawing.Point(33, 254);
			this._clipTestTextBox.Multiline = true;
			this._clipTestTextBox.Name = "_clipTestTextBox";
			this._clipTestTextBox.Size = new System.Drawing.Size(915, 219);
			this._clipTestTextBox.TabIndex = 3;
			// 
			// listMerger1
			// 
			this.listMerger1.DisplayMember = "";
			this.listMerger1.LeftHeader = "_leftHeaderLabel";
			this.listMerger1.Location = new System.Drawing.Point(282, 55);
			this.listMerger1.Name = "listMerger1";
			this.listMerger1.RightHeader = "_rightHeaderLabel";
			this.listMerger1.Size = new System.Drawing.Size(401, 154);
			this.listMerger1.TabIndex = 1;
			this.listMerger1.ValueMember = "";
			// 
			// _compressTestButton
			// 
			this._compressTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._compressTestButton.Location = new System.Drawing.Point(268, 476);
			this._compressTestButton.Name = "_compressTestButton";
			this._compressTestButton.Size = new System.Drawing.Size(148, 23);
			this._compressTestButton.TabIndex = 7;
			this._compressTestButton.Text = "Compress String";
			this._compressTestButton.UseVisualStyleBackColor = true;
			this._compressTestButton.Click += new System.EventHandler(this._compressTestButton_Click);
			// 
			// _pathTemplatesButton
			// 
			this._pathTemplatesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._pathTemplatesButton.Location = new System.Drawing.Point(422, 476);
			this._pathTemplatesButton.Name = "_pathTemplatesButton";
			this._pathTemplatesButton.Size = new System.Drawing.Size(148, 23);
			this._pathTemplatesButton.TabIndex = 8;
			this._pathTemplatesButton.Text = "Path Templates";
			this._pathTemplatesButton.UseVisualStyleBackColor = true;
			this._pathTemplatesButton.Click += new System.EventHandler(this._pathTemplatesButton_Click);
			// 
			// _sqliteTestButton
			// 
			this._sqliteTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._sqliteTestButton.Location = new System.Drawing.Point(576, 476);
			this._sqliteTestButton.Name = "_sqliteTestButton";
			this._sqliteTestButton.Size = new System.Drawing.Size(107, 23);
			this._sqliteTestButton.TabIndex = 9;
			this._sqliteTestButton.Text = "Sqlite Test";
			this._sqliteTestButton.UseVisualStyleBackColor = true;
			this._sqliteTestButton.Click += new System.EventHandler(this._sqliteTestButton_Click);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button2.Location = new System.Drawing.Point(689, 476);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(94, 23);
			this.button2.TabIndex = 10;
			this.button2.Text = "Sqlite Test 2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// _sqlServerTest
			// 
			this._sqlServerTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._sqlServerTest.Location = new System.Drawing.Point(789, 476);
			this._sqlServerTest.Name = "_sqlServerTest";
			this._sqlServerTest.Size = new System.Drawing.Size(113, 23);
			this._sqlServerTest.TabIndex = 11;
			this._sqlServerTest.Text = "SQL Server Test";
			this._sqlServerTest.UseVisualStyleBackColor = true;
			this._sqlServerTest.Click += new System.EventHandler(this._sqlServerTest_Click);
			// 
			// progressBarEx1
			// 
			this.progressBarEx1.CustomText = "sadfasdfasdf";
			this.progressBarEx1.DisplayStyle = Sphere10.Framework.Windows.Forms.ProgressBarDisplayText.CustomText;
			this.progressBarEx1.Location = new System.Drawing.Point(67, 177);
			this.progressBarEx1.Name = "progressBarEx1";
			this.progressBarEx1.Size = new System.Drawing.Size(186, 23);
			this.progressBarEx1.TabIndex = 12;
			this.progressBarEx1.Value = 50;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(873, 225);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 13;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// MiscTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(960, 511);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.progressBarEx1);
			this.Controls.Add(this._sqlServerTest);
			this.Controls.Add(this.button2);
			this.Controls.Add(this._sqliteTestButton);
			this.Controls.Add(this._pathTemplatesButton);
			this.Controls.Add(this._compressTestButton);
			this.Controls.Add(this._scopedContextTestButton);
			this.Controls.Add(this._clipTestButton);
			this.Controls.Add(this._clipTestTextBox);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.listMerger1);
			this.Controls.Add(this.comboBox1);
			this.Name = "MiscTestScreen";
			this.Text = "MiscTestForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox1;
		private Sphere10.Framework.Windows.Forms.ListMerger listMerger1;
		private System.Windows.Forms.Button button1;
        private Sphere10.Framework.Windows.Forms.TextBoxEx _clipTestTextBox;
        private System.Windows.Forms.Button _clipTestButton;
        private System.Windows.Forms.Button _scopedContextTestButton;
        private System.Windows.Forms.Button _compressTestButton;
        private System.Windows.Forms.Button _pathTemplatesButton;
        private System.Windows.Forms.Button _sqliteTestButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button _sqlServerTest;
        private Sphere10.Framework.Windows.Forms.ProgressBarEx progressBarEx1;
		private System.Windows.Forms.Button button3;
	}
}
