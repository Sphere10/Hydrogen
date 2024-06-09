// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ScheduleBuilder<T> where T : BaseJob {
	readonly JobBuilder<T> _jobBuilder;

	internal ScheduleBuilder(JobBuilder<T> jobBuilder) {
		_jobBuilder = jobBuilder;

	}
	public JobBuilder<T> OnInterval(TimeSpan interval, ReschedulePolicy reschedulePolicy = ReschedulePolicy.OnFinish, uint? totalIterations = null, DateTime? endDate = null) {
		return OnInterval(DateTime.UtcNow.Add(interval), interval, reschedulePolicy, totalIterations, endDate);
	}

	public JobBuilder<T> OnInterval(DateTime startOn, TimeSpan interval, ReschedulePolicy reschedulePolicy = ReschedulePolicy.OnFinish, uint? totalIterations = null, DateTime? endDate = null) {
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
