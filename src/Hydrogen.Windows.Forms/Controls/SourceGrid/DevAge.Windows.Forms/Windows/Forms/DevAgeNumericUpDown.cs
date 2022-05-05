//-----------------------------------------------------------------------
// <copyright file="DevAgeNumericUpDown.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>TODO</author>
// <date>TODO</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;


namespace DevAge.Windows.Forms
{
	public class DevAgeNumericUpDown : NumericUpDown
	{
		private System.ComponentModel.IContainer components = null;

		public DevAgeNumericUpDown()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			UserEdit = true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		protected override void OnValidated(EventArgs e)
		{
			base.OnValidated(e);
			base.ParseEditText();
		}
	}
}

