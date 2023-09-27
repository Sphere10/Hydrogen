//-----------------------------------------------------------------------
// <copyright file="ScreenB.Designer.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Utils.WinFormsTester {
    partial class ScreenB {
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
            this.listMerger1 = new ListMerger();
            this.expando2 = new Expando();
            this.expando1 = new Expando();
            ((System.ComponentModel.ISupportInitialize)(this.expando2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expando1)).BeginInit();
            this.SuspendLayout();
            // 
            // listMerger1
            // 
            this.listMerger1.LeftHeader = "_leftHeaderLabel";
            this.listMerger1.Location = new System.Drawing.Point(313, 3);
            this.listMerger1.Name = "listMerger1";
            this.listMerger1.RightHeader = "_rightHeaderLabel";
            this.listMerger1.Size = new System.Drawing.Size(292, 148);
            this.listMerger1.TabIndex = 1;
            // 
            // expando2
            // 
            this.expando2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expando2.Location = new System.Drawing.Point(57, 121);
            this.expando2.Name = "expando2";
            this.expando2.Size = new System.Drawing.Size(186, 100);
            this.expando2.TabIndex = 3;
            this.expando2.Text = "expando2";
            // 
            // expando1
            // 
            this.expando1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expando1.Location = new System.Drawing.Point(57, 15);
            this.expando1.Name = "expando1";
            this.expando1.Size = new System.Drawing.Size(186, 100);
            this.expando1.TabIndex = 2;
            this.expando1.Text = "expando1";
            // 
            // ScreenB
            // 
            this.Controls.Add(this.expando2);
            this.Controls.Add(this.expando1);
            this.Controls.Add(this.listMerger1);
            this.Name = "ScreenB";
            this.Size = new System.Drawing.Size(696, 375);
            ((System.ComponentModel.ISupportInitialize)(this.expando2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expando1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ListMerger listMerger1;
        private Expando expando2;
		private Expando expando1;
    }
}
