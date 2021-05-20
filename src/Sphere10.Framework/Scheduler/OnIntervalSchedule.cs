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
		private readonly TimeSpan _repeatInterval;
	
		public OnIntervalSchedule(DateTime start, TimeSpan repeatEvery, ReschedulePolicy reschedulePolicy = ReschedulePolicy.OnFinish, uint? totalTimesToRun = null)
			: base(start, totalTimesToRun) {
			_repeatInterval = repeatEvery;
			ReschedulePolicy = reschedulePolicy;
		}

		public override ReschedulePolicy ReschedulePolicy { get; }

		public override JobScheduleSerializableSurrogate ToSerializableSurrogate() {
			return new IntervalScheduleSerializableSurrogate {
				IterationsExecuted = this.IterationsExecuted,
				IterationsRemaining = this.IterationsRemaining,
				LastEndTime = this.LastEndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				LastStartTime = this.LastStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
				NextStartTime = this.NextStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
				ReschedulePolicy = this.ReschedulePolicy,
				RepeatIntervalMS = (long)Math.Round(_repeatInterval.TotalMilliseconds, 0)
			};
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

			return originTime.Add(_repeatInterval).ToUniversalTime().ClipTo(DateTime.UtcNow, DateTime.UtcNow + _repeatInterval);
		}
	}

}
