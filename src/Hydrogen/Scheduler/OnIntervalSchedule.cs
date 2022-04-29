//-----------------------------------------------------------------------
// <copyright file="OnIntervalSchedule.cs" company="Sphere 10 Software">
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

	public class OnIntervalSchedule <T> : BaseJobSchedule<T> where T : class, IJob {
		private TimeSpan RepeatInterval { get; set; }

		public OnIntervalSchedule()
        {
        }

		public OnIntervalSchedule(DateTime start, TimeSpan repeatEvery, ReschedulePolicy reschedulePolicy = ReschedulePolicy.OnFinish, uint? totalTimesToRun = null,
			DateTime? endDate = null)
			: base(start, totalTimesToRun, endDate) {
			RepeatInterval = repeatEvery;
			ReschedulePolicy = reschedulePolicy;
		}

		public override ReschedulePolicy ReschedulePolicy { get; protected set; }

		public override JobScheduleSerializableSurrogate ToSerializableSurrogate() {
			return new IntervalScheduleSerializableSurrogate {
				TotalIterations = TotalIterations?.ToString(),
				InitialStartTime = InitialStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),

				IterationsExecuted = this.IterationsExecuted,
				IterationsRemaining = this.IterationsRemaining,
				LastEndTime = this.LastEndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				LastStartTime = this.LastStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				EndDate = EndDate?.ToString("yyyy-MM-dd HH:mm:ss"),
				ReschedulePolicy = this.ReschedulePolicy,
				RepeatIntervalMS = (long)Math.Round(RepeatInterval.TotalMilliseconds, 0),
			};
		}

        public override void FromSerializableSurrogate(JobScheduleSerializableSurrogate scheduleSurrogate)
        {
			var surrogate = (IntervalScheduleSerializableSurrogate)scheduleSurrogate;

			TotalIterations = surrogate.TotalIterations.ToUintOrDefault();
			InitialStartTime = surrogate.InitialStartTime.ToDateTimeOrDefault();

			IterationsExecuted = surrogate.IterationsExecuted;
			IterationsRemaining = surrogate.IterationsRemaining;
			LastEndTime = surrogate.LastEndTime.ToDateTimeOrDefault();
			LastStartTime = surrogate.LastStartTime.ToDateTimeOrDefault();
			EndDate = surrogate.EndDate.ToDateTimeOrDefault();
			ReschedulePolicy = surrogate.ReschedulePolicy;

			RepeatInterval = TimeSpan.FromMilliseconds(surrogate.RepeatIntervalMS);
		}

        protected override DateTime CalculateNextRunTime() {
			DateTime originTime;
			switch (ReschedulePolicy) {
				case ReschedulePolicy.OnStart:			
					if (LastStartTime == null)
						throw new SoftwareException("Cannot calculate interval since LastStartTime was null");
					originTime = LastStartTime.Value;
					break;
				case ReschedulePolicy.OnFinish:
					if (LastEndTime == null)
						throw new SoftwareException("Cannot calculate interval since LastEndTime was null");
					originTime = LastEndTime.Value;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(ReschedulePolicy));
			}

			return originTime.Add(RepeatInterval).ToUniversalTime().ClipTo(DateTime.UtcNow, DateTime.UtcNow + RepeatInterval);
		}
	}

}
