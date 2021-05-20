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
using System.Xml.Serialization;
using Sphere10.Framework.Scheduler.Serializable;

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

		public override JobScheduleSerializableSurrogate ToSerializableSurrogate() {
			return new DayOfMonthScheduleSerializableSurrogate {
				DayOfMonth = this._dayOfMonth,
				IterationsExecuted = this.IterationsExecuted,
				IterationsRemaining = this.IterationsRemaining,
				LastEndTime = this.LastEndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				LastStartTime = this.LastStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				NextStartTime = this.NextStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
				ReschedulePolicy = this.ReschedulePolicy,
				TimeOfDay = DateTime.Now.ToMidnight().Add(this._timeOfDay).ToString("HH:mm:ss")
			};
		}

		protected override DateTime CalculateNextRunTime() {
            return Tools.Time.CalculateNextDayOfMonth(DateTime.UtcNow, _dayOfMonth, _timeOfDay);
		}
	}



}
