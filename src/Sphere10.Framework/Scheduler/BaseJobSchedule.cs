//-----------------------------------------------------------------------
// <copyright file="BaseJobSchedule.cs" company="Sphere 10 Software">
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

	public abstract class BaseJobSchedule : IJobSchedule {
		private readonly uint? _totalIterations;
		private readonly DateTime? _initialStartTime;
		protected BaseJobSchedule(DateTime? startOn, uint? numIterations = null) {
			LastStartTime = null;
			LastEndTime = null;
			_initialStartTime = startOn?.ToUniversalTime();
			_totalIterations = numIterations;
			IterationsExecuted = 0;
			IterationsRemaining = _totalIterations ?? uint.MaxValue;
		}

		public IJob Job { get; internal set; }	
		public DateTime? LastStartTime { get; protected set; }
		public DateTime? LastEndTime { get; protected set; }

		public DateTime NextStartTime {
			get {
				if (IterationsRemaining == 0)
					return DateTime.MaxValue;

				if (IterationsExecuted == 0 && _initialStartTime != null)
					return _initialStartTime.Value;

				return CalculateNextRunTime();
			}
		}

		public abstract ReschedulePolicy ReschedulePolicy { get; }

		public uint IterationsRemaining { get; protected set; }

		public uint IterationsExecuted { get; protected set; }

		public void NotifyStart(DateTime start) {
			if (IterationsRemaining == 0)
				throw new SoftwareException("Job Schedule has no more scheduled iterations, yet is being called");

			LastStartTime = start.ToUniversalTime();
			if (_totalIterations != null && _totalIterations > 0) {

				IterationsRemaining--;
			}
		}

		public virtual void NotifyExecution(DateTime start, DateTime end) {
			LastStartTime = start.ToUniversalTime();
			LastEndTime = end.ToUniversalTime();
			unchecked { IterationsExecuted++; }
		}

		protected abstract DateTime CalculateNextRunTime();

		public int CompareTo(IJobSchedule other) {
			return NextStartTime.CompareTo(other.NextStartTime);
		}
	}

	public abstract class BaseJobSchedule<T> : BaseJobSchedule where T : class, IJob {
		protected BaseJobSchedule(DateTime? startOn, uint? numIterations = null) : base(startOn, numIterations) {			
		}

		public new T Job {  get { return (T)base.Job; } set { base.Job = value; } }		
	}

}
