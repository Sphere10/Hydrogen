// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public abstract class BaseAppointmentBookViewModel {
	private TimePeriodType _timeView;
	private DateTime _timePeriodStart;
	private DateTime _timePeriodEnd;

	public BaseAppointmentBookViewModel(DateTime timePeriodStart, TimePeriodType timeView) {
		Initialize(timePeriodStart, timeView);
	}

	protected void Initialize(DateTime timePeriodStart, TimePeriodType timeView) {
		_timeView = timeView;
		switch (timeView) {
			case TimePeriodType.Monthly:
				_timePeriodStart = timePeriodStart.ToBeginningOfMonth();
				_timePeriodEnd = timePeriodStart.AddMonths(1).ToBeginningOfMonth();
				break;
			default:
				_timePeriodStart = timePeriodStart.ToMidnight();
				_timePeriodEnd = timePeriodStart.ToNextDay().ToMidnight();
				break;
		}
	}

	public TimePeriodType TimeView {
		get { return _timeView; }
	}

	public virtual DateTime TimePeriodStart {
		get { return _timePeriodStart; }
	}

	public virtual DateTime TimePeriodEnd {
		get { return _timePeriodEnd; }
	}

	public void SetTimePeriod(TimePeriodType timePeriodType, DateTime timePeriodStart) {
		_timeView = timePeriodType;
		_timePeriodStart = timePeriodStart;
		RecreateCellDisplays();
	}

	public void SetTimePeriodType(TimePeriodType timePeriodType) {
		_timeView = timePeriodType;
		RecreateCellDisplays();
	}


	internal Tuple<string, string> GetRowHeaderText(int row) {
		var time = RowToStartTime(row);
		switch (TimeView) {
			case TimePeriodType.Monthly:
				return Tuple.Create(string.Format("{0:MM}", time), string.Format("{0:dd}", time));
			default:
				return Tuple.Create(string.Format("{0:HH}", time), string.Format("{0:mm}", time));
		}
	}

	internal abstract string GetColumnHeaderText(int col);

	internal abstract int GetColumnCount();

	internal virtual int GetRowCount() {
		int rowCount = 0;
		switch (TimeView) {
			case TimePeriodType.Monthly:
				// row denotes day in month
				rowCount = TimePeriodStart.ToLastDayOfMonth().Day;
				break;
			case TimePeriodType.DailyHourly:
				// row denotes hour of day
				rowCount = 24;
				break;
			case TimePeriodType.DailyHalfHourly:
				// row denotes 30'th minute of day
				rowCount = 24 * 2;
				break;
			case TimePeriodType.DailyQuaterHourly:
				// row denotes 15'th minute of 
				rowCount = 24 * 4;
				break;
			default:
				throw new SoftwareException("Internal error. Unsupported time period type {0}", TimeView);

		}

		return rowCount;
	}

	internal DateTime RowToStartTime(int row) {
		DateTime time;
		switch (TimeView) {
			case TimePeriodType.Monthly:
				// row denotes day in month
				time = TimePeriodStart.ToFirstDayOfMonth().ToMidnight().AddDays(row);
				break;
			case TimePeriodType.DailyHourly:
				// row denotes hour of day
				time = TimePeriodStart.ToMidnight().AddHours(row);
				break;
			case TimePeriodType.DailyHalfHourly:
				// row denotes 30'th minute of day
				time = TimePeriodStart.ToMidnight().AddMinutes(row * 30.0);
				break;
			case TimePeriodType.DailyQuaterHourly:
				// row denotes 15'th minute of 
				time = TimePeriodStart.ToMidnight().AddMinutes(row * 15.0);
				break;
			default:
				throw new SoftwareException("Internal error. Unsupported time period type {0}", TimeView);

		}
		return time;
	}

	internal DateTime RowToEndTime(int row) {
		var endTime = RowToStartTime(row);
		switch (TimeView) {
			case TimePeriodType.DailyQuaterHourly:
				endTime = endTime.AddMinutes(15);
				break;
			case TimePeriodType.DailyHalfHourly:
				endTime = endTime.AddMinutes(30);
				break;
			case TimePeriodType.DailyHourly:
				endTime = endTime.AddHours(1);
				break;
			case TimePeriodType.Monthly:
			default:
				break;
		}
		return endTime;
	}

	internal int StartTimeToRow(DateTime time) {
		int row = -1;
		switch (TimeView) {
			case TimePeriodType.Monthly:
				// row denotes day in month
				row = (int)Math.Floor(time.Subtract(TimePeriodStart.ToFirstDayOfMonth().ToMidnight()).TotalDays);
				break;
			case TimePeriodType.DailyHourly:
				// row denotes hour of day
				row = (int)Math.Ceiling(time.Subtract(TimePeriodStart.ToMidnight()).TotalHours);
				break;
			case TimePeriodType.DailyHalfHourly:
				// row denotes 30'th minute of day
				row = (int)Math.Ceiling(time.Subtract(TimePeriodStart.ToMidnight()).TotalHours * 2.0f);
				break;
			case TimePeriodType.DailyQuaterHourly:
				// row denotes 15'th minute of 
				row = (int)Math.Ceiling(time.Subtract(TimePeriodStart.ToMidnight()).TotalHours * 4.0f);
				break;
			default:
				throw new SoftwareException("Internal error. Unsupported time period type {0}", TimeView);
		}
		return row;
	}

	internal int FinishTimeToRow(DateTime time) {
		return TimeView != TimePeriodType.Monthly ? StartTimeToRow(time) - 1 : StartTimeToRow(time);
	}

	internal abstract CellViewModel GetCellDisplay(int col, int row);

	protected abstract void RecreateCellDisplays();
}
