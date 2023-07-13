// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public class AppointmentBookViewModel : BaseAppointmentBookViewModel {
	private ColumnViewModel[] _allColumns;
	private ColumnViewModel[] _visibleColumns;
	private AppointmentBookViewModelFilter _columnFilter;
	protected IDictionary<Tuple<int, int>, AppointmentViewModel> _blockLookup;

	public AppointmentBookViewModel(IEnumerable<ColumnViewModel> appointments, DateTime timePeriodStart, TimePeriodType timeView, AppointmentBookViewModelFilter columnFilter = AppointmentBookViewModelFilter.All)
		: base(timePeriodStart, timeView) {
		SetData(appointments, timePeriodStart, timeView, columnFilter);
	}

	public AppointmentBookViewModelFilter ColumnFilter {
		get { return _columnFilter; }
		set {
			_columnFilter = value;
			SetVisibleColumns();
		}
	}

	public void SetData(IEnumerable<ColumnViewModel> appointments, DateTime timePeriodStart, TimePeriodType timeView, AppointmentBookViewModelFilter columnFilter = AppointmentBookViewModelFilter.All) {
		base.Initialize(timePeriodStart, timeView);
		_blockLookup = new Dictionary<Tuple<int, int>, AppointmentViewModel>();
		_allColumns = appointments.ToArray();
		_visibleColumns = new ColumnViewModel[0];
		_columnFilter = columnFilter;
		SetVisibleColumns();
	}

	internal override string GetColumnHeaderText(int col) {
		return _visibleColumns[col].Name;
	}

	internal ColumnViewModel GetColumnAt(int col) {
		return _visibleColumns[col];
	}

	internal AppointmentViewModel GetAppointmentBlockAt(int col, int row) {
		var key = Tuple.Create(col, row);
		if (!_blockLookup.ContainsKey(key))
			return null;

		return _blockLookup[key];
	}

	internal AppointmentViewModel FindAppointmentByDataObject(Appointment appointment) {
		return _blockLookup.Values.FirstOrDefault(avm => avm.AppointmentDataObject.Equals(appointment));
	}

	internal bool ContainsColumn(AppointmentColumn column) {
		return _visibleColumns.Any(vc => vc.ColumnDataObject == column);
	}

	internal override int GetColumnCount() {
		return _visibleColumns.Length;
	}

	internal override CellViewModel GetCellDisplay(int col, int row) {
		if (col < 0 || col > GetColumnCount() - 1 || row < 0 || row > GetRowCount()) {
			return CellViewModel.Empty;
		}
		// The CellDisplay's ARE the data source, so we route call to datasource
		var display = CellViewModel.Empty;

		var block = GetAppointmentBlockAt(col, row);
		if (block != null) {
			var startRow = block.StartRow;
			var index = row - startRow;
			display = block.Lines[index];
		}
		return display;
	}

	protected override void RecreateCellDisplays() {
		_blockLookup.Clear();
		if (_visibleColumns != null) {
			for (int i = 0; i < _visibleColumns.Length; i++) {
				var appointmentColumn = _visibleColumns[i];
				appointmentColumn.Index = i;
				for (int j = 0; j < appointmentColumn.Appointments.Length; j++) {
					var appointmentBlock = appointmentColumn.Appointments[j];
					appointmentBlock.Column = appointmentColumn;

					if (appointmentBlock.AppointmentDataObject.EndTime <= TimePeriodStart || appointmentBlock.AppointmentDataObject.StartTime >= TimePeriodEnd)
						continue;

					// clip the appointment starttimes/endtimes (for the viewmodel)
					if (appointmentBlock.AppointmentDataObject.StartTime < this.TimePeriodStart)
						appointmentBlock.VisibleStartTime = TimePeriodStart;
					else
						appointmentBlock.VisibleStartTime = appointmentBlock.AppointmentDataObject.StartTime;

					if (appointmentBlock.AppointmentDataObject.EndTime > TimePeriodEnd)
						appointmentBlock.VisibleEndTime = TimePeriodEnd;
					else
						appointmentBlock.VisibleEndTime = appointmentBlock.AppointmentDataObject.EndTime;

					var startRow = StartTimeToRow(appointmentBlock.VisibleStartTime);
					var endRow = FinishTimeToRow(appointmentBlock.VisibleEndTime);

					// save the apppointment block in the lookup
					for (int x = startRow; x <= endRow; x++)
						_blockLookup[Tuple.Create(i, x)] = appointmentBlock;

					appointmentBlock.StartRow = startRow;
					appointmentBlock.EndRow = endRow;
					appointmentBlock.Lines = CellViewModel.CreateArray(endRow - startRow + 1);


					// Set the traits of the cells
					appointmentBlock.Lines.WithDescriptions().ForEach(cellDescription => {
						cellDescription.Item.Traits = 0;
						if (cellDescription.Description.HasFlag(EnumeratedItemDescription.First)) {
							cellDescription.Item.Traits = cellDescription.Item.Traits.CopyAndSetFlags(CellTraits.Top);
						}

						if (cellDescription.Description.HasFlag(EnumeratedItemDescription.Last)) {
							cellDescription.Item.Traits = cellDescription.Item.Traits.CopyAndSetFlags(CellTraits.Bottom);
						}

						if (cellDescription.Description.HasFlag(EnumeratedItemDescription.Interior)) {
							cellDescription.Item.Traits = cellDescription.Item.Traits.CopyAndSetFlags(CellTraits.Interior);
						}
					});
					appointmentBlock.RequestRender(appointmentBlock, appointmentBlock.Lines);
				}
			}
		}
	}

	protected virtual void SetVisibleColumns() {
		try {
			_blockLookup.Clear();

			//// filter out unnecessary appointment columns and blocks
			DateTime startTime;
			DateTime endTime;
			switch (TimeView) {
				case TimePeriodType.Monthly:
					startTime = TimePeriodStart.ToBeginningOfMonth();
					endTime = TimePeriodStart.ToEndOfMonth();
					break;
				case TimePeriodType.DailyHourly:
				case TimePeriodType.DailyHalfHourly:
				case TimePeriodType.DailyQuaterHourly:
				default:
					startTime = TimePeriodStart.ToMidnight();
					endTime = TimePeriodStart.To1159PM();
					break;
			}


			switch (ColumnFilter) {
				case AppointmentBookViewModelFilter.All:
					_visibleColumns = _allColumns;
					break;
				case AppointmentBookViewModelFilter.ExcludeEmptyColumns:
					_visibleColumns = (
						from appointment in _allColumns
						where appointment.Appointments.Any(block => block.VisibleStartTime < endTime && block.VisibleEndTime > startTime)
						select appointment
					).ToArray();
					break;
				case AppointmentBookViewModelFilter.ShowOnlyEmptyColumns:
					_visibleColumns = (
						from appointment in _allColumns
						where !appointment.Appointments.Any(block => block.VisibleStartTime <= endTime && block.VisibleEndTime >= startTime)
						select appointment
					).ToArray();
					;
					break;
			}

			RecreateCellDisplays();
		} catch (Exception error) {
			SystemLog.Exception(error);
			ExceptionDialog.Show(error);
		}
	}

}
