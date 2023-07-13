// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public class SuperscriptRowHeaderCellView : SourceGrid.Cells.Views.RowHeader {
	public SuperscriptRowHeaderCellView() {
		Font = new System.Drawing.Font(SystemFonts.DefaultFont.FontFamily, 7);
		TextAlignment = DevAge.Drawing.ContentAlignment.TopCenter;
	}
}
