// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen;

namespace Hydrogen {

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
}
