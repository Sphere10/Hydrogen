//-----------------------------------------------------------------------
// <copyright file="AppointmentDraggingEvent.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Hydrogen.Windows.Forms.AppointmentBook {
	public class AppointmentDraggingEvent : AppointmentEvent {

		public AppointmentDraggingEvent() {
			DestinationColumn = null;
			IsDestinationCompatible = true;
		}

		public AppointmentColumn DestinationColumn { get; set; }
		public DateTime DestinationStartTime { get; set; }
		public DateTime DestinationEndTime { get; set; }
		public bool IsDestinationCompatible { get; set; }
	}
}
