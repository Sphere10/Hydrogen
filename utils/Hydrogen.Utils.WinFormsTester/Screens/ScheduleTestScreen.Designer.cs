//-----------------------------------------------------------------------
// <copyright file="ScheduleTestForm.Designer.cs" company="Sphere 10 Software">
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
    partial class ScheduleTestScreen {
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this._job1Button = new System.Windows.Forms.Button();
            this.TestButton = new System.Windows.Forms.Button();
            this.SyncStopButton = new System.Windows.Forms.Button();
            this.SyncStartButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SyncSerializeButton = new System.Windows.Forms.Button();
            this.XmlSerializeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.AsyncTestButton = new System.Windows.Forms.Button();
            this.AsyncStartButton = new System.Windows.Forms.Button();
            this.AsyncStopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(14, 89);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(771, 497);
            this.textBox1.TabIndex = 1;
            // 
            // _job1Button
            // 
            this._job1Button.Location = new System.Drawing.Point(395, 14);
            this._job1Button.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._job1Button.Name = "_job1Button";
            this._job1Button.Size = new System.Drawing.Size(88, 27);
            this._job1Button.TabIndex = 2;
            this._job1Button.Text = "Job 1";
            this._job1Button.UseVisualStyleBackColor = true;
            this._job1Button.Click += new System.EventHandler(this._job1Button_Click);
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(107, 56);
            this.TestButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(88, 27);
            this.TestButton.TabIndex = 3;
            this.TestButton.Text = "Test";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // SyncStopButton
            // 
            this.SyncStopButton.Location = new System.Drawing.Point(299, 56);
            this.SyncStopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SyncStopButton.Name = "SyncStopButton";
            this.SyncStopButton.Size = new System.Drawing.Size(88, 27);
            this.SyncStopButton.TabIndex = 4;
            this.SyncStopButton.Text = "Stop";
            this.SyncStopButton.UseVisualStyleBackColor = true;
            this.SyncStopButton.Click += new System.EventHandler(this.SyncStopButton_Click);
            // 
            // SyncStartButton
            // 
            this.SyncStartButton.Location = new System.Drawing.Point(203, 56);
            this.SyncStartButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SyncStartButton.Name = "SyncStartButton";
            this.SyncStartButton.Size = new System.Drawing.Size(88, 27);
            this.SyncStartButton.TabIndex = 5;
            this.SyncStartButton.Text = "Start";
            this.SyncStartButton.UseVisualStyleBackColor = true;
            this.SyncStartButton.Click += new System.EventHandler(this.SyncStartButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Synchronous:";
            // 
            // SyncSerializeButton
            // 
            this.SyncSerializeButton.Location = new System.Drawing.Point(395, 56);
            this.SyncSerializeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SyncSerializeButton.Name = "SyncSerializeButton";
            this.SyncSerializeButton.Size = new System.Drawing.Size(88, 27);
            this.SyncSerializeButton.TabIndex = 7;
            this.SyncSerializeButton.Text = "Serialize";
            this.SyncSerializeButton.UseVisualStyleBackColor = true;
            this.SyncSerializeButton.Click += new System.EventHandler(this.SyncSerializeButton_Click);
            // 
            // XmlSerializeButton
            // 
            this.XmlSerializeButton.Location = new System.Drawing.Point(491, 56);
            this.XmlSerializeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.XmlSerializeButton.Name = "XmlSerializeButton";
            this.XmlSerializeButton.Size = new System.Drawing.Size(88, 27);
            this.XmlSerializeButton.TabIndex = 8;
            this.XmlSerializeButton.Text = "XML Serialize";
            this.XmlSerializeButton.UseVisualStyleBackColor = true;
            this.XmlSerializeButton.Click += new System.EventHandler(this.XmlSerializeButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Asynchronous:";
            // 
            // AsyncTestButton
            // 
            this.AsyncTestButton.Location = new System.Drawing.Point(107, 14);
            this.AsyncTestButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AsyncTestButton.Name = "AsyncTestButton";
            this.AsyncTestButton.Size = new System.Drawing.Size(88, 27);
            this.AsyncTestButton.TabIndex = 10;
            this.AsyncTestButton.Text = "Test";
            this.AsyncTestButton.UseVisualStyleBackColor = true;
            this.AsyncTestButton.Click += new System.EventHandler(this.AsyncTestButton_Click);
            // 
            // AsyncStartButton
            // 
            this.AsyncStartButton.Location = new System.Drawing.Point(203, 14);
            this.AsyncStartButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AsyncStartButton.Name = "AsyncStartButton";
            this.AsyncStartButton.Size = new System.Drawing.Size(88, 27);
            this.AsyncStartButton.TabIndex = 11;
            this.AsyncStartButton.Text = "Start";
            this.AsyncStartButton.UseVisualStyleBackColor = true;
            this.AsyncStartButton.Click += new System.EventHandler(this.AsyncStartButton_Click);
            // 
            // AsyncStopButton
            // 
            this.AsyncStopButton.Location = new System.Drawing.Point(299, 14);
            this.AsyncStopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AsyncStopButton.Name = "AsyncStopButton";
            this.AsyncStopButton.Size = new System.Drawing.Size(88, 27);
            this.AsyncStopButton.TabIndex = 12;
            this.AsyncStopButton.Text = "Stop";
            this.AsyncStopButton.UseVisualStyleBackColor = true;
            this.AsyncStopButton.Click += new System.EventHandler(this.AsyncStopButton_Click);
            // 
            // ScheduleTestScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AsyncStopButton);
            this.Controls.Add(this.AsyncStartButton);
            this.Controls.Add(this.AsyncTestButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.XmlSerializeButton);
            this.Controls.Add(this.SyncSerializeButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SyncStartButton);
            this.Controls.Add(this.SyncStopButton);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this._job1Button);
            this.Controls.Add(this.textBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ScheduleTestScreen";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button _job1Button;
        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.Button SyncStopButton;
        private System.Windows.Forms.Button SyncStartButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SyncSerializeButton;
        private System.Windows.Forms.Button XmlSerializeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button AsyncTestButton;
        private System.Windows.Forms.Button AsyncStartButton;
        private System.Windows.Forms.Button AsyncStopButton;
    }
}
