// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;

namespace Hydrogen;

public class SchedulerJob : BaseJob {
	private Type JobType { get; set; }

	public SchedulerJob() {
	}

	public SchedulerJob(Type jobType) {
		if (typeof(ISchedulerJob).IsAssignableFrom(jobType) == false) {
			throw new ArgumentException("Job must inherit ISchedulerJob");
		}

		JobType = jobType;
	}

	public override void Execute() {
		var instance = (ISchedulerJob)Activator.CreateInstance(JobType);
		instance.Execute(this);
	}

	public override JobSerializableSurrogate ToSerializableSurrogate() {
		var surrogate = new JobSerializableSurrogate {
			JobType = JobType.ToString(),
			Name = Name,
			Policy = Policy,
			Status = Status,
			Schedules = Schedules.Select(x => x.ToSerializableSurrogate()).ToArray()
		};
		return surrogate;
	}

	public override void FromSerializableSurrogate(JobSerializableSurrogate jobSurrogate) {
		JobType = TypeResolver.ResolveTypeInAllAssemblies(jobSurrogate.JobType);
		Name = jobSurrogate.Name;
		Status = jobSurrogate.Status;
		Policy = jobSurrogate.Policy;

		foreach (var surrogateSchedule in jobSurrogate.Schedules) {
			if (surrogateSchedule is IntervalScheduleSerializableSurrogate) {
				var schedule = new OnIntervalSchedule<SchedulerJob>();
				schedule.FromSerializableSurrogate(surrogateSchedule);
				AddSchedule(schedule);
			} else if (surrogateSchedule is DayOfWeekScheduleSerializableSurrogate) {
				var schedule = new OnDayOfWeekScheduler<SchedulerJob>();
				schedule.FromSerializableSurrogate(surrogateSchedule);
				AddSchedule(schedule);
			} else if (surrogateSchedule is DayOfMonthScheduleSerializableSurrogate) {
				var schedule = new OnDayOfMonthScheduler<SchedulerJob>();
				schedule.FromSerializableSurrogate(surrogateSchedule);
				AddSchedule(schedule);
			} else {
				throw new ArgumentOutOfRangeException($"Unknown schedule type {surrogateSchedule.GetType().Name}");
			}
		}
	}
}
