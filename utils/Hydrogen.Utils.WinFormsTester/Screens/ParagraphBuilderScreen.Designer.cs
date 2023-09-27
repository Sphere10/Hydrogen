//-----------------------------------------------------------------------
// <copyright file="ParagraphBuilderForm.Designer.cs" company="Sphere 10 Software">
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
	partial class ParagraphBuilderScreen {
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
			this._appendSentenceButton = new System.Windows.Forms.Button();
			this._paragraphTextBox = new System.Windows.Forms.TextBox();
			this._sentenceTextBox = new System.Windows.Forms.TextBox();
			this._appendBreakButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _appendSentenceButton
			// 
			this._appendSentenceButton.Location = new System.Drawing.Point(125, 12);
			this._appendSentenceButton.Name = "_appendSentenceButton";
			this._appendSentenceButton.Size = new System.Drawing.Size(107, 23);
			this._appendSentenceButton.TabIndex = 0;
			this._appendSentenceButton.Text = "Append Sentence";
			this._appendSentenceButton.UseVisualStyleBackColor = true;
			this._appendSentenceButton.Click += new System.EventHandler(this._appendSentenceButton_Click);
			// 
			// _paragraphTextBox
			// 
			this._paragraphTextBox.Location = new System.Drawing.Point(12, 41);
			this._paragraphTextBox.Multiline = true;
			this._paragraphTextBox.Name = "_paragraphTextBox";
			this._paragraphTextBox.Size = new System.Drawing.Size(655, 380);
			this._paragraphTextBox.TabIndex = 1;
			// 
			// _sentenceTextBox
			// 
			this._sentenceTextBox.Location = new System.Drawing.Point(238, 14);
			this._sentenceTextBox.Name = "_sentenceTextBox";
			this._sentenceTextBox.Size = new System.Drawing.Size(429, 20);
			this._sentenceTextBox.TabIndex = 2;
			// 
			// _appendBreakButton
			// 
			this._appendBreakButton.Location = new System.Drawing.Point(12, 12);
			this._appendBreakButton.Name = "_appendBreakButton";
			this._appendBreakButton.Size = new System.Drawing.Size(107, 23);
			this._appendBreakButton.TabIndex = 3;
			this._appendBreakButton.Text = "Append Break";
			this._appendBreakButton.UseVisualStyleBackColor = true;
			this._appendBreakButton.Click += new System.EventHandler(this._appendBreakButton_Click);
			// 
			// ParagraphBuilderForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(679, 433);
			this.Controls.Add(this._appendBreakButton);
			this.Controls.Add(this._sentenceTextBox);
			this.Controls.Add(this._paragraphTextBox);
			this.Controls.Add(this._appendSentenceButton);
			this.Name = "ParagraphBuilderScreen";
			this.Text = "Paragraph Builder Tester";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _appendSentenceButton;
		private System.Windows.Forms.TextBox _paragraphTextBox;
		private System.Windows.Forms.TextBox _sentenceTextBox;
		private System.Windows.Forms.Button _appendBreakButton;
	}
}
