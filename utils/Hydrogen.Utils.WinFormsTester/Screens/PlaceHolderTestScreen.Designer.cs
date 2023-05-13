//-----------------------------------------------------------------------
// <copyright file="PlaceHolderTestForm.Designer.cs" company="Sphere 10 Software">
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
	partial class PlaceHolderTestScreen {
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
			this.components = new System.ComponentModel.Container();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.searchTextBox2 = new Hydrogen.Windows.Forms.SearchTextBox();
			this.moneyBox1 = new Hydrogen.Windows.Forms.MoneyBox();
			this.textBoxEx1 = new Hydrogen.Windows.Forms.TextBoxEx();
			this.searchTextBox1 = new Hydrogen.Windows.Forms.SearchTextBox();
			this.cueTextExtender1 = new Hydrogen.Windows.Forms.PlaceHolderTextExtender(this.components);
			this.comboBoxEx1 = new Hydrogen.Windows.Forms.ComboBoxEx();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.searchTextBox1);
			this.groupBox1.Controls.Add(this.richTextBox1);
			this.groupBox1.Controls.Add(this.maskedTextBox1);
			this.groupBox1.Controls.Add(this.comboBox1);
			this.groupBox1.Controls.Add(this.textBox1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(615, 84);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Extender";
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(433, 18);
			this.richTextBox1.Name = "richTextBox1";
			this.cueTextExtender1.SetPlaceHolderText(this.richTextBox1, "Rich Text Box");
			this.richTextBox1.Size = new System.Drawing.Size(123, 25);
			this.richTextBox1.TabIndex = 3;
			this.richTextBox1.Text = "";
			// 
			// maskedTextBox1
			// 
			this.maskedTextBox1.Location = new System.Drawing.Point(327, 18);
			this.maskedTextBox1.Name = "maskedTextBox1";
			this.cueTextExtender1.SetPlaceHolderText(this.maskedTextBox1, "Masked Text Box");
			this.maskedTextBox1.Size = new System.Drawing.Size(100, 20);
			this.maskedTextBox1.TabIndex = 2;
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "Item1",
            "Item2",
            "Item3"});
			this.comboBox1.Location = new System.Drawing.Point(186, 18);
			this.comboBox1.Name = "comboBox1";
			this.cueTextExtender1.SetPlaceHolderText(this.comboBox1, "ComboBox");
			this.comboBox1.Size = new System.Drawing.Size(121, 21);
			this.comboBox1.TabIndex = 1;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(6, 19);
			this.textBox1.Name = "textBox1";
			this.cueTextExtender1.SetPlaceHolderText(this.textBox1, "TextBox");
			this.textBox1.Size = new System.Drawing.Size(130, 20);
			this.textBox1.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.comboBoxEx1);
			this.groupBox2.Controls.Add(this.searchTextBox2);
			this.groupBox2.Controls.Add(this.moneyBox1);
			this.groupBox2.Controls.Add(this.textBoxEx1);
			this.groupBox2.Location = new System.Drawing.Point(12, 102);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(615, 172);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Controls";
			// 
			// searchTextBox2
			// 
			this.searchTextBox2.Location = new System.Drawing.Point(420, 19);
			this.searchTextBox2.Name = "searchTextBox2";
			this.cueTextExtender1.SetPlaceHolderText(this.searchTextBox2, "");
			this.searchTextBox2.PlaceHolderText = "Search";
			this.searchTextBox2.Size = new System.Drawing.Size(100, 20);
			this.searchTextBox2.TabIndex = 2;
			// 
			// moneyBox1
			// 
			this.moneyBox1.Location = new System.Drawing.Point(186, 18);
			this.moneyBox1.Name = "moneyBox1";
			this.cueTextExtender1.SetPlaceHolderText(this.moneyBox1, "");
			this.moneyBox1.PlaceHolderText = "MoneyBox PlaceHolder";
			this.moneyBox1.Size = new System.Drawing.Size(145, 20);
			this.moneyBox1.TabIndex = 1;
			// 
			// textBoxEx1
			// 
			this.textBoxEx1.Location = new System.Drawing.Point(6, 19);
			this.textBoxEx1.Name = "textBoxEx1";
			this.cueTextExtender1.SetPlaceHolderText(this.textBoxEx1, "");
			this.textBoxEx1.PlaceHolderText = "Placeholder Property";
			this.textBoxEx1.Size = new System.Drawing.Size(130, 20);
			this.textBoxEx1.TabIndex = 0;
			// 
			// searchTextBox1
			// 
			this.searchTextBox1.Location = new System.Drawing.Point(6, 45);
			this.searchTextBox1.Name = "searchTextBox1";
			this.cueTextExtender1.SetPlaceHolderText(this.searchTextBox1, "SearchTextBox via extender");
			this.searchTextBox1.PlaceHolderText = "Search";
			this.searchTextBox1.Size = new System.Drawing.Size(228, 20);
			this.searchTextBox1.TabIndex = 4;
			// 
			// comboBoxEx1
			// 
			this.comboBoxEx1.FormattingEnabled = true;
			this.comboBoxEx1.Location = new System.Drawing.Point(15, 58);
			this.comboBoxEx1.Name = "comboBoxEx1";
			this.cueTextExtender1.SetPlaceHolderText(this.comboBoxEx1, "");
			this.comboBoxEx1.PlaceHolderText = "ComboBox";
			this.comboBoxEx1.Size = new System.Drawing.Size(121, 21);
			this.comboBoxEx1.TabIndex = 3;
			// 
			// PlaceHolderTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(639, 286);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "PlaceHolderTestScreen";
			this.Text = "CueTestForm";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboBox1;
		private Hydrogen.Windows.Forms.PlaceHolderTextExtender cueTextExtender1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.MaskedTextBox maskedTextBox1;
		private Hydrogen.Windows.Forms.SearchTextBox searchTextBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private Hydrogen.Windows.Forms.SearchTextBox searchTextBox2;
		private Hydrogen.Windows.Forms.MoneyBox moneyBox1;
		private Hydrogen.Windows.Forms.TextBoxEx textBoxEx1;
		private Hydrogen.Windows.Forms.ComboBoxEx comboBoxEx1;

	}
}
