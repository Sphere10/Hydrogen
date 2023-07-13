// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;

namespace Hydrogen.Windows.Forms.AppointmentBook;

internal class AppointmentDragObject {
	public AppointmentColumn SourceColumn { get; set; }
	public Appointment Appointment { get; set; }
	public Bitmap CanDropAppointmentBitmap { get; set; }
	public Bitmap CannotDropAppointmentBitmap { get; set; }
	public Point CursorOffset { get; set; }

	internal AppointmentColumn LastColumnOver { get; set; }
}
