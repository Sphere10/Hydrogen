// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;


namespace Hydrogen;

/// <summary>
/// Extension methods for <see cref="DataTime"/>
/// </summary>
public static class DateTimeExtensions {
	public const int ApproxEqualToleranceMS = 250;

	public static bool ApproxEqual(this DateTime date, DateTime testDate, TimeSpan? tolerance = null) {
		if (tolerance == null)
			tolerance = TimeSpan.FromMilliseconds(ApproxEqualToleranceMS);
		return date.Subtract(testDate).Duration().CompareTo(tolerance) <= 0;
	}
	public static DateTime RoundToSecond(this DateTime date) {
		return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
	}

	public static TimeSpan TimeElapsed(this DateTime date) {
		return DateTime.Now.Subtract(date);
	}

	public static DateTime? ToNullable(this DateTime date) {
		if (date == new DateTime())
			return null;
		return date;
	}

	public static uint MilliSecondsSince1979Jan1(this DateTime toDate) {
		return Tools.Values.ClipValue((uint)(toDate - new DateTime(1970, 1, 1)).TotalMilliseconds, 0, uint.MaxValue);
	}
	public static uint ToUnixTime(this DateTime toDate) {
		return Tools.Values.ClipValue((uint)(toDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0, uint.MaxValue);
	}

	public static string ToShortDateString(this DateTime? dateTime) {
		if (dateTime == null)
			return String.Empty;
		else
			return ((DateTime)dateTime).ToShortDateString();
	}

	public static string ToString(this DateTime? dateTime, string format) {
		if (dateTime == null)
			return String.Empty;
		else
			return ((DateTime)dateTime).ToString(format);
	}

	public static DateTime ToNextDay(this DateTime dateTime) {
		return dateTime.AddDays(1);
	}

	public static DateTime ToNextDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek) {
		return Tools.Time.CalculateNextDayOfWeek(dateTime, dayOfWeek);
	}

	public static DateTime ToNextDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek, TimeSpan timeOfDay) {
		return Tools.Time.CalculateNextDayOfWeek(dateTime, dayOfWeek, timeOfDay);
	}

	public static DateTime ToNextDayOfMonth(this DateTime dateTime, int dayOfMonth) {
		return Tools.Time.CalculateNextDayOfMonth(dateTime, dayOfMonth);
	}

	public static DateTime ToNextDayOfMonth(this DateTime dateTime, int dayOfMonth, TimeSpan timeOfDay) {
		return Tools.Time.CalculateNextDayOfMonth(dateTime, dayOfMonth, timeOfDay);
	}

	public static DateTime ToMidnight(this DateTime dateTime) {
		return dateTime.SetTime(0, 0, 0, 0);
		//return dateTime.Subtract(
		//	TimeSpan.FromSeconds(
		//		dateTime.Hour * 3600 +
		//		dateTime.Minute * 60 +
		//		dateTime.Second +
		//		(double)dateTime.Millisecond / 100.0D
		//	)
		//	);
	}

	public static DateTime To1159PM(this DateTime dateTime) {
		return dateTime.ToMidnight().ToNextDay().Subtract(TimeSpan.FromSeconds(1));
	}

	public static DateTime ToDayOfMonth(this DateTime dateTime, int day) {
		return dateTime.SetDate(dateTime.Year, dateTime.Month, day);
	}

	public static DateTime ToStartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday) {
		int diff = dt.DayOfWeek - startOfWeek;
		if (diff < 0) {
			diff += 7;
		}

		return dt.AddDays(-1 * diff).Date;
	}


	/// <summary>
	///  Returns first day of the month of the given date.
	/// </summary>
	/// <param name="dateTime"></param>
	/// <returns></returns>
	public static DateTime ToFirstDayOfMonth(this DateTime dateTime) {
		return DateTime.Parse(
			string.Format(
				"{0}-{1:00}-01 12:00:00.00",
				dateTime.Year,
				dateTime.Month
			)
		);
	}

	/// <summary>
	/// Returns last day of the month of the given date.
	/// </summary>
	/// <param name="dateTime"></param>
	/// <returns></returns>
	public static DateTime ToLastDayOfMonth(this DateTime dateTime) {
		return
			DateTime.Parse(
					string.Format(
						"{0}-{1}-28 12:00:00.00",
						dateTime.Year,
						dateTime.Month
					)
				)
				.AddDays(10.0)
				.ToFirstDayOfMonth()
				.Subtract(TimeSpan.FromHours(24.0));
	}

	public static DateTime ToPreviousMonth(this DateTime dateTime) {
		return
			dateTime.Subtract(TimeSpan.FromDays(dateTime.Day + 1))
				.ToFirstDayOfMonth();
	}

	public static DateTime? ToMidnightIfHasValue(this DateTime? dateTime) {
		DateTime? retval = new DateTime?();
		if (dateTime.HasValue) {
			retval = new DateTime?(dateTime.Value.ToMidnight());
		}
		return retval;
	}

	public static string ToApplicationDateString(this DateTime dateTime) {
		return dateTime.ToString("dd/MM/yyyy");
	}

	public static string ToApplicationDateStringIfApplicable(this DateTime? dateTime) {
		string retval = string.Empty;
		if (dateTime.HasValue) {
			retval = dateTime.Value.ToApplicationDateString();
		}
		return retval;
	}


	public static string ToApplicationLongDateString(this DateTime dateTime) {
		return
			string.Format(
				"{0} {1}, {2}",
				dateTime.ToFullMonthStringEN(),
				dateTime.Day,
				dateTime.Year
			);
	}


	public static string ToFullMonthStringEN(this DateTime dateTime) {
		string retval = string.Empty;
		switch (dateTime.Month) {
			case 1:
				retval = "January";
				break;
			case 2:
				retval = "Febuary";
				break;
			case 3:
				retval = "March";
				break;
			case 4:
				retval = "April";
				break;
			case 5:
				retval = "May";
				break;
			case 6:
				retval = "June";
				break;
			case 7:
				retval = "July";
				break;
			case 8:
				retval = "August";
				break;
			case 9:
				retval = "September";
				break;
			case 10:
				retval = "October";
				break;
			case 11:
				retval = "November";
				break;
			case 12:
				retval = "December";
				break;
			default:
				throw new ApplicationException("[Internal error] 7A0EA040-60F3-4A08-BC11-617113EAB12D");
		}
		return retval;
	}


	public static DateTime? Parse(string date) {
		if (!string.IsNullOrEmpty(date)) {
			return DateTime.Parse(date);
		}
		return null;
	}

	/// <summary>
	/// Adds the business days.
	/// </summary>
	/// <param name="date">The date.</param>
	/// <param name="days">The days.</param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static DateTime AddBusinessDays(this DateTime date, int days) {
		double sign = Convert.ToDouble(Math.Sign(days));
		int unsignedDays = Math.Sign(days) * days;
		for (int i = 0; i < unsignedDays; i++) {
			do {
				date = date.AddDays(sign);
			} while (date.DayOfWeek == DayOfWeek.Saturday ||
			         date.DayOfWeek == DayOfWeek.Sunday);
		}
		return date;
	}


	/// <summary>
	/// Fiscals the year.
	/// </summary>
	/// <param name="dateTime">The date time.</param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static int FiscalYear(this DateTime dateTime) {
		return dateTime.Month >= 7 ? dateTime.Year + 1 : dateTime.Year;
	}

	/// Returns the first day of week with in the month.
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="dow">What day of week to find the first one of in the month.</param>
	/// <returns>Returns DateTime object that represents the first day of week with in the month.</returns>
	public static DateTime FirstDayOfWeekInMonth(this DateTime obj, DayOfWeek dow) {
		DateTime firstDay = new DateTime(obj.Year, obj.Month, 1);
		int diff = firstDay.DayOfWeek - dow;
		if (diff > 0) diff -= 7;
		return firstDay.AddDays(diff * -1);
	}

	/// <summary>
	/// Returns the first weekday (Financial day) of the month
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Returns DateTime object that represents the first weekday (Financial day) of the month</returns>
	public static DateTime FirstWeekDayOfMonth(this DateTime obj) {
		DateTime firstDay = new DateTime(obj.Year, obj.Month, 1);
		for (int i = 0; i < 7; i++) {
			if (firstDay.AddDays(i).DayOfWeek != DayOfWeek.Saturday && firstDay.AddDays(i).DayOfWeek != DayOfWeek.Sunday)
				return firstDay.AddDays(i);
		}
		return firstDay;
	}

	/// <summary>
	/// Returns the last day of week with in the month.
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="dow">What day of week to find the last one of in the month.</param>
	/// <returns>Returns DateTime object that represents the last day of week with in the month.</returns>
	public static DateTime LastDayOfWeekInMonth(this DateTime obj, DayOfWeek dow) {
		DateTime lastDay = new DateTime(obj.Year, obj.Month, DateTime.DaysInMonth(obj.Year, obj.Month));
		DayOfWeek lastDow = lastDay.DayOfWeek;

		int diff = dow - lastDow;
		if (diff > 0) diff -= 7;

		return lastDay.AddDays(diff);
	}

	/// <summary>
	/// Returns the last weekday (Financial day) of the month
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Returns DateTime object that represents the last weekday (Financial day) of the month</returns>
	public static DateTime LastWeekDayOfMonth(this DateTime obj) {
		DateTime lastDay = new DateTime(obj.Year, obj.Month, DateTime.DaysInMonth(obj.Year, obj.Month));
		for (int i = 0; i < 7; i++) {
			if (lastDay.AddDays(i * -1).DayOfWeek != DayOfWeek.Saturday && lastDay.AddDays(i * -1).DayOfWeek != DayOfWeek.Sunday)
				return lastDay.AddDays(i * -1);
		}
		return lastDay;
	}

	/// <summary>
	/// Returns the closest Weekday (Financial day) Date
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Returns the closest Weekday (Financial day) Date</returns>
	public static DateTime FindClosestWeekDay(this DateTime obj) {
		if (obj.DayOfWeek == DayOfWeek.Saturday)
			return obj.AddDays(-1);
		if (obj.DayOfWeek == DayOfWeek.Sunday)
			return obj.AddDays(1);
		else
			return obj;
	}

	/// <summary>
	/// Returns the very end of the given month (the last millisecond of the last hour for the given date)
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Returns the very end of the given month (the last millisecond of the last hour for the given date)</returns>
	public static DateTime ToEndOfMonth(this DateTime obj) {
		return new DateTime(obj.Year, obj.Month, DateTime.DaysInMonth(obj.Year, obj.Month), 23, 59, 59, 999);
	}

	/// <summary>
	/// Returns the Start of the given month (the fist millisecond of the given date)
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Returns the Start of the given month (the fist millisecond of the given date)</returns>
	public static DateTime ToBeginningOfMonth(this DateTime obj) {
		return new DateTime(obj.Year, obj.Month, 1, 0, 0, 0, 0);
	}

	/// <summary>
	/// Returns the very end of the given day (the last millisecond of the last hour for the given date)
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Returns the very end of the given day (the last millisecond of the last hour for the given date)</returns>
	public static DateTime ToEndOfDay(this DateTime obj) {
		return obj.SetTime(23, 59, 59, 999);
	}

	/// <summary>
	/// Returns the Start of the given day (the fist millisecond of the given date)
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Returns the Start of the given day (the fist millisecond of the given date)</returns>
	public static DateTime BeginningOfDay(this DateTime obj) {
		return obj.SetTime(0, 0, 0, 0);
	}

	/// <summary>
	/// Returns a given datetime according to the week of year and the specified day within the week.
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="week">A number of whole and fractional weeks. The value parameter can only be positive.</param>
	/// <param name="dayofweek">A DayOfWeek to find in the week</param>
	/// <returns>A DateTime whose value is the sum according to the week of year and the specified day within the week.</returns>
	public static DateTime GetDateByWeek(this DateTime obj, int week, DayOfWeek dayofweek) {
		if (week > 0 && week < 54) {
			DateTime FirstDayOfyear = new DateTime(obj.Year, 1, 1);
			int daysToFirstCorrectDay = (((int)dayofweek - (int)FirstDayOfyear.DayOfWeek) + 7) % 7;
			return FirstDayOfyear.AddDays(7 * (week - 1) + daysToFirstCorrectDay);
		} else
			return obj;
	}

	private static int Sub(DayOfWeek s, DayOfWeek e) {
		if ((s - e) > 0) return (s - e) - 7;
		if ((s - e) == 0) return -7;
		return (s - e);
	}

	/// <summary>
	/// Returns next "first" occurence of specified DayOfTheWeek
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="day">A DayOfWeek to find the previous occurence of</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the enum value represented by the day.</returns>
	public static DateTime Previous(this DateTime obj, DayOfWeek day) {
		return obj.AddDays(Sub(day, obj.DayOfWeek));
	}

	private static DateTime SetDateWithChecks(DateTime obj, int year, int month, int day, int? hour, int? minute, int? second, int? millisecond) {
		DateTime StartDate;

		if (year == 0)
			StartDate = new DateTime(obj.Year, 1, 1, 0, 0, 0, 0);
		else {
			if (DateTime.MaxValue.Year < year)
				StartDate = new DateTime(DateTime.MinValue.Year, 1, 1, 0, 0, 0, 0);
			else if (DateTime.MinValue.Year > year)
				StartDate = new DateTime(DateTime.MaxValue.Year, 1, 1, 0, 0, 0, 0);
			else
				StartDate = new DateTime(year, 1, 1, 0, 0, 0, 0);
		}

		if (month == 0)
			StartDate = StartDate.AddMonths(obj.Month - 1);
		else
			StartDate = StartDate.AddMonths(month - 1);
		if (day == 0)
			StartDate = StartDate.AddDays(obj.Day - 1);
		else
			StartDate = StartDate.AddDays(day - 1);
		if (!hour.HasValue)
			StartDate = StartDate.AddHours(obj.Hour);
		else
			StartDate = StartDate.AddHours(hour.Value);
		if (!minute.HasValue)
			StartDate = StartDate.AddMinutes(obj.Minute);
		else
			StartDate = StartDate.AddMinutes(minute.Value);
		if (!second.HasValue)
			StartDate = StartDate.AddSeconds(obj.Second);
		else
			StartDate = StartDate.AddSeconds(second.Value);
		if (!millisecond.HasValue)
			StartDate = StartDate.AddMilliseconds(obj.Millisecond);
		else
			StartDate = StartDate.AddMilliseconds(millisecond.Value);

		return StartDate;
	}

	/// <summary>
	/// Returns the original DateTime with Hour part changed to supplied hour parameter
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="hour">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the numbers represented by the parameters.</returns>
	public static DateTime SetTime(this DateTime obj, int hour) {
		return SetDateWithChecks(obj, 0, 0, 0, hour, null, null, null);
	}

	/// <summary>
	/// Returns the original DateTime with Hour and Minute parts changed to supplied hour and minute parameters
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="hour">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
	/// <param name="minute">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the numbers represented by the parameters.</returns>
	public static DateTime SetTime(this DateTime obj, int hour, int minute) {
		return SetDateWithChecks(obj, 0, 0, 0, hour, minute, null, null);
	}

	/// <summary>
	/// Returns the original DateTime with Hour, Minute and Second parts changed to supplied hour, minute and second parameters
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="hour">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
	/// <param name="minute">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
	/// <param name="second">A number of whole and fractional seconds. The value parameter can be negative or positive.</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the numbers represented by the parameters.</returns>
	public static DateTime SetTime(this DateTime obj, int hour, int minute, int second) {
		return SetDateWithChecks(obj, 0, 0, 0, hour, minute, second, null);
	}

	/// <summary>
	/// Returns the original DateTime with Hour, Minute, Second and Millisecond parts changed to supplied hour, minute, second and millisecond parameters
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="hour">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
	/// <param name="minute">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
	/// <param name="second">A number of whole and fractional seconds. The value parameter can be negative or positive.</param>
	/// <param name="millisecond">A number of whole and fractional milliseconds. The value parameter can be negative or positive.</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the numbers represented by the parameters.</returns>
	public static DateTime SetTime(this DateTime obj, int hour, int minute, int second, int millisecond) {
		return SetDateWithChecks(obj, 0, 0, 0, hour, minute, second, millisecond);
	}

	/// <summary>
	/// Returns DateTime with changed Year part
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="year">A number of whole and fractional years. The value parameter can be negative or positive.</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the numbers represented by the parameters.</returns>
	public static DateTime SetDate(this DateTime obj, int year) {
		return SetDateWithChecks(obj, year, 0, 0, null, null, null, null);
	}

	/// <summary>
	/// Returns DateTime with changed Year and Month part
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="year">A number of whole and fractional years. The value parameter can be negative or positive.</param>
	/// <param name="month">A number of whole and fractional month. The value parameter can be negative or positive.</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the numbers represented by the parameters.</returns>
	public static DateTime SetDate(this DateTime obj, int year, int month) {
		return SetDateWithChecks(obj, year, month, 0, null, null, null, null);
	}

	/// <summary>
	/// Returns DateTime with changed Year, Month and Day part
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="year">A number of whole and fractional years. The value parameter can be negative or positive.</param>
	/// <param name="month">A number of whole and fractional month. The value parameter can be negative or positive.</param>
	/// <param name="day">A number of whole and fractional day. The value parameter can be negative or positive.</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the numbers represented by the parameters.</returns>
	public static DateTime SetDate(this DateTime obj, int year, int month, int day) {
		return SetDateWithChecks(obj, year, month, day, null, null, null, null);
	}

	public static DateTime SetDateTime(this DateTime obj, int year, int month, int day, int hour, int minute, int second, int millisecond) {
		return SetDateWithChecks(obj, year, month, day, hour, minute, second, millisecond);
	}

	/// <summary>
	/// Adds the specified number of financials days to the value of this instance.
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="days">A number of whole and fractional financial days. The value parameter can be negative or positive.</param>
	/// <returns>A DateTime whose value is the sum of the date and time represented by this instance and the number of financial days represented by days.</returns>
	public static DateTime AddFinancialDays(this DateTime obj, int days) {
		int addint = Math.Sign(days);
		for (int i = 0; i < (Math.Sign(days) * days); i++) {
			do {
				obj = obj.AddDays(addint);
			} while (obj.IsWeekend());
		}
		return obj;
	}

	/// <summary>
	/// Calculate Financial days between two dates.
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <param name="otherdate">End or start date to calculate to or from.</param>
	/// <returns>Amount of financial days between the two dates</returns>
	public static int CountFinancialDays(this DateTime obj, DateTime otherdate) {
		TimeSpan ts = (otherdate - obj);
		int addint = Math.Sign(ts.Days);
		int unsigneddays = (Math.Sign(ts.Days) * ts.Days);
		int businessdays = 0;
		for (int i = 0; i < unsigneddays; i++) {
			obj = obj.AddDays(addint);
			if (!obj.IsWeekend())
				businessdays++;
		}
		return businessdays;
	}

	/// <summary>
	/// Converts any datetime to the amount of seconds from 1972.01.01 00:00:00
	/// Microsoft sometimes uses the amount of seconds from 1972.01.01 00:00:00 to indicate an datetime.
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Total seconds past since 1972.01.01 00:00:00</returns>
	public static double ToMicrosoftNumber(this DateTime obj) {
		return (obj - new DateTime(1972, 1, 1, 0, 0, 0, 0)).TotalSeconds;
	}

	/// <summary>
	/// Returns true if the day is Saturday or Sunday
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>boolean value indicating if the date is a weekend</returns>
	public static bool IsWeekend(this DateTime obj) {
		return (obj.DayOfWeek == DayOfWeek.Saturday || obj.DayOfWeek == DayOfWeek.Sunday);
	}

	/// <summary>
	/// Get the quarter that the datetime is in.
	/// </summary>
	/// <param name="obj">DateTime Base, from where the calculation will be preformed.</param>
	/// <returns>Returns 1 to 4 that represenst the quarter that the datetime is in.</returns>
	public static int Quarter(this DateTime obj) {
		return ((obj.Month - 1) / 3) + 1;
	}


	/// <summary>
	/// Gets a DateTime representing the first day in the current month
	/// </summary>
	/// <param name="current">The current date</param>
	/// <returns></returns>
	public static DateTime First(this DateTime current) {
		DateTime first = current.AddDays(1 - current.Day);
		return first;
	}

	/// <summary>
	/// Gets a DateTime representing the first specified day in the current month
	/// </summary>
	/// <param name="current">The current day</param>
	/// <param name="dayOfWeek">The current day of week</param>
	/// <returns></returns>
	public static DateTime First(this DateTime current, DayOfWeek dayOfWeek) {
		DateTime first = current.First();

		if (first.DayOfWeek != dayOfWeek) {
			first = first.Next(dayOfWeek);
		}

		return first;
	}

	/// <summary>
	/// Gets a DateTime representing the last day in the current month
	/// </summary>
	/// <param name="current">The current date</param>
	/// <returns></returns>
	public static DateTime Last(this DateTime current) {
		int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);

		DateTime last = current.First().AddDays(daysInMonth - 1);
		return last;
	}

	/// <summary>
	/// Gets a DateTime representing the last specified day in the current month
	/// </summary>
	/// <param name="current">The current date</param>
	/// <param name="dayOfWeek">The current day of week</param>
	/// <returns></returns>
	public static DateTime Last(this DateTime current, DayOfWeek dayOfWeek) {
		DateTime last = current.Last();

		last = last.AddDays(Math.Abs(dayOfWeek - last.DayOfWeek) * -1);
		return last;
	}

	/// <summary>
	/// Gets a DateTime representing the first date following the current date which falls on the given day of the week
	/// </summary>
	/// <param name="current">The current date</param>
	/// <param name="dayOfWeek">The day of week for the next date to get</param>
	public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek) {
		int offsetDays = dayOfWeek - current.DayOfWeek;

		if (offsetDays <= 0) {
			offsetDays += 7;
		}

		DateTime result = current.AddDays(offsetDays);
		return result;
	}


	/// <summary>
	/// Gets a DateTime representing midnight on the current date
	/// </summary>
	/// <param name="current">The current date</param>
	public static DateTime Midnight(this DateTime current) {
		DateTime midnight = new DateTime(current.Year, current.Month, current.Day);
		return midnight;
	}

	/// <summary>
	/// Gets a DateTime representing noon on the current date
	/// </summary>
	/// <param name="current">The current date</param>
	public static DateTime Noon(this DateTime current) {
		DateTime noon = new DateTime(current.Year, current.Month, current.Day, 12, 0, 0);
		return noon;
	}


	public static bool IsSameDay(this DateTime current, DateTime otherDate) {
		return current.Year == otherDate.Year && current.Month == otherDate.Month && current.Day == otherDate.Day;
	}

	public static bool IsSameDayOfYear(this DateTime current, DateTime otherDate) {
		return current.Month == otherDate.Month && current.Day == otherDate.Day;
	}

	public static bool IsSameDayOfMonth(this DateTime current, DateTime otherDate) {
		return current.Day == otherDate.Day;
	}

	public static bool InSameWeek(this DateTime a, DateTime b) {
		return a.AddDays(7 - (int)a.DayOfWeek).Date.Equals(b.AddDays(7 - (int)b.DayOfWeek).Date);
	}

	public static bool InLastWeek(this DateTime a, DateTime b) {
		return a.AddDays(7 - (int)a.DayOfWeek).Date.Equals(b.AddDays(-(int)b.DayOfWeek).Date);
	}

	/// <summary>
	/// Try to convert a string into a nullable DateTime. If the string cannot be converted
	/// the default value is returned.
	/// </summary>
	/// <param name="value">The string to parse.</param>
	/// <param name="defaultValue">The value to return if the string could not be converted.</param>
	/// <returns>A nullable DateTime if the input was valid; otherwise, defaultValue.</returns>
	public static DateTime? ToDateTimeOrDefault(this string value, DateTime? defaultValue = null) {
		if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParse(value, out var result)) {
			return result;
		}

		return defaultValue;
	}
}
