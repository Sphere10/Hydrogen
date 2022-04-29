//-----------------------------------------------------------------------
// <copyright file="SuperscriptRowHeaderCellView.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Windows.Forms.AppointmentBook {
	public class SuperscriptRowHeaderCellView : SourceGrid.Cells.Views.RowHeader {
		public SuperscriptRowHeaderCellView()   {
			Font = new System.Drawing.Font(SystemFonts.DefaultFont.FontFamily, 7);
			TextAlignment = DevAge.Drawing.ContentAlignment.TopCenter;
		}
	}
}
