//-----------------------------------------------------------------------
// <copyright file="BaseJob.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Scheduler {
	public abstract class BaseJob : IJob {
		private readonly IList<IJobSchedule> _schedules;

		protected BaseJob(JobPolicy policy = JobPolicy.Asyncronous) {
			Status = JobStatus.Uninitalized;
			_schedules = new List<IJobSchedule>();
			Policy = policy;
		}

		public virtual string Name { get; set; }

		public virtual JobStatus Status { get; set; }

		public virtual JobPolicy Policy { get; set; }

		public IEnumerable<IJobSchedule> Schedules => _schedules.Select(x => x);

		public virtual void AddSchedule(BaseJobSchedule schedule)  {
			schedule.Job = this;
			_schedules.Add(schedule);
		}

		public abstract void Execute();
		
	}
}
