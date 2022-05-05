//-----------------------------------------------------------------------
// <copyright file="EmptyCellView.cs" company="Sphere 10 Software">
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
using System.Windows.Forms;
using DevAge.Drawing;

namespace Hydrogen.Windows.Forms.AppointmentBook {
	/// <summary>
	/// Customized View to draw a rounded background
	/// </summary>
	internal class EmptyCellView : SourceGrid.Cells.Views.Cell {
		public EmptyCellView(BaseAppointmentBook parent) {
			Border = new RectangleBorder {
				Top = BorderLine.NoBorder,
				Left = BorderLine.NoBorder,
				Right = BorderLine.NoBorder,
				Bottom = parent.HorizontalBorderLine
			};
		}
	}
}
