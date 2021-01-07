//-----------------------------------------------------------------------
// <copyright file="FormatExTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class DateTimeTests {

        [Test]
        public void NextDayOfWeek_Tomorrow() {
            var startDayTime = new DateTime(2014, 09, 29); // Monday
            var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Tuesday);
            Assert.AreEqual(DayOfWeek.Tuesday, nextDayOfWeek.DayOfWeek);
            Assert.AreEqual(30, nextDayOfWeek.Day);
            Assert.AreEqual(9, nextDayOfWeek.Month);
            Assert.AreEqual(2014, nextDayOfWeek.Year);
        }

        [Test]
        public void NextDayOfWeek_NextMonday() {
            var startDayTime = new DateTime(2014, 09, 29); // Monday
            var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(DayOfWeek.Monday, nextDayOfWeek.DayOfWeek);
            Assert.AreEqual(6, nextDayOfWeek.Day);
            Assert.AreEqual(10, nextDayOfWeek.Month);
            Assert.AreEqual(2014, nextDayOfWeek.Year);
        }

        [Test]
        public void NextDayOfWeek_NextSunday() {
            var startDayTime = new DateTime(2014, 09, 29); // Monday
            var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Sunday);
            Assert.AreEqual(DayOfWeek.Sunday, nextDayOfWeek.DayOfWeek);
            Assert.AreEqual(5, nextDayOfWeek.Day);
            Assert.AreEqual(10, nextDayOfWeek.Month);
            Assert.AreEqual(2014, nextDayOfWeek.Year);
        }


        [Test]
        public void NextDayOfWeek_1HourAhead() {
            var startDayTime = new DateTime(2014, 09, 29, 11, 0, 0, 0); // Monday
            var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Monday, TimeSpan.FromHours(12));
            Assert.AreEqual(nextDayOfWeek.DayOfWeek, startDayTime.DayOfWeek);
            Assert.AreEqual(nextDayOfWeek.Day, startDayTime.Day);
            Assert.AreEqual(nextDayOfWeek.Month, startDayTime.Month);
            Assert.AreEqual(nextDayOfWeek.Year, startDayTime.Year);
            Assert.AreEqual(12, nextDayOfWeek.Hour);
        }

        [Test]
        public void NextDayOfWeek_168HoursAhead() {
            var startDayTime = new DateTime(2014, 09, 29, 12, 0, 0, 0); // Monday
            var nextDayOfWeek = startDayTime.ToNextDayOfWeek(DayOfWeek.Monday, TimeSpan.FromHours(12));
            Assert.AreEqual(nextDayOfWeek.DayOfWeek, startDayTime.DayOfWeek);
            Assert.AreEqual(6, nextDayOfWeek.Day);
            Assert.AreEqual(10, nextDayOfWeek.Month);
            Assert.AreEqual(2014, nextDayOfWeek.Year);
            Assert.AreEqual(12, nextDayOfWeek.Hour);
        }


        [Test]
        public void NextDayOfMonth_NextMonth() {
            var start = new DateTime(2014, 09, 29); // Monday
            var nextDayOfMonth = start.ToNextDayOfMonth(29);
            Assert.AreEqual(DayOfWeek.Wednesday, nextDayOfMonth.DayOfWeek);
            Assert.AreEqual(29, nextDayOfMonth.Day);
            Assert.AreEqual(10, nextDayOfMonth.Month);
            Assert.AreEqual(2014, nextDayOfMonth.Year);
        }


        [Test]
        public void NextDayOfMonth_Tomorrow() {
            var start = new DateTime(2014, 09, 29); // Monday
            var nextDayOfMonth = start.ToNextDayOfMonth(30);
            Assert.AreEqual(DayOfWeek.Tuesday, nextDayOfMonth.DayOfWeek);
            Assert.AreEqual(30, nextDayOfMonth.Day);
            Assert.AreEqual(9, nextDayOfMonth.Month);
            Assert.AreEqual(2014, nextDayOfMonth.Year);
        }

        [Test]
        public void NextDayOfMonth_1HourAhead() {
            var start = new DateTime(2014, 09, 29, 11, 0, 0, 0); // Monday
            var nextDayOfMonth = start.ToNextDayOfMonth(29, TimeSpan.FromHours(12));
            Assert.AreEqual(nextDayOfMonth.DayOfWeek, start.DayOfWeek);
            Assert.AreEqual(nextDayOfMonth.Day, start.Day);
            Assert.AreEqual(nextDayOfMonth.Month, start.Month);
            Assert.AreEqual(nextDayOfMonth.Year, start.Year);
            Assert.AreEqual(12, nextDayOfMonth.Hour);
        }

        [Test]
        public void NextDayOfMonth_MonthsHoursAhead() {
            var start = new DateTime(2014, 09, 29, 12, 0, 0, 0); // Monday
            var nextDayOfMonth = start.ToNextDayOfMonth(29, TimeSpan.FromHours(12));
            Assert.AreEqual(29, nextDayOfMonth.Day);
            Assert.AreEqual(10, nextDayOfMonth.Month);
            Assert.AreEqual(2014, nextDayOfMonth.Year);
            Assert.AreEqual(12, nextDayOfMonth.Hour);
        }

    }
}
