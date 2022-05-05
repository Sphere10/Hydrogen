//-----------------------------------------------------------------------
// <copyright file="TextBoxNumeric.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;


namespace SourceGrid.Cells.Editors
{
	/// <summary>
	/// An editor that use a TextBoxTypedNumeric for editing support. You can customize the Control.NumericCharStyle property to enable char validation.
	/// </summary>
    [System.ComponentModel.ToolboxItem(false)]
    public class TextBoxNumeric : TextBox
	{
		#region Constructor
		/// <summary>
		/// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
		/// </summary>
		/// <param name="p_Type">The type of this model</param>
		public TextBoxNumeric(Type p_Type):base(p_Type)
		{
		}
		#endregion

		#region Edit Control
		/// <summary>
		/// Create the editor control
		/// </summary>
		/// <returns></returns>
		protected override Control CreateControl()
		{
            DevAge.Windows.Forms.DevAgeTextBox editor = new DevAge.Windows.Forms.DevAgeTextBox();
			editor.BorderStyle = BorderStyle.None;
			editor.AutoSize = false;
			editor.Validator = this;
			return editor;
		}

		/// <summary>
		/// Gets the control used for editing the cell.
		/// </summary>
		public new DevAge.Windows.Forms.DevAgeTextBox Control
		{
			get
			{
                return (DevAge.Windows.Forms.DevAgeTextBox)base.Control;
			}
		}
		#endregion
	}
}
