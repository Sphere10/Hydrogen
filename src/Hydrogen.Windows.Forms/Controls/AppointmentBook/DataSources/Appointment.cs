//-----------------------------------------------------------------------
// <copyright file="Appointment.cs" company="Sphere 10 Software">
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
	public class Appointment {
		public Appointment() {
			Color = Color.LightGray;
		}

		public string ID { get; set; }

		public object UserObject { get; set; }

		public string Subject { get; set; }

		public string LocationPart1 { get; set; }

		public string LocationPart2 { get; set; }

		public string Notes { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public Color Color { get; set; }

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Appointment) obj);
		}

		protected bool Equals(Appointment other) {
			return ID == other.ID;
		}

		public override int GetHashCode() {
			return (UserObject != null ? UserObject.GetHashCode() : 0);
		}

	}
}
