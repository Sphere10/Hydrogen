//-----------------------------------------------------------------------
// <copyright file="IAppointmentBookDataSource.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Windows.Forms.AppointmentBook {
	public interface IAppointmentBookDataSource {
		void SetTimeRange(DateTime startTime, DateTime endTime);
		void Reschedule(Appointment appointment, AppointmentColumn sourceColumn,  AppointmentColumn destColumn, DateTime newStartTime, DateTime newEndTime);
		Appointment NewAppointment(AppointmentColumn column, string id, DateTime startTime, DateTime endTime, string subject, string locationPart1, string locationPart2, string notes, Color color, object userObject);
		void DeleteAppointment(AppointmentColumn column, Appointment appointment);
		IEnumerable<AppointmentColumn> GetColumns();
		IEnumerable<Appointment> GetAppointments(AppointmentColumn column);
		bool IsTimeAvailable(AppointmentColumn column, Appointment appointment, DateTime startTime, DateTime endTime);
	}
}
