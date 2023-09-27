//-----------------------------------------------------------------------
// <copyright file="MiscTestForm.Designer.cs" company="Sphere 10 Software">
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
			comboBox1 = new System.Windows.Forms.ComboBox();
			button1 = new System.Windows.Forms.Button();
			_clipTestButton = new System.Windows.Forms.Button();
			_scopedContextTestButton = new System.Windows.Forms.Button();
			_outputTextBox = new Windows.Forms.TextBoxEx();
			listMerger1 = new Windows.Forms.ListMerger();
			_compressTestButton = new System.Windows.Forms.Button();
			_pathTemplatesButton = new System.Windows.Forms.Button();
			_sqliteTestButton = new System.Windows.Forms.Button();
			button2 = new System.Windows.Forms.Button();
			_sqlServerTest = new System.Windows.Forms.Button();
			progressBarEx1 = new Windows.Forms.ProgressBarEx();
			button3 = new System.Windows.Forms.Button();
			_systemPathsButton = new System.Windows.Forms.Button();
			_macAddressesButton = new System.Windows.Forms.Button();
			_mappingTests = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// comboBox1
			// 
			comboBox1.FormattingEnabled = true;
			comboBox1.Location = new System.Drawing.Point(14, 14);
			comboBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			comboBox1.Name = "comboBox1";
			comboBox1.Size = new System.Drawing.Size(140, 23);
			comboBox1.TabIndex = 0;
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(329, 248);
			button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(88, 27);
			button1.TabIndex = 2;
			button1.Text = "button1";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// _clipTestButton
			// 
			_clipTestButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_clipTestButton.Location = new System.Drawing.Point(38, 549);
			_clipTestButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_clipTestButton.Name = "_clipTestButton";
			_clipTestButton.Size = new System.Drawing.Size(88, 27);
			_clipTestButton.TabIndex = 4;
			_clipTestButton.Text = "Clip Test";
			_clipTestButton.UseVisualStyleBackColor = true;
			_clipTestButton.Click += _clipTestButton_Click;
			// 
			// _scopedContextTestButton
			// 
			_scopedContextTestButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_scopedContextTestButton.Location = new System.Drawing.Point(133, 549);
			_scopedContextTestButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_scopedContextTestButton.Name = "_scopedContextTestButton";
			_scopedContextTestButton.Size = new System.Drawing.Size(134, 27);
			_scopedContextTestButton.TabIndex = 5;
			_scopedContextTestButton.Text = "Scoped Context Test";
			_scopedContextTestButton.UseVisualStyleBackColor = true;
			_scopedContextTestButton.Click += _scopedContextTestButton_Click;
			// 
			// _outputTextBox
			// 
			_outputTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_outputTextBox.Location = new System.Drawing.Point(38, 293);
			_outputTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_outputTextBox.Multiline = true;
			_outputTextBox.Name = "_outputTextBox";
			_outputTextBox.Size = new System.Drawing.Size(1109, 252);
			_outputTextBox.TabIndex = 3;
			// 
			// listMerger1
			// 
			listMerger1.DisplayMember = "";
			listMerger1.LeftHeader = "_leftHeaderLabel";
			listMerger1.Location = new System.Drawing.Point(329, 63);
			listMerger1.Margin = new System.Windows.Forms.Padding(8);
			listMerger1.Name = "listMerger1";
			listMerger1.RightHeader = "_rightHeaderLabel";
			listMerger1.Size = new System.Drawing.Size(468, 178);
			listMerger1.TabIndex = 1;
			listMerger1.ValueMember = "";
			// 
			// _compressTestButton
			// 
			_compressTestButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_compressTestButton.Location = new System.Drawing.Point(275, 549);
			_compressTestButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_compressTestButton.Name = "_compressTestButton";
			_compressTestButton.Size = new System.Drawing.Size(109, 27);
			_compressTestButton.TabIndex = 7;
			_compressTestButton.Text = "Compress String";
			_compressTestButton.UseVisualStyleBackColor = true;
			_compressTestButton.Click += _compressTestButton_Click;
			// 
			// _pathTemplatesButton
			// 
			_pathTemplatesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_pathTemplatesButton.Location = new System.Drawing.Point(392, 549);
			_pathTemplatesButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_pathTemplatesButton.Name = "_pathTemplatesButton";
			_pathTemplatesButton.Size = new System.Drawing.Size(104, 27);
			_pathTemplatesButton.TabIndex = 8;
			_pathTemplatesButton.Text = "Path Templates";
			_pathTemplatesButton.UseVisualStyleBackColor = true;
			_pathTemplatesButton.Click += _pathTemplatesButton_Click;
			// 
			// _sqliteTestButton
			// 
			_sqliteTestButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_sqliteTestButton.Location = new System.Drawing.Point(504, 549);
			_sqliteTestButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_sqliteTestButton.Name = "_sqliteTestButton";
			_sqliteTestButton.Size = new System.Drawing.Size(84, 27);
			_sqliteTestButton.TabIndex = 9;
			_sqliteTestButton.Text = "Sqlite Test";
			_sqliteTestButton.UseVisualStyleBackColor = true;
			_sqliteTestButton.Click += _sqliteTestButton_Click;
			// 
			// button2
			// 
			button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			button2.Location = new System.Drawing.Point(596, 549);
			button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(82, 27);
			button2.TabIndex = 10;
			button2.Text = "Sqlite Test 2";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// _sqlServerTest
			// 
			_sqlServerTest.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_sqlServerTest.Location = new System.Drawing.Point(686, 549);
			_sqlServerTest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_sqlServerTest.Name = "_sqlServerTest";
			_sqlServerTest.Size = new System.Drawing.Size(104, 27);
			_sqlServerTest.TabIndex = 11;
			_sqlServerTest.Text = "SQL Server Test";
			_sqlServerTest.UseVisualStyleBackColor = true;
			_sqlServerTest.Click += _sqlServerTest_Click;
			// 
			// progressBarEx1
			// 
			progressBarEx1.CustomText = "sadfasdfasdf";
			progressBarEx1.DisplayStyle = Windows.Forms.ProgressBarDisplayText.CustomText;
			progressBarEx1.Location = new System.Drawing.Point(78, 204);
			progressBarEx1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			progressBarEx1.Name = "progressBarEx1";
			progressBarEx1.Size = new System.Drawing.Size(217, 27);
			progressBarEx1.TabIndex = 12;
			progressBarEx1.Value = 50;
			// 
			// button3
			// 
			button3.Location = new System.Drawing.Point(1018, 260);
			button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(88, 27);
			button3.TabIndex = 13;
			button3.Text = "button3";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// _systemPathsButton
			// 
			_systemPathsButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_systemPathsButton.Location = new System.Drawing.Point(798, 549);
			_systemPathsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_systemPathsButton.Name = "_systemPathsButton";
			_systemPathsButton.Size = new System.Drawing.Size(97, 27);
			_systemPathsButton.TabIndex = 14;
			_systemPathsButton.Text = "System Paths";
			_systemPathsButton.UseVisualStyleBackColor = true;
			_systemPathsButton.Click += _systemPathsButton_Click;
			// 
			// _macAddressesButton
			// 
			_macAddressesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_macAddressesButton.Location = new System.Drawing.Point(903, 549);
			_macAddressesButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_macAddressesButton.Name = "_macAddressesButton";
			_macAddressesButton.Size = new System.Drawing.Size(102, 27);
			_macAddressesButton.TabIndex = 15;
			_macAddressesButton.Text = "MAC Addresses";
			_macAddressesButton.UseVisualStyleBackColor = true;
			_macAddressesButton.Click += _macAddressesButton_Click;
			// 
			// _mappingTests
			// 
			_mappingTests.Location = new System.Drawing.Point(38, 260);
			_mappingTests.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_mappingTests.Name = "_mappingTests";
			_mappingTests.Size = new System.Drawing.Size(102, 27);
			_mappingTests.TabIndex = 16;
			_mappingTests.Text = "Mapping Tests";
			_mappingTests.UseVisualStyleBackColor = true;
			_mappingTests.Click += _mappingTests_Click;
			// 
			// MiscTestScreen
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(_mappingTests);
			Controls.Add(_macAddressesButton);
			Controls.Add(_systemPathsButton);
			Controls.Add(button3);
			Controls.Add(progressBarEx1);
			Controls.Add(_sqlServerTest);
			Controls.Add(button2);
			Controls.Add(_sqliteTestButton);
			Controls.Add(_pathTemplatesButton);
			Controls.Add(_compressTestButton);
			Controls.Add(_scopedContextTestButton);
			Controls.Add(_clipTestButton);
			Controls.Add(_outputTextBox);
			Controls.Add(button1);
			Controls.Add(listMerger1);
			Controls.Add(comboBox1);
			Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			Name = "MiscTestScreen";
			Size = new System.Drawing.Size(1162, 590);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox1;
		private Hydrogen.Windows.Forms.ListMerger listMerger1;
		private System.Windows.Forms.Button button1;
		private Hydrogen.Windows.Forms.TextBoxEx _outputTextBox;
		private System.Windows.Forms.Button _clipTestButton;
		private System.Windows.Forms.Button _scopedContextTestButton;
		private System.Windows.Forms.Button _compressTestButton;
		private System.Windows.Forms.Button _pathTemplatesButton;
		private System.Windows.Forms.Button _sqliteTestButton;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button _sqlServerTest;
		private Hydrogen.Windows.Forms.ProgressBarEx progressBarEx1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button _systemPathsButton;
		private System.Windows.Forms.Button _macAddressesButton;
		private System.Windows.Forms.Button _mappingTests;
	}
}
