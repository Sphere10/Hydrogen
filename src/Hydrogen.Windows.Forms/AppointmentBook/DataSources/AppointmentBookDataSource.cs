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
using System.Linq;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public class AppointmentBookDataSource : IAppointmentBookDataSource {
	private readonly IDictionary<AppointmentColumn, List<Appointment>> _appointmentBook;
	private readonly IList<Appointment> _unallocatedAppointments;

	public AppointmentBookDataSource() {
		_appointmentBook = new Dictionary<AppointmentColumn, List<Appointment>>();
		_unallocatedAppointments = new List<Appointment>();
		StartTime = DateTime.Now.ToMidnight();
		EndTime = StartTime.ToEndOfDay();
	}

	public virtual AppointmentColumn NewColumn(string name, object userObject) {
		var column = new AppointmentColumn {
			Name = name,
			UserObject = userObject
		};
		_appointmentBook.Add(column, new List<Appointment>());
		return column;
	}

	#region IAppointmentBookDataSource

	public DateTime StartTime { get; private set; }

	public DateTime EndTime { get; private set; }

	public void SetTimeRange(DateTime startTime, DateTime endTime) {
		StartTime = startTime;
		EndTime = endTime;
	}

	public virtual void Reschedule(Appointment appointment, AppointmentColumn sourceColumn, AppointmentColumn destColumn, DateTime newStartTime, DateTime newEndTime) {
		if (appointment == null)
			throw new ArgumentNullException("appointment");
		if (sourceColumn == null)
			throw new ArgumentNullException("sourceColumn");
		if (destColumn == null)
			throw new ArgumentNullException("destColumn");

		var sourceColumnIsUnallocated = sourceColumn is UnallocatedColumn;
		var destColumnIsUnallocated = destColumn is UnallocatedColumn;

		// Moved columns:
		//	Column to Column
		//	Column to UnallocatedColumn
		//	UnallocatedColumn to Column

		if (sourceColumnIsUnallocated && destColumnIsUnallocated) {
			throw new NotSupportedException("Dragging/resizing an appointment from an unallocated book to an unallocated book is not supported");
		} else if (!sourceColumnIsUnallocated && !destColumnIsUnallocated) {
			if (sourceColumn != destColumn) {
				// Column to Column
				CheckColumnExists(sourceColumn);
				CheckColumnExists(destColumn);
				_appointmentBook[sourceColumn].Remove(appointment);
				_appointmentBook[destColumn].Add(appointment);
			}
		} else if (!sourceColumnIsUnallocated) {
			//	Column to UnallocatedColumn
			CheckColumnExists(sourceColumn);
			_appointmentBook[sourceColumn].Remove(appointment);
			_unallocatedAppointments.Add(appointment);
		} else {
			//	UnallocatedColumn to Column
			CheckColumnExists(destColumn);
			_unallocatedAppointments.Remove(appointment);
			_appointmentBook[destColumn].Add(appointment);
		}

		appointment.StartTime = newStartTime;
		appointment.EndTime = newEndTime;

	}

	public Appointment NewAppointment(AppointmentColumn column, string id, DateTime startTime, DateTime endTime, string subject, string locationPart1, string locationPart2, string notes, Color color, object userObject) {
		if (column == null)
			throw new ArgumentNullException("column");

		var appointment = new Appointment {
			ID = id,
			StartTime = startTime,
			EndTime = endTime,
			Subject = subject,
			LocationPart1 = locationPart1,
			LocationPart2 = locationPart2,
			Notes = notes,
			Color = color,
			UserObject = userObject
		};
		if (column is UnallocatedColumn) {
			_unallocatedAppointments.Add(appointment);
		} else {
			CheckColumnExists(column);
			_appointmentBook[column].Add(appointment);
		}
		return appointment;
	}

	public virtual void DeleteAppointment(AppointmentColumn column, Appointment appointment) {
		if (column == null)
			throw new ArgumentNullException("column");

		if (column is UnallocatedColumn) {
			_unallocatedAppointments.Remove(appointment);
		} else {
			CheckColumnExists(column);
			_appointmentBook[column].Remove(appointment);
		}
	}

	public virtual IEnumerable<AppointmentColumn> GetColumns() {
		return _appointmentBook.Keys;
	}

	public IEnumerable<Appointment> GetAppointments(AppointmentColumn column) {
		if (column == null)
			throw new ArgumentNullException("column");

		if (column is UnallocatedColumn) {
			return _unallocatedAppointments;
		} else {
			CheckColumnExists(column);
			return _appointmentBook[column];
		}
	}

	public bool IsTimeAvailable(AppointmentColumn column, Appointment appointment, DateTime startTime, DateTime endTime) {
		if (column == null)
			throw new ArgumentNullException("column");

		CheckColumnExists(column);
		var overlappingAppointments =
			_appointmentBook[column]
				.Where(a => startTime < a.EndTime && endTime > a.StartTime)
				.Except(appointment);

		return !overlappingAppointments.Any();
	}

	public bool IsColumnCompatible(AppointmentColumn column, Appointment appointment) {
		return true;
	}

	#endregion


	#region Auxillary

	void CheckColumnExists(AppointmentColumn column) {
		if (!_appointmentBook.ContainsKey(column))
			throw new SoftwareException("Column '{0}' does not exist", column.Name);
	}

	#endregion

}
