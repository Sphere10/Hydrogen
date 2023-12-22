//-----------------------------------------------------------------------
// <copyright file="ScreenA.Designer.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Utils.WinFormsTester
{
    partial class ObjectSpaceScreen
    {
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
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectSpaceScreen));
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			_buildButton = new System.Windows.Forms.ToolStripButton();
			_objectSpacePathControl = new PathSelectorControl();
			textBox1 = new System.Windows.Forms.TextBox();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// toolStrip1
			// 
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _buildButton });
			toolStrip1.Location = new System.Drawing.Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new System.Drawing.Size(580, 25);
			toolStrip1.TabIndex = 2;
			toolStrip1.Text = "toolStrip1";
			// 
			// _buildButton
			// 
			_buildButton.Image = (System.Drawing.Image)resources.GetObject("_buildButton.Image");
			_buildButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			_buildButton.Name = "_buildButton";
			_buildButton.Size = new System.Drawing.Size(54, 22);
			_buildButton.Text = "Build";
			_buildButton.Click += _buildButton_Click;
			// 
			// _objectSpacePathControl
			// 
			_objectSpacePathControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_objectSpacePathControl.Location = new System.Drawing.Point(3, 25);
			_objectSpacePathControl.Margin = new System.Windows.Forms.Padding(0);
			_objectSpacePathControl.Name = "_objectSpacePathControl";
			_objectSpacePathControl.Path = "";
			_objectSpacePathControl.PlaceHolderText = "Select path to Object Space database";
			_objectSpacePathControl.Size = new System.Drawing.Size(574, 23);
			_objectSpacePathControl.TabIndex = 3;
			// 
			// textBox1
			// 
			textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			textBox1.Location = new System.Drawing.Point(3, 51);
			textBox1.Multiline = true;
			textBox1.Name = "textBox1";
			textBox1.Size = new System.Drawing.Size(574, 215);
			textBox1.TabIndex = 4;
			// 
			// ObjectSpaceScreen
			// 
			Controls.Add(textBox1);
			Controls.Add(_objectSpacePathControl);
			Controls.Add(toolStrip1);
			Name = "ObjectSpaceScreen";
			Size = new System.Drawing.Size(580, 269);
			ToolBar = toolStrip1;
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton _buildButton;
		private PathSelectorControl _objectSpacePathControl;
		private System.Windows.Forms.TextBox textBox1;
	}
}
