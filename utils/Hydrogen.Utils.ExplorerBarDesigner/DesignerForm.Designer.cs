//-----------------------------------------------------------------------
// <copyright file="DesignerForm.Designer.cs" company="Sphere 10 Software">
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

using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.ExplorerBarDesigner {
    partial class DesignerForm {
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
            this._exportButton = new System.Windows.Forms.Button();
            this._propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.taskPane1 = new TaskPane();
            this.expando1 = new Expando();
            this.taskItem1 = new TaskItem();
            this.taskItem2 = new TaskItem();
            this.expando2 = new Expando();
            this.taskItem3 = new TaskItem();
            this.taskItem4 = new TaskItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.taskPane1)).BeginInit();
            this.taskPane1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expando1)).BeginInit();
            this.expando1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expando2)).BeginInit();
            this.expando2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _exportButton
            // 
            this._exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._exportButton.Location = new System.Drawing.Point(692, 420);
            this._exportButton.Name = "_exportButton";
            this._exportButton.Size = new System.Drawing.Size(75, 23);
            this._exportButton.TabIndex = 3;
            this._exportButton.Text = "Export";
            this._exportButton.UseVisualStyleBackColor = true;
            this._exportButton.Click += new System.EventHandler(this._exportButton_Click);
            // 
            // _propertyGrid
            // 
            this._propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._propertyGrid.Location = new System.Drawing.Point(278, 12);
            this._propertyGrid.Name = "_propertyGrid";
            this._propertyGrid.Size = new System.Drawing.Size(489, 388);
            this._propertyGrid.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.taskPane1);
            this.panel1.Location = new System.Drawing.Point(12, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(246, 412);
            this.panel1.TabIndex = 4;
            // 
            // taskPane1
            // 
            this.taskPane1.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.taskPane1.Expandos.AddRange(new Expando[] {
            this.expando1,
            this.expando2});
            this.taskPane1.Location = new System.Drawing.Point(3, 22);
            this.taskPane1.Name = "taskPane1";
            this.taskPane1.Size = new System.Drawing.Size(159, 348);
            this.taskPane1.TabIndex = 1;
            this.taskPane1.Text = "taskPane1";
            // 
            // expando1
            // 
            this.expando1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.expando1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expando1.Items.AddRange(new System.Windows.Forms.Control[] {
            this.taskItem1,
            this.taskItem2});
            this.expando1.Location = new System.Drawing.Point(12, 12);
            this.expando1.Name = "expando1";
            this.expando1.Size = new System.Drawing.Size(135, 100);
            this.expando1.SpecialGroup = true;
            this.expando1.TabIndex = 0;
            this.expando1.Text = "expando1";
            // 
            // taskItem1
            // 
            this.taskItem1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.taskItem1.BackColor = System.Drawing.Color.Transparent;
            this.taskItem1.Image = null;
            this.taskItem1.Location = new System.Drawing.Point(20, 28);
            this.taskItem1.Name = "taskItem1";
            this.taskItem1.Size = new System.Drawing.Size(111, 16);
            this.taskItem1.TabIndex = 0;
            this.taskItem1.Text = "taskItem1";
            this.taskItem1.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.taskItem1.UseVisualStyleBackColor = false;
            // 
            // taskItem2
            // 
            this.taskItem2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.taskItem2.BackColor = System.Drawing.Color.Transparent;
            this.taskItem2.Image = null;
            this.taskItem2.Location = new System.Drawing.Point(20, 50);
            this.taskItem2.Name = "taskItem2";
            this.taskItem2.Size = new System.Drawing.Size(111, 16);
            this.taskItem2.TabIndex = 1;
            this.taskItem2.Text = "taskItem2";
            this.taskItem2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.taskItem2.UseVisualStyleBackColor = false;
            // 
            // expando2
            // 
            this.expando2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.expando2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expando2.Items.AddRange(new System.Windows.Forms.Control[] {
            this.taskItem3,
            this.taskItem4});
            this.expando2.Location = new System.Drawing.Point(12, 124);
            this.expando2.Name = "expando2";
            this.expando2.Size = new System.Drawing.Size(135, 100);
            this.expando2.TabIndex = 1;
            this.expando2.Text = "expando2";
            // 
            // taskItem3
            // 
            this.taskItem3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.taskItem3.BackColor = System.Drawing.Color.Transparent;
            this.taskItem3.Image = null;
            this.taskItem3.Location = new System.Drawing.Point(20, 29);
            this.taskItem3.Name = "taskItem3";
            this.taskItem3.Size = new System.Drawing.Size(111, 16);
            this.taskItem3.TabIndex = 0;
            this.taskItem3.Text = "taskItem3";
            this.taskItem3.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.taskItem3.UseVisualStyleBackColor = false;
            // 
            // taskItem4
            // 
            this.taskItem4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.taskItem4.BackColor = System.Drawing.Color.Transparent;
            this.taskItem4.Image = null;
            this.taskItem4.Location = new System.Drawing.Point(20, 52);
            this.taskItem4.Name = "taskItem4";
            this.taskItem4.Size = new System.Drawing.Size(111, 16);
            this.taskItem4.TabIndex = 1;
            this.taskItem4.Text = "taskItem4";
            this.taskItem4.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.taskItem4.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panel2.Location = new System.Drawing.Point(193, 128);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(34, 100);
            this.panel2.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 455);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._exportButton);
            this.Controls.Add(this._propertyGrid);
            this.Name = "Form1";
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.taskPane1)).EndInit();
            this.taskPane1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expando1)).EndInit();
            this.expando1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expando2)).EndInit();
            this.expando2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _exportButton;
        private System.Windows.Forms.PropertyGrid _propertyGrid;
        private System.Windows.Forms.Panel panel1;
        private TaskPane taskPane1;
        private Expando expando1;
        private TaskItem taskItem1;
        private TaskItem taskItem2;
        private Expando expando2;
        private TaskItem taskItem3;
        private TaskItem taskItem4;
        private System.Windows.Forms.Panel panel2;
    }
}

