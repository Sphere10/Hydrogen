// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms {
    partial class ServiceStatusControl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this._trafficLightLabel = new System.Windows.Forms.Label();
            this._trafficLight = new Hydrogen.Windows.Forms.PanelEx();
            this._serviceButton = new System.Windows.Forms.Button();
            this._serviceDetailLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _trafficLightLabel
            // 
            this._trafficLightLabel.AutoSize = true;
            this._trafficLightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._trafficLightLabel.Location = new System.Drawing.Point(33, 7);
            this._trafficLightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._trafficLightLabel.Name = "_trafficLightLabel";
            this._trafficLightLabel.Size = new System.Drawing.Size(78, 13);
            this._trafficLightLabel.TabIndex = 16;
            this._trafficLightLabel.Text = "Not Running";
            // 
            // _trafficLight
            // 
            this._trafficLight.EnableStateChangeEvent = false;
            this._trafficLight.BackColor = System.Drawing.Color.Red;
            this._trafficLight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._trafficLight.Location = new System.Drawing.Point(-2, 0);
            this._trafficLight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._trafficLight.Name = "_trafficLight";
            this._trafficLight.Size = new System.Drawing.Size(28, 27);
            this._trafficLight.TabIndex = 15;
            // 
            // _serviceButton
            // 
            this._serviceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._serviceButton.Location = new System.Drawing.Point(846, 0);
            this._serviceButton.Margin = new System.Windows.Forms.Padding(2);
            this._serviceButton.Name = "_serviceButton";
            this._serviceButton.Size = new System.Drawing.Size(83, 28);
            this._serviceButton.TabIndex = 14;
            this._serviceButton.Text = "Stop";
            this._serviceButton.UseVisualStyleBackColor = true;
            this._serviceButton.Click += new System.EventHandler(this._serviceButton_Click);
            // 
            // _serviceDetailLabel
            // 
            this._serviceDetailLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._serviceDetailLabel.AutoEllipsis = true;
            this._serviceDetailLabel.Location = new System.Drawing.Point(131, 7);
            this._serviceDetailLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._serviceDetailLabel.Name = "_serviceDetailLabel";
            this._serviceDetailLabel.Size = new System.Drawing.Size(709, 15);
            this._serviceDetailLabel.TabIndex = 17;
            this._serviceDetailLabel.Text = "Service Detail";
            // 
            // ServiceStatusControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._serviceDetailLabel);
            this.Controls.Add(this._trafficLightLabel);
            this.Controls.Add(this._trafficLight);
            this.Controls.Add(this._serviceButton);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ServiceStatusControl";
            this.Size = new System.Drawing.Size(929, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _trafficLightLabel;
        private Hydrogen.Windows.Forms.PanelEx _trafficLight;
        private System.Windows.Forms.Button _serviceButton;
        private System.Windows.Forms.Label _serviceDetailLabel;
    }
}
