// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class OnDayOfWeekScheduler<T> : BaseJobSchedule<T> where T : class, IJob {
	private DayOfWeek DayOfWeek { get; set; }
	private TimeSpan TimeOfDay { get; set; }

	public OnDayOfWeekScheduler() {
	}

	public OnDayOfWeekScheduler(DayOfWeek dayOfWeek, TimeSpan timeOfDay, bool repeat = false, DateTime? endDate = null)
		: this(dayOfWeek, timeOfDay, repeat ? (uint?)null : 1, endDate) {
	}

	public OnDayOfWeekScheduler(DayOfWeek dayOfWeek, TimeSpan timeOfDay, uint? totalTimesToRun, DateTime? endDate = null)
		: base(Tools.Time.CalculateNextDayOfWeek(DateTime.UtcNow, dayOfWeek, timeOfDay), totalTimesToRun, endDate) {
		DayOfWeek = dayOfWeek;
		TimeOfDay = timeOfDay;
	}

	public override ReschedulePolicy ReschedulePolicy { get; protected set; } = ReschedulePolicy.OnStart;


	public override JobScheduleSerializableSurrogate ToSerializableSurrogate() {
		return new DayOfWeekScheduleSerializableSurrogate {
			TotalIterations = TotalIterations?.ToString(),
			InitialStartTime = InitialStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),

			DayOfWeek = this.DayOfWeek,
			IterationsExecuted = this.IterationsExecuted,
			IterationsRemaining = this.IterationsRemaining,
			LastEndTime = this.LastEndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
			LastStartTime = this.LastStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
			EndDate = EndDate?.ToString("yyyy-MM-dd HH:mm:ss"),
			ReschedulePolicy = this.ReschedulePolicy,
			TimeOfDay = (long)Math.Round(TimeOfDay.TotalMilliseconds, 0)
		};
	}

	public override void FromSerializableSurrogate(JobScheduleSerializableSurrogate scheduleSurrogate) {
		var surrogate = (DayOfWeekScheduleSerializableSurrogate)scheduleSurrogate;

		TotalIterations = surrogate.TotalIterations.ToUIntOrDefault();
		InitialStartTime = surrogate.InitialStartTime.ToDateTimeOrDefault();

		IterationsExecuted = surrogate.IterationsExecuted;
		IterationsRemaining = surrogate.IterationsRemaining;
		LastEndTime = surrogate.LastEndTime.ToDateTimeOrDefault();
		LastStartTime = surrogate.LastStartTime.ToDateTimeOrDefault();
		EndDate = surrogate.EndDate.ToDateTimeOrDefault();
		ReschedulePolicy = surrogate.ReschedulePolicy;

		DayOfWeek = surrogate.DayOfWeek;
		TimeOfDay = TimeSpan.FromMilliseconds(surrogate.TimeOfDay);
	}

	protected override DateTime CalculateNextRunTime() {
		return Tools.Time.CalculateNextDayOfWeek(DateTime.UtcNow, DayOfWeek, TimeOfDay);
	}

}
