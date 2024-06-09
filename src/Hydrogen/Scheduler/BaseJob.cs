// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public abstract class BaseJob : IJob {
	private IList<IJobSchedule> _schedules;

	protected BaseJob(JobPolicy policy = JobPolicy.Asyncronous) {
		Status = JobStatus.Uninitalized;
		_schedules = new List<IJobSchedule>();
		Policy = policy;
	}

	public virtual string Name { get; set; }

	public virtual JobStatus Status { get; set; }

	public virtual JobPolicy Policy { get; set; }

	public IEnumerable<IJobSchedule> Schedules {
		get => _schedules.Select(x => x);
		set => _schedules = value.ToList();
	}

	public ILogger Log { get; set; }

	public virtual void AddSchedule(BaseJobSchedule schedule) {
		schedule.Job = this;
		_schedules.Add(schedule);
	}

	public abstract void Execute();

	public abstract JobSerializableSurrogate ToSerializableSurrogate();

	public abstract void FromSerializableSurrogate(JobSerializableSurrogate jobSurrogate);
}
