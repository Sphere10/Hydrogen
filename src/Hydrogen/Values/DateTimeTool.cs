// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Time {

	public static DateTime CalculateNextDayOfWeek(DateTime fromDate, DayOfWeek dayOfWeek) {
		//Sunday    1  0
		//Monday    2  1 
		//Tuesday   3  2
		//Wednesday 4  3
		//Thursday  5  4
		//Friday    6  5
		//Saturday  7  6
		var startDayOfWeek = (int)(fromDate.DayOfWeek) - 1;
		var destDayOfWeek = (int)dayOfWeek - 1;

		return destDayOfWeek > startDayOfWeek ? fromDate.AddDays(destDayOfWeek - startDayOfWeek).ToMidnight() : fromDate.AddDays((7 + destDayOfWeek) - startDayOfWeek).ToMidnight();
		;

	}

	public static DateTime CalculateNextDayOfWeek(DateTime fromDate, DayOfWeek dayOfWeek, TimeSpan timeOfDay) {
		DateTime date;
		if (fromDate.DayOfWeek == dayOfWeek && fromDate.TimeOfDay < timeOfDay) {
			date = fromDate;
		} else {
			date = CalculateNextDayOfWeek(fromDate, dayOfWeek);
		}
		return date.ToMidnight().Add(timeOfDay);

	}


	public static DateTime CalculateNextDayOfMonth(DateTime fromDate, int dayOfMonth) {
		if (fromDate.Day < dayOfMonth) {
			return new DateTime(fromDate.Year, fromDate.Month, dayOfMonth, fromDate.Hour, fromDate.Minute, fromDate.Second, fromDate.Millisecond, fromDate.Kind);
		}
		return
			fromDate
				.ToEndOfMonth()
				.AddDays(1)
				.ToBeginningOfMonth()
				.AddDays(dayOfMonth - 1)
				.ToMidnight()
				.Add(fromDate.TimeOfDay);
	}


	public static DateTime CalculateNextDayOfMonth(DateTime fromDate, int dayOfMonth, TimeSpan timeOfDay) {
		DateTime date;
		if (fromDate.Day < dayOfMonth || (fromDate.Day == dayOfMonth && fromDate.TimeOfDay < timeOfDay)) {
			date = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0, 0);
		} else {
			date = CalculateNextDayOfMonth(fromDate, dayOfMonth).ToMidnight();
		}
		return date.Add(timeOfDay);
	}

	public static DateTime DateFrom1979Jan1(uint milliseconds) {
		return new DateTime(1979, 1, 1).AddMilliseconds(milliseconds);
	}


	public static DateTime FromUnixTime(uint unixTimeStamp) {
		// Unix timestamp is seconds past epoch
		DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
		return dtDateTime;
	}

	/// <summary>
	/// Return the ordinal suffix for the specified day of the month.
	/// Such as 'st' for 1st or 'rd' for 3rd.
	/// </summary>
	public static string GetOrdinalDaySuffix(int dayNumber) {
		// Return: st, nd, rd, th, ...

		if (((dayNumber % 100) / 10) != 1) {
			int modResult = dayNumber % 10;

			if (modResult == 1)
				return "st";
			else if (modResult == 2)
				return "nd";
			else if (modResult == 3)
				return "rd";
		}

		return "th";
	}


	public static string ConvertMonth(int month) {
		switch (month) {
			case 1:
				return "January";
			case 2:
				return "February";
			case 3:
				return "March";
			case 4:
				return "April";
			case 5:
				return "May";
			case 6:
				return "June";
			case 7:
				return "July";
			case 8:
				return "August";
			case 9:
				return "September";
			case 10:
				return "October";
			case 11:
				return "November";
			case 12:
				return "December";
		}
		throw new ArgumentOutOfRangeException(nameof(month), "Must  be between 1 and 12 (inclusive)");
	}

	public static DateTime Max(DateTime date1, DateTime date2) {
		return date1 >= date2 ? date1 : date2;
	}

	public static DateTime? Max(DateTime? date1, DateTime? date2) {
		if (date1.HasValue && !date2.HasValue)
			return date1;

		if (!date1.HasValue && date2.HasValue)
			return date2;

		if (!date1.HasValue)
			return null;

		return date1.Value >= date2.Value ? date1.Value : date2.Value;
	}

	public static DateTime Min(DateTime date1, DateTime date2) {
		return date1 <= date2 ? date1 : date2;
	}

}
