//-----------------------------------------------------------------------
// <copyright file="OnDayOfWeekScheduler.cs" company="Sphere 10 Software">
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
using Sphere10.Framework.Scheduler.Serializable;

namespace Sphere10.Framework.Scheduler {

	public class OnDayOfWeekScheduler <T> : BaseJobSchedule<T> where T : class, IJob {
	    private readonly DayOfWeek _dayOfWeek;
	    private readonly TimeSpan _timeOfDay;
	
		public OnDayOfWeekScheduler(DayOfWeek dayOfWeek, TimeSpan timeOfDay, bool repeat = false)
            : this(dayOfWeek, timeOfDay, repeat ? (uint?)null : 1) {
		}

        public OnDayOfWeekScheduler(DayOfWeek dayOfWeek, TimeSpan timeOfDay, uint? totalTimesToRun)
            : base(Tools.Time.CalculateNextDayOfWeek(DateTime.UtcNow, dayOfWeek, timeOfDay), totalTimesToRun) {
            _dayOfWeek = dayOfWeek;
            _timeOfDay = timeOfDay;
        }

		public override ReschedulePolicy ReschedulePolicy => ReschedulePolicy.OnStart;


		public override JobScheduleSerializableSurrogate ToSerializableSurrogate() {
			return new DayOfWeekScheduleSerializableSurrogate {
				DayOfWeek = this._dayOfWeek,
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
            return Tools.Time.CalculateNextDayOfWeek(DateTime.UtcNow, _dayOfWeek, _timeOfDay);
		}
        
	}
}
