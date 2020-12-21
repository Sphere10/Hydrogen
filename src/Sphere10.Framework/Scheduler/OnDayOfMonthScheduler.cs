//-----------------------------------------------------------------------
// <copyright file="OnDayOfMonthScheduler.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Scheduler {

	public class OnDayOfMonthScheduler <T> : BaseJobSchedule<T> where T : class, IJob {
        private readonly int _dayOfMonth;
	    private readonly TimeSpan _timeOfDay;
	
		public OnDayOfMonthScheduler(int dayOfMonth, TimeSpan timeOfDay, bool repeat = false)
            : this(dayOfMonth, timeOfDay, repeat ? (uint?)null : 1) {
		}

        public OnDayOfMonthScheduler(int dayOfMonth, TimeSpan timeOfDay, uint? totalTimesToRun)
            : base(Tools.Time.CalculateNextDayOfMonth(DateTime.UtcNow, dayOfMonth, timeOfDay), totalTimesToRun) {
            _dayOfMonth = dayOfMonth;
            _timeOfDay = timeOfDay;
        }

		public override ReschedulePolicy ReschedulePolicy => ReschedulePolicy.OnStart;

		protected override DateTime CalculateNextRunTime() {
            return Tools.Time.CalculateNextDayOfMonth(DateTime.UtcNow, _dayOfMonth, _timeOfDay);
		}
	}
}
