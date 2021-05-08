//-----------------------------------------------------------------------
// <copyright file="JobBuilder.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Scheduler {

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

	}
}
