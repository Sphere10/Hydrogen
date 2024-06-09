// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class BaseJobSchedule : IJobSchedule {
	protected uint? TotalIterations { get; set; }
	protected DateTime? InitialStartTime { get; set; }

	protected BaseJobSchedule() {
	}

	protected BaseJobSchedule(DateTime? startOn, uint? numIterations = null, DateTime? endDate = null) {
		LastStartTime = null;
		LastEndTime = null;
		EndDate = endDate;
		InitialStartTime = startOn?.ToUniversalTime();
		TotalIterations = numIterations;
		IterationsExecuted = 0;
		IterationsRemaining = TotalIterations ?? uint.MaxValue;
	}

	public IJob Job { get; internal set; }
	public DateTime? LastStartTime { get; protected set; }
	public DateTime? LastEndTime { get; protected set; }
	public DateTime? EndDate { get; protected set; }

	public DateTime NextStartTime {
		get {
			if (EndDate.HasValue && DateTime.Now >= EndDate.Value)
				return DateTime.MaxValue;

			if (IterationsRemaining == 0)
				return DateTime.MaxValue;

			if (IterationsExecuted == 0 && InitialStartTime != null)
				return InitialStartTime.Value;

			return CalculateNextRunTime();
		}
	}

	public abstract ReschedulePolicy ReschedulePolicy { get; protected set; }

	public uint IterationsRemaining { get; protected set; }

	public uint IterationsExecuted { get; protected set; }

	public void NotifyStart(DateTime start) {
		if (IterationsRemaining == 0)
			throw new SoftwareException("Job Schedule has no more scheduled iterations, yet is being called");

		LastStartTime = start.ToUniversalTime();
		if (TotalIterations != null && TotalIterations > 0) {

			IterationsRemaining--;
		}
	}

	public virtual void NotifyExecution(DateTime start, DateTime end) {
		LastStartTime = start.ToUniversalTime();
		LastEndTime = end.ToUniversalTime();
		unchecked {
			IterationsExecuted++;
		}
	}

	public abstract JobScheduleSerializableSurrogate ToSerializableSurrogate();

	public abstract void FromSerializableSurrogate(JobScheduleSerializableSurrogate scheduleSurrogate);

	protected abstract DateTime CalculateNextRunTime();

	public int CompareTo(IJobSchedule other) {
		return NextStartTime.CompareTo(other.NextStartTime);
	}
}


public abstract class BaseJobSchedule<T> : BaseJobSchedule where T : class, IJob {
	protected BaseJobSchedule() {
	}

	protected BaseJobSchedule(DateTime? startOn, uint? numIterations = null, DateTime? endDate = null) : base(startOn, numIterations, endDate) {
	}

	public new T Job {
		get { return (T)base.Job; }
		set { base.Job = value; }
	}
}
