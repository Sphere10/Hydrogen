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
using Sphere10.Framework.Scheduler.Serializable;

namespace Sphere10.Framework.Scheduler {

	public class OnDayOfMonthScheduler <T> : BaseJobSchedule<T> where T : class, IJob {
        private int DayOfMonth { get; set; }
	    private TimeSpan TimeOfDay { get; set; }

		public OnDayOfMonthScheduler()
        {
        }

		public OnDayOfMonthScheduler(int dayOfMonth, TimeSpan timeOfDay, bool repeat = false, DateTime? endDate = null)
            : this(dayOfMonth, timeOfDay, repeat ? (uint?)null : 1, endDate) {
		}

        public OnDayOfMonthScheduler(int dayOfMonth, TimeSpan timeOfDay, uint? totalTimesToRun, DateTime? endDate)
            : base(Tools.Time.CalculateNextDayOfMonth(DateTime.UtcNow, dayOfMonth, timeOfDay), totalTimesToRun, endDate) {
            DayOfMonth = dayOfMonth;
            TimeOfDay = timeOfDay;
        }

		public override ReschedulePolicy ReschedulePolicy { get; protected set; } = ReschedulePolicy.OnStart;

		public override JobScheduleSerializableSurrogate ToSerializableSurrogate() {
			return new DayOfMonthScheduleSerializableSurrogate {
				TotalIterations = TotalIterations?.ToString(),
				InitialStartTime = InitialStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),

				DayOfMonth = this.DayOfMonth,
				IterationsExecuted = this.IterationsExecuted,
				IterationsRemaining = this.IterationsRemaining,
				LastEndTime = this.LastEndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				LastStartTime = this.LastStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				EndDate = EndDate?.ToString("yyyy-MM-dd HH:mm:ss"),
				ReschedulePolicy = this.ReschedulePolicy,
				TimeOfDay = (long)Math.Round(TimeOfDay.TotalMilliseconds, 0)
			};
		}

        public override void FromSerializableSurrogate(JobScheduleSerializableSurrogate scheduleSurrogate)
        {
			var surrogate = (DayOfMonthScheduleSerializableSurrogate)scheduleSurrogate;

			TotalIterations = surrogate.TotalIterations.ToUintOrDefault();
			InitialStartTime = surrogate.InitialStartTime.ToDateTimeOrDefault();

			IterationsExecuted = surrogate.IterationsExecuted;
			IterationsRemaining = surrogate.IterationsRemaining;
			LastEndTime = surrogate.LastEndTime.ToDateTimeOrDefault();
			LastStartTime = surrogate.LastStartTime.ToDateTimeOrDefault();
			EndDate = surrogate.EndDate.ToDateTimeOrDefault();
			ReschedulePolicy = surrogate.ReschedulePolicy;

			DayOfMonth = surrogate.DayOfMonth;
			TimeOfDay = TimeSpan.FromMilliseconds(surrogate.TimeOfDay);
		}

		protected override DateTime CalculateNextRunTime() {
            return Tools.Time.CalculateNextDayOfMonth(DateTime.UtcNow, DayOfMonth, TimeOfDay);
		}
	}



}
