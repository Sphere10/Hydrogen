// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
    partial class WizardDialog<T> {
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
			this._nextButton = new System.Windows.Forms.Button();
			this._previousButton = new System.Windows.Forms.Button();
			this._contentPanel = new System.Windows.Forms.Panel();
			this.loadingCircle1 = new Hydrogen.Windows.Forms.LoadingCircle();
			this.SuspendLayout();
			// 
			// _nextButton
			// 
			this._nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._nextButton.Location = new System.Drawing.Point(388, 332);
			this._nextButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._nextButton.Name = "_nextButton";
			this._nextButton.Size = new System.Drawing.Size(88, 27);
			this._nextButton.TabIndex = 0;
			this._nextButton.Text = "&Next";
			this._nextButton.UseVisualStyleBackColor = true;
			this._nextButton.Click += new System.EventHandler(this._nextButton_Click);
			// 
			// _previousButton
			// 
			this._previousButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._previousButton.Location = new System.Drawing.Point(13, 331);
			this._previousButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._previousButton.Name = "_previousButton";
			this._previousButton.Size = new System.Drawing.Size(88, 27);
			this._previousButton.TabIndex = 2;
			this._previousButton.Text = "&Previous";
			this._previousButton.UseVisualStyleBackColor = true;
			this._previousButton.Click += new System.EventHandler(this._previousButton_Click);
			// 
			// _contentPanel
			// 
			this._contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._contentPanel.Location = new System.Drawing.Point(14, 12);
			this._contentPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._contentPanel.Name = "_contentPanel";
			this._contentPanel.Size = new System.Drawing.Size(462, 313);
			this._contentPanel.TabIndex = 4;
			// 
			// loadingCircle1
			// 
			this.loadingCircle1.Active = false;
			this.loadingCircle1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.loadingCircle1.BackColor = System.Drawing.Color.Transparent;
			this.loadingCircle1.Color = System.Drawing.Color.DarkGray;
			this.loadingCircle1.InnerCircleRadius = 5;
			this.loadingCircle1.Location = new System.Drawing.Point(345, 330);
			this.loadingCircle1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.loadingCircle1.Name = "loadingCircle1";
			this.loadingCircle1.NumberSpoke = 12;
			this.loadingCircle1.OuterCircleRadius = 11;
			this.loadingCircle1.RotationSpeed = 100;
			this.loadingCircle1.Size = new System.Drawing.Size(35, 28);
			this.loadingCircle1.SpokeThickness = 2;
			this.loadingCircle1.StylePreset = Hydrogen.Windows.Forms.LoadingCircle.StylePresets.MacOSX;
			this.loadingCircle1.TabIndex = 5;
			this.loadingCircle1.Text = "_loadingCircle";
			this.loadingCircle1.Visible = false;
			// 
			// WizardDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(489, 370);
			this.Controls.Add(this.loadingCircle1);
			this.Controls.Add(this._contentPanel);
			this.Controls.Add(this._previousButton);
			this.Controls.Add(this._nextButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "WizardDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "WizardDialog";
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel _contentPanel;
        internal System.Windows.Forms.Button _nextButton;
        internal System.Windows.Forms.Button _previousButton;
        private LoadingCircle loadingCircle1;
    }
}
