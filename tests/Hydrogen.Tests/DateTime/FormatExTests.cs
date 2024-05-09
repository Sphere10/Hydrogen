// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class DateTimeTests {

	[Test]
	public void NextDayOfWeek_Tomorrow() {
		var startDayTime = new DateTime(2014, 09, 29); // Monday
		var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Tuesday);
		ClassicAssert.AreEqual(DayOfWeek.Tuesday, nextDayOfWeek.DayOfWeek);
		ClassicAssert.AreEqual(30, nextDayOfWeek.Day);
		ClassicAssert.AreEqual(9, nextDayOfWeek.Month);
		ClassicAssert.AreEqual(2014, nextDayOfWeek.Year);
	}

	[Test]
	public void NextDayOfWeek_NextMonday() {
		var startDayTime = new DateTime(2014, 09, 29); // Monday
		var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Monday);
		ClassicAssert.AreEqual(DayOfWeek.Monday, nextDayOfWeek.DayOfWeek);
		ClassicAssert.AreEqual(6, nextDayOfWeek.Day);
		ClassicAssert.AreEqual(10, nextDayOfWeek.Month);
		ClassicAssert.AreEqual(2014, nextDayOfWeek.Year);
	}

	[Test]
	public void NextDayOfWeek_NextSunday() {
		var startDayTime = new DateTime(2014, 09, 29); // Monday
		var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Sunday);
		ClassicAssert.AreEqual(DayOfWeek.Sunday, nextDayOfWeek.DayOfWeek);
		ClassicAssert.AreEqual(5, nextDayOfWeek.Day);
		ClassicAssert.AreEqual(10, nextDayOfWeek.Month);
		ClassicAssert.AreEqual(2014, nextDayOfWeek.Year);
	}


	[Test]
	public void NextDayOfWeek_1HourAhead() {
		var startDayTime = new DateTime(2014, 09, 29, 11, 0, 0, 0); // Monday
		var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Monday, TimeSpan.FromHours(12));
		ClassicAssert.AreEqual(nextDayOfWeek.DayOfWeek, startDayTime.DayOfWeek);
		ClassicAssert.AreEqual(nextDayOfWeek.Day, startDayTime.Day);
		ClassicAssert.AreEqual(nextDayOfWeek.Month, startDayTime.Month);
		ClassicAssert.AreEqual(nextDayOfWeek.Year, startDayTime.Year);
		ClassicAssert.AreEqual(12, nextDayOfWeek.Hour);
	}

	[Test]
	public void NextDayOfWeek_168HoursAhead() {
		var startDayTime = new DateTime(2014, 09, 29, 12, 0, 0, 0); // Monday
		var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Monday, TimeSpan.FromHours(12));
		ClassicAssert.AreEqual(nextDayOfWeek.DayOfWeek, startDayTime.DayOfWeek);
		ClassicAssert.AreEqual(6, nextDayOfWeek.Day);
		ClassicAssert.AreEqual(10, nextDayOfWeek.Month);
		ClassicAssert.AreEqual(2014, nextDayOfWeek.Year);
		ClassicAssert.AreEqual(12, nextDayOfWeek.Hour);
	}


	[Test]
	public void NextDayOfMonth_NextMonth() {
		var start = new DateTime(2014, 09, 29); // Monday
		var nextDayOfMonth = start.ToNextDayOfMonth(29);
		ClassicAssert.AreEqual(DayOfWeek.Wednesday, nextDayOfMonth.DayOfWeek);
		ClassicAssert.AreEqual(29, nextDayOfMonth.Day);
		ClassicAssert.AreEqual(10, nextDayOfMonth.Month);
		ClassicAssert.AreEqual(2014, nextDayOfMonth.Year);
	}


	[Test]
	public void NextDayOfMonth_Tomorrow() {
		var start = new DateTime(2014, 09, 29); // Monday
		var nextDayOfMonth = start.ToNextDayOfMonth(30);
		ClassicAssert.AreEqual(DayOfWeek.Tuesday, nextDayOfMonth.DayOfWeek);
		ClassicAssert.AreEqual(30, nextDayOfMonth.Day);
		ClassicAssert.AreEqual(9, nextDayOfMonth.Month);
		ClassicAssert.AreEqual(2014, nextDayOfMonth.Year);
	}

	[Test]
	public void NextDayOfMonth_1HourAhead() {
		var start = new DateTime(2014, 09, 29, 11, 0, 0, 0); // Monday
		var nextDayOfMonth = start.ToNextDayOfMonth(29, TimeSpan.FromHours(12));
		ClassicAssert.AreEqual(nextDayOfMonth.DayOfWeek, start.DayOfWeek);
		ClassicAssert.AreEqual(nextDayOfMonth.Day, start.Day);
		ClassicAssert.AreEqual(nextDayOfMonth.Month, start.Month);
		ClassicAssert.AreEqual(nextDayOfMonth.Year, start.Year);
		ClassicAssert.AreEqual(12, nextDayOfMonth.Hour);
	}

	[Test]
	public void NextDayOfMonth_MonthsHoursAhead() {
		var start = new DateTime(2014, 09, 29, 12, 0, 0, 0); // Monday
		var nextDayOfMonth = start.ToNextDayOfMonth(29, TimeSpan.FromHours(12));
		ClassicAssert.AreEqual(29, nextDayOfMonth.Day);
		ClassicAssert.AreEqual(10, nextDayOfMonth.Month);
		ClassicAssert.AreEqual(2014, nextDayOfMonth.Year);
		ClassicAssert.AreEqual(12, nextDayOfMonth.Hour);
	}

}
