// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System;
using System.Drawing;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public sealed class AppointmentViewModel {

	public AppointmentViewModel(Appointment appointmentDataObject, Action<AppointmentViewModel, CellViewModel[]> cellViewModelRenderer) {
		if (appointmentDataObject == null)
			throw new ArgumentNullException("appointmentDataObject");

		if (cellViewModelRenderer == null)
			throw new ArgumentNullException("cellViewModelRenderer");

		VisibleStartTime = appointmentDataObject.StartTime;
		VisibleEndTime = appointmentDataObject.EndTime;
		Color = appointmentDataObject.Color;
		AppointmentDataObject = appointmentDataObject;
		RequestRender = cellViewModelRenderer;
		ID = Guid.NewGuid();
	}
	public Guid ID { get; private set; }
	public ColumnViewModel Column { get; internal set; }
	public DateTime VisibleStartTime { get; internal set; }
	public DateTime VisibleEndTime { get; internal set; }
	public Color Color { get; internal set; }
	public CellViewModel[] Lines { get; internal set; }
	public Appointment AppointmentDataObject { get; internal set; }
	internal Action<AppointmentViewModel, CellViewModel[]> RequestRender { get; private set; }
	internal int StartRow { get; set; }
	internal int EndRow { get; set; }

	public bool Within(DateTime time) {
		// Example:
		// 7:00 = start work time (considered within work time)
		// 16:59 = about to end (considered within work time)
		// 17:00 = end work time (not considered within work time)
		return VisibleStartTime <= time && time < VisibleEndTime;
	}
}
