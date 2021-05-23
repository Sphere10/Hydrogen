//-----------------------------------------------------------------------
// <copyright file="ScheduleBuilder.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Scheduler {

	public class ScheduleBuilder<T> where T : BaseJob {
		readonly JobBuilder<T> _jobBuilder;

		internal ScheduleBuilder(JobBuilder<T> jobBuilder) {
			_jobBuilder = jobBuilder;
			
		}
		public JobBuilder<T> OnInterval(TimeSpan interval, ReschedulePolicy reschedulePolicy = ReschedulePolicy.OnFinish, uint? totalIterations = null, DateTime? endDate = null) {
			return OnInterval(DateTime.UtcNow.Add(interval), interval, reschedulePolicy, totalIterations, endDate);
		}

		public JobBuilder<T> OnInterval(DateTime startOn, TimeSpan interval, ReschedulePolicy reschedulePolicy = ReschedulePolicy.OnFinish,  uint? totalIterations = null, DateTime? endDate = null) {
			_jobBuilder.Job.AddSchedule(new OnIntervalSchedule<T>(startOn, interval, reschedulePolicy, totalIterations, endDate)); 
			return _jobBuilder;
		}

	    public JobBuilder<T> OnDayOfWeek(DayOfWeek dayOfWeek, TimeSpan timeOfDay, uint? totalIterations = null, DateTime? endDate = null) {            
            _jobBuilder.Job.AddSchedule(new OnDayOfWeekScheduler<T>(dayOfWeek, timeOfDay, totalIterations, endDate)); 
            return _jobBuilder;
	    }

        public JobBuilder<T> OnDayOfMonth(int dayOfMonth, TimeSpan timeOfDay, uint? totalIterations = null, DateTime? endDate = null) {            
            _jobBuilder.Job.AddSchedule(new OnDayOfMonthScheduler<T>(dayOfMonth, timeOfDay, totalIterations, endDate));
            return _jobBuilder;
        }

	    public T Build() {
			return _jobBuilder.Build();
		}
	}

	public class ScheduleBuilder : ScheduleBuilder<BaseJob> {
		
		internal ScheduleBuilder(JobBuilder<BaseJob> jobBuilder) : base(jobBuilder) {

		}
	}
}
