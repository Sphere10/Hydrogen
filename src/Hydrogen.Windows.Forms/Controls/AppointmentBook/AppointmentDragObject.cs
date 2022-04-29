//-----------------------------------------------------------------------
// <copyright file="AppointmentDragObject.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.Forms.AppointmentBook {
	internal class AppointmentDragObject {
		public AppointmentColumn SourceColumn { get; set; }
		public Appointment Appointment { get; set; }
		public Bitmap CanDropAppointmentBitmap { get; set; }
		public Bitmap CannotDropAppointmentBitmap { get; set; }
		public Point CursorOffset { get; set;  }

		internal AppointmentColumn LastColumnOver { get; set; }
	}
}
