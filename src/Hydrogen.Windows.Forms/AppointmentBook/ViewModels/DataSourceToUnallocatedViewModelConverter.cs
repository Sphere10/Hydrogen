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

public class DataSourceToUnallocatedViewModelConverter : DataSourceToViewModelConverter {
	public override AppointmentBookViewModel Convert(IAppointmentBookDataSource dataSource, DateTime timePeriodStart, TimePeriodType timeView, AppointmentBookViewModelFilter columnFilter) {
		DateTime startTime;
		DateTime endTime;
		switch (timeView) {
			case TimePeriodType.Monthly:
				startTime = timePeriodStart.ToBeginningOfMonth();
				endTime = timePeriodStart.AddMonths(1).ToBeginningOfMonth();
				break;
			default:
				startTime = timePeriodStart.ToMidnight();
				endTime = timePeriodStart.ToNextDay().ToMidnight();
				break;
		}

		return new UnallocatedBookViewModel(
			"Unallocated",
			from appointment in dataSource.GetAppointments(new UnallocatedColumn())
			select Convert(appointment),
			startTime,
			timeView
		);
	}


	protected override void RenderAppointmentCellsTiny(AppointmentViewModel viewModel, CellViewModel[] cells) {
		if (cells.Length < 1)
			throw new ArgumentOutOfRangeException("cells", "Must contain at least 1 cell(s)");

		var subjectString = (viewModel.AppointmentDataObject.Subject ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var location1 = (viewModel.AppointmentDataObject.LocationPart1 ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var location2 = (viewModel.AppointmentDataObject.LocationPart2 ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var notesString = (viewModel.AppointmentDataObject.Notes ?? string.Empty).Replace(Environment.NewLine, string.Empty);

		bool underline = false;
		var string1 = string.Empty;
		var string2 = string.Empty;
		if (!string.IsNullOrEmpty(subjectString)) {
			string1 = subjectString;
			underline = true;
		} else if (!string.IsNullOrEmpty(location1)) {
			string1 = location1;
			string2 = location2;
		} else {
			string1 = notesString;
		}

		cells[0].Text = string1;
		if (underline)
			cells[0].FontStyle = FontStyle.Bold | FontStyle.Underline;

		if (cells.Length > 1)
			cells[1].Text = string2;

		ColorCells(cells, viewModel.Color);
	}

	protected override void RenderAppointmentCellsSmall(AppointmentViewModel viewModel, CellViewModel[] cells) {
		if (cells.Length < 4)
			throw new ArgumentOutOfRangeException("cells", "Must contain at least 4 cell(s)");

		var subjectString = (viewModel.AppointmentDataObject.Subject ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var location1 = (viewModel.AppointmentDataObject.LocationPart1 ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var location2 = (viewModel.AppointmentDataObject.LocationPart2 ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var notesString = (viewModel.AppointmentDataObject.Notes ?? string.Empty).Replace(Environment.NewLine, string.Empty);

		var stringStack = new List<string>();
		bool underline = false;

		if (!string.IsNullOrEmpty(subjectString)) {
			stringStack.Add(subjectString);
			underline = true;
		}

		if (!string.IsNullOrEmpty(location1)) {
			stringStack.Add(location1);
		}

		if (!string.IsNullOrEmpty(location2)) {
			stringStack.Add(location2);
		}

		if (!string.IsNullOrEmpty(notesString)) {
			stringStack.Add(notesString);
		}

		for (int i = 0; i < stringStack.Count; i++) {
			cells[i].Text = stringStack[i];
			if (i == 0 && underline)
				cells[i].FontStyle = FontStyle.Bold | FontStyle.Underline;
		}
		ColorCells(cells, viewModel.Color);
	}

	protected override void RenderTinyAppointmentCellsFull(AppointmentViewModel viewModel, CellViewModel[] cells) {
		if (cells.Length < 7)
			throw new ArgumentOutOfRangeException("cells", "Must contain at least 7 cell(s)");

		var subjectString = (viewModel.AppointmentDataObject.Subject ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var location1 = (viewModel.AppointmentDataObject.LocationPart1 ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var location2 = (viewModel.AppointmentDataObject.LocationPart2 ?? string.Empty).Replace(Environment.NewLine, string.Empty);
		var notesString = (viewModel.AppointmentDataObject.Notes ?? string.Empty).Replace(Environment.NewLine, string.Empty);

		var stringStack = new List<string>();
		bool underline = false;
		stringStack.Add(string.Empty);

		if (!string.IsNullOrEmpty(subjectString)) {
			stringStack.Add(subjectString);
			underline = true;
		}

		stringStack.Add(string.Empty);

		if (!string.IsNullOrEmpty(location1)) {
			stringStack.Add(location1);
		}

		if (!string.IsNullOrEmpty(location2)) {
			stringStack.Add(location2);
		}

		stringStack.Add(string.Empty);

		if (!string.IsNullOrEmpty(notesString)) {
			stringStack.Add(notesString);
		}

		for (int i = 0; i < stringStack.Count; i++) {
			cells[i].Text = stringStack[i];
		}

		for (int i = 0; i < stringStack.Count; i++) {
			if (cells.Length < i + 1)
				break;

			cells[i].Text = stringStack[i];
			if (i == 2 && underline)
				cells[1].FontStyle = FontStyle.Bold | FontStyle.Underline;
		}
		ColorCells(cells, viewModel.Color);
	}

	private void ColorCells(CellViewModel[] cells, Color color) {
		foreach (var cell in cells) {
			cell.BackColor = color;
		}
	}
}
