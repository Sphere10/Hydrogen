// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IJobSchedule : IComparable<IJobSchedule> {
	IJob Job { get; }
	DateTime? LastStartTime { get; }
	DateTime? LastEndTime { get; }
	DateTime NextStartTime { get; }
	DateTime? EndDate { get; }
	ReschedulePolicy ReschedulePolicy { get; }
	uint IterationsRemaining { get; }
	uint IterationsExecuted { get; }

	void NotifyStart(DateTime start);

	void NotifyExecution(DateTime start, DateTime end);

	JobScheduleSerializableSurrogate ToSerializableSurrogate();

	void FromSerializableSurrogate(JobScheduleSerializableSurrogate scheduleSurrogate);
}
