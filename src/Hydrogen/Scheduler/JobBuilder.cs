// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class JobBuilder<T> where T : BaseJob {
	internal readonly T Job;

	internal JobBuilder(T job) {
		Job = job;
	}

	public static JobBuilder<T> For(T job) {
		return new JobBuilder<T>(job);
	}

	public JobBuilder<T> Called(string name) {
		Job.Name = name;
		return this;
	}

	public JobBuilder<T> RunOnce(DateTime start) {
		Job.AddSchedule(new OnIntervalSchedule<T>(start, TimeSpan.Zero, ReschedulePolicy.OnStart, 1));
		return this;
	}

	public JobBuilder<T> RunSyncronously() {
		Job.Policy = Job.Policy.CopyAndClearFlags(JobPolicy.Asyncronous);
		return this;
	}

	public JobBuilder<T> RunAsyncronously() {
		Job.Policy = Job.Policy.CopyAndSetFlags(JobPolicy.Asyncronous);
		return this;
	}

	public ScheduleBuilder<T> Repeat => new ScheduleBuilder<T>(this);

	public T Build() {
		return Job;
	}

}


public class JobBuilder : JobBuilder<BaseJob> {

	internal JobBuilder(BaseJob job) : base(job) {
	}

	public static JobBuilder<BaseJob> For(Action action) {
		return new JobBuilder(new ActionJob(action));
	}

	public static JobBuilder<BaseJob> For(Type jobType) {
		return new JobBuilder(new SchedulerJob(jobType));
	}
}
