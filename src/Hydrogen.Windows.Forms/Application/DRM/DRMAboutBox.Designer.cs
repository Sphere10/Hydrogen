// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms
{
	partial class DRMAboutBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			_changeProductKeyButton = new System.Windows.Forms.Button();
			_expirationControl = new ProductExpirationDetailsControl();
			SuspendLayout();
			// 
			// okButton
			// 
			okButton.Location = new System.Drawing.Point(412, 303);
			// 
			// _label12
			// 
			_label12.Size = new System.Drawing.Size(132, 15);
			_label12.Text = "{NumberOfUsesByUser}";
			// 
			// _changeProductKeyButton
			// 
			_changeProductKeyButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_changeProductKeyButton.Location = new System.Drawing.Point(14, 303);
			_changeProductKeyButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_changeProductKeyButton.Name = "_changeProductKeyButton";
			_changeProductKeyButton.Size = new System.Drawing.Size(134, 27);
			_changeProductKeyButton.TabIndex = 50;
			_changeProductKeyButton.Text = "Change Product Key";
			_changeProductKeyButton.UseVisualStyleBackColor = true;
			_changeProductKeyButton.Click += _changeProductKeyButton_Click;
			// 
			// _expirationControl
			// 
			_expirationControl.AutoSize = true;
			_expirationControl.Location = new System.Drawing.Point(14, 239);
			_expirationControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			_expirationControl.Name = "_expirationControl";
			_expirationControl.Size = new System.Drawing.Size(532, 58);
			_expirationControl.TabIndex = 51;
			// 
			// DRMAboutBox
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(559, 343);
			Controls.Add(_expirationControl);
			Controls.Add(_changeProductKeyButton);
			Name = "DRMAboutBox";
			Controls.SetChildIndex(okButton, 0);
			Controls.SetChildIndex(applicationBanner1, 0);
			Controls.SetChildIndex(_label1, 0);
			Controls.SetChildIndex(_label2, 0);
			Controls.SetChildIndex(_label3, 0);
			Controls.SetChildIndex(_label4, 0);
			Controls.SetChildIndex(_link1, 0);
			Controls.SetChildIndex(_label5, 0);
			Controls.SetChildIndex(_label6, 0);
			Controls.SetChildIndex(_label7, 0);
			Controls.SetChildIndex(_label8, 0);
			Controls.SetChildIndex(_label9, 0);
			Controls.SetChildIndex(_label10, 0);
			Controls.SetChildIndex(_label11, 0);
			Controls.SetChildIndex(_label12, 0);
			Controls.SetChildIndex(_companyNumberLabel, 0);
			Controls.SetChildIndex(_changeProductKeyButton, 0);
			Controls.SetChildIndex(_expirationControl, 0);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		protected System.Windows.Forms.Button _changeProductKeyButton;
		private ProductExpirationDetailsControl _expirationControl;
	}
}
