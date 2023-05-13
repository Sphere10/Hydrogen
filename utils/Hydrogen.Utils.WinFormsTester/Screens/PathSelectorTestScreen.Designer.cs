//-----------------------------------------------------------------------
// <copyright file="PathSelectorTestForm.Designer.cs" company="Sphere 10 Software">
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
    partial class PathSelectorTestScreen {
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
            this.pathSelectorControl4 = new Hydrogen.Windows.Forms.PathSelectorControl();
            this.pathSelectorControl3 = new Hydrogen.Windows.Forms.PathSelectorControl();
            this.pathSelectorControl2 = new Hydrogen.Windows.Forms.PathSelectorControl();
            this.pathSelectorControl1 = new Hydrogen.Windows.Forms.PathSelectorControl();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // pathSelectorControl4
            // 
            this.pathSelectorControl4.Location = new System.Drawing.Point(12, 104);
            this.pathSelectorControl4.Mode = Hydrogen.Windows.Forms.PathSelectionMode.Folder;
            this.pathSelectorControl4.Name = "pathSelectorControl4";
            this.pathSelectorControl4.Path = "";
            this.pathSelectorControl4.PlaceHolderText = "Folder";
            this.pathSelectorControl4.Size = new System.Drawing.Size(260, 20);
            this.pathSelectorControl4.TabIndex = 3;
            this.pathSelectorControl4.PathChanged += this.pathSelectorControl4_PathChanged;
            // 
            // pathSelectorControl3
            // 
            this.pathSelectorControl3.Location = new System.Drawing.Point(12, 78);
            this.pathSelectorControl3.Mode = Hydrogen.Windows.Forms.PathSelectionMode.File;
            this.pathSelectorControl3.Name = "pathSelectorControl3";
            this.pathSelectorControl3.Path = "";
            this.pathSelectorControl3.PlaceHolderText = "File";
            this.pathSelectorControl3.Size = new System.Drawing.Size(260, 20);
            this.pathSelectorControl3.TabIndex = 2;
            this.pathSelectorControl3.PathChanged += this.pathSelectorControl3_PathChanged;
            // 
            // pathSelectorControl2
            // 
            this.pathSelectorControl2.Location = new System.Drawing.Point(12, 52);
            this.pathSelectorControl2.Mode = Hydrogen.Windows.Forms.PathSelectionMode.SaveFile;
            this.pathSelectorControl2.Name = "pathSelectorControl2";
            this.pathSelectorControl2.Path = "";
            this.pathSelectorControl2.PlaceHolderText = "Save File";
            this.pathSelectorControl2.Size = new System.Drawing.Size(260, 20);
            this.pathSelectorControl2.TabIndex = 1;
            this.pathSelectorControl2.PathChanged += this.pathSelectorControl2_PathChanged;
            // 
            // pathSelectorControl1
            // 
            this.pathSelectorControl1.Location = new System.Drawing.Point(12, 26);
            this.pathSelectorControl1.Name = "pathSelectorControl1";
            this.pathSelectorControl1.Path = "";
            this.pathSelectorControl1.PlaceHolderText = "Open File";
            this.pathSelectorControl1.Size = new System.Drawing.Size(260, 20);
            this.pathSelectorControl1.TabIndex = 0;
            this.pathSelectorControl1.PathChanged += this.pathSelectorControl1_PathChanged;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 142);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(106, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Force path exists";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // PathSelectorTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 260);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.pathSelectorControl4);
            this.Controls.Add(this.pathSelectorControl3);
            this.Controls.Add(this.pathSelectorControl2);
            this.Controls.Add(this.pathSelectorControl1);
            this.Name = "PathSelectorTestScreen";
            this.Text = "PathSelectorTestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Hydrogen.Windows.Forms.PathSelectorControl pathSelectorControl1;
        private Hydrogen.Windows.Forms.PathSelectorControl pathSelectorControl2;
        private Hydrogen.Windows.Forms.PathSelectorControl pathSelectorControl3;
        private Hydrogen.Windows.Forms.PathSelectorControl pathSelectorControl4;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
