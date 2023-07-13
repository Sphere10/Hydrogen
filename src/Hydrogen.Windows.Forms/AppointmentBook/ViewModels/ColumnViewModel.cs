// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms.AppointmentBook;

public sealed class ColumnViewModel {

	public ColumnViewModel(
		string name,
		AppointmentViewModel[] appointments,
		AppointmentColumn columnDataObject) {
		Name = name;
		Appointments = appointments;
		ColumnDataObject = columnDataObject;
	}

	public string Name { get; internal set; }

	public int Index { get; internal set; }

	public AppointmentViewModel[] Appointments { get; internal set; }

	public AppointmentColumn ColumnDataObject { get; internal set; }

}
