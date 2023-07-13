// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public interface IAppointmentBookDataSource {
	void SetTimeRange(DateTime startTime, DateTime endTime);

	void Reschedule(Appointment appointment, AppointmentColumn sourceColumn, AppointmentColumn destColumn, DateTime newStartTime, DateTime newEndTime);

	Appointment NewAppointment(AppointmentColumn column, string id, DateTime startTime, DateTime endTime, string subject, string locationPart1, string locationPart2, string notes, Color color, object userObject);

	void DeleteAppointment(AppointmentColumn column, Appointment appointment);

	IEnumerable<AppointmentColumn> GetColumns();

	IEnumerable<Appointment> GetAppointments(AppointmentColumn column);

	bool IsTimeAvailable(AppointmentColumn column, Appointment appointment, DateTime startTime, DateTime endTime);
}
