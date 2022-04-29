//-----------------------------------------------------------------------
// <copyright file="IJobSchedule.cs" company="Sphere 10 Software">
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
