// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Hydrogen;

public class Scheduler<TJob, TJobSchedule> : SyncDisposable where TJob : class, IJob where TJobSchedule : class, IJobSchedule, IComparable<TJobSchedule> {
	private readonly object _threadLock; // only used for start, stop & interrupt method
	private readonly ElapsedEventHandler _timerHandler;
	private readonly Timer _timer;
	private readonly SynchronizedHeap<TJobSchedule> _timeLine;
	private readonly SynchronizedCollection<TJobSchedule> _userPausedJobs;

	public event EventHandlerEx<TJob, JobStatus, JobStatus> JobStatusChanged;
	public event EventHandlerEx<SchedulerStatus, SchedulerStatus> StatusChanged;

	#region Constructors

	public Scheduler() : this(SchedulerPolicy.DontThrow) {
	}

	public Scheduler(SchedulerPolicy policy, ILogger logger = null) {
		_threadLock = new object();
		_timer = new Timer {
			AutoReset = false,
			SynchronizingObject = null
		};
		_timerHandler = (sender, args) => RunPendingJobs();
		_timer.Elapsed += _timerHandler;
		_timeLine = new SynchronizedHeap<TJobSchedule>();
		_userPausedJobs = new SynchronizedCollection<TJobSchedule>();
		Policy = policy;
		Log = logger ?? new NoOpLogger();
	}

	#endregion

	#region Properties

	public SchedulerStatus Status { get; private set; }

	public SchedulerPolicy Policy { get; set; }

	protected ILogger Log { get; }

	public Action<TJob> OnBeforeJobBegin { get; set; }

	public Action<TJob> OnAfterJobEnd { get; set; }

	public Action<TJob, Exception> OnJobError { get; set; }

	#endregion

	#region Methods

	public virtual IEnumerable<TJob> GetJobs() {
		Log.Debug($"Scheduler GetJobs");
		using (_timeLine.EnterReadScope()) {
			return _timeLine.Select(s => (TJob)s.Job).Concat(_userPausedJobs.Select(s => (TJob)s.Job)).Distinct().ToArray();
		}
	}

	public virtual void AddJob(TJob job) {
		Guard.ArgumentNotNull(job, "job");
		Guard.ArgumentNotNull(job.Schedules, "job.Schedules");

		// Check asychronous/sychronous jobs. Make sure:
		// - Asychronous jobs can't be added to a sychronous scheduler; and
		// - Sychronous jobs can't be added to an asychronous scheduler.
		var jobIsAsynchronus = job.Policy.HasFlag(JobPolicy.Asyncronous);
		var schedulerIsAsynchronous = !Policy.HasFlag(SchedulerPolicy.ForceSyncronous);

		if ((jobIsAsynchronus && !schedulerIsAsynchronous) || (!jobIsAsynchronus && schedulerIsAsynchronous)) {
			var jobText = jobIsAsynchronus ? "asychronous" : "sychronous";
			var schedulerText = schedulerIsAsynchronous ? "asychronous" : "sychronous";
			throw new InvalidOperationException($"Cannot run {jobText} job on {schedulerText} scheduler (check job and scheduler policies)");
		}

		Log.Debug($"Scheduler Added {job.Name}");
		try {
			if (job.Log == null) {
				job.Log = Log;
			}

			_timer.Stop();
			UpdateJobStatus(job, JobStatus.Initialized);
			// Add all the schedules to the timeline
			using (_timeLine.EnterWriteScope()) {
				job.Schedules.ForEach(s => _timeLine.Add((TJobSchedule)s));
			}
			UpdateJobStatus(job, Status != SchedulerStatus.Off ? JobStatus.Scheduled : JobStatus.Paused);
		} catch {
			if (!Policy.HasFlag(SchedulerPolicy.DontThrow))
				throw;
		} finally {
			RestartTimer();
		}
	}

	public virtual void RemoveJob(TJob job) {
		try {
			Log.Debug($"Scheduler Removed {job.Name}");
			_timer.Stop();
			using (_timeLine.EnterWriteScope()) {
				_timeLine.Where(t => t.Job == job).ToArray().ForEach(s => _timeLine.Remove(s));
			}
			using (_userPausedJobs.EnterWriteScope()) {
				_userPausedJobs.Where(t => t.Job == job).ToArray().ForEach(s => _timeLine.Remove(s));
			}
			UpdateJobStatus(job, JobStatus.Uninitalized);
		} catch {
			if (!Policy.HasFlag(SchedulerPolicy.DontThrow))
				throw;
		} finally {
			RestartTimer();
		}
	}

	public virtual void PauseJob(TJob job) {
		try {
			Log.Debug($"Scheduler Paused {job.Name}");
			_timer.Stop();
			TJobSchedule[] jobSchedules;
			using (_timeLine.EnterReadScope()) {
				jobSchedules = _timeLine.Where(s => s.Job == job).ToArray();
			}
			using (_timeLine.EnterWriteScope()) {
				jobSchedules.ForEach(s => _timeLine.Remove(s));
			}
			using (_userPausedJobs.EnterWriteScope()) {
				jobSchedules.ForEach(s => _userPausedJobs.Add(s));
			}
			UpdateJobStatus(job, JobStatus.Paused);
		} catch {
			if (!Policy.HasFlag(SchedulerPolicy.DontThrow))
				throw;
		} finally {
			RestartTimer();
		}
	}

	public virtual void ResumeJob(TJob job) {
		try {
			Log.Debug($"Scheduler Resumed {job.Name}");
			_timer.Stop();
			TJobSchedule[] jobSchedules = null;
			using (_userPausedJobs.EnterReadScope()) {
				jobSchedules = _userPausedJobs.Where(s => s.Job == job).ToArray();
			}
			using (_userPausedJobs.EnterWriteScope()) {
				jobSchedules.ForEach(s => _userPausedJobs.Remove(s));
			}
			using (_timeLine.EnterWriteScope()) {
				jobSchedules.ForEach(s => _timeLine.Add(s));
			}
			UpdateJobStatus(job, Status != SchedulerStatus.Off ? JobStatus.Scheduled : JobStatus.Paused);
		} catch {
			if (!Policy.HasFlag(SchedulerPolicy.DontThrow))
				throw;
		} finally {
			RestartTimer();
		}
	}

	public virtual void RescheduleJob(TJob job) {
		try {
			Log.Debug($"Scheduler Rescheduled {job.Name}");
			_timer.Stop();
			SynchronizedCollection<TJobSchedule> jobLocation;
			using (_userPausedJobs.EnterReadScope()) {
				jobLocation = _userPausedJobs.Any(t => t.Job == job) ? _userPausedJobs : _timeLine;
			}

			using (jobLocation.EnterWriteScope()) {
				jobLocation.Where(t => t.Job == job).ToArray().ForEach(s => jobLocation.Remove(s));
				job.Schedules.ForEach(s => jobLocation.Add((TJobSchedule)s));
			}

			if (jobLocation == _timeLine && Status != SchedulerStatus.Off) {
				UpdateJobStatus(job, JobStatus.Scheduled);
			} else {
				UpdateJobStatus(job, JobStatus.Paused);
			}
		} catch {
			if (!Policy.HasFlag(SchedulerPolicy.DontThrow))
				throw;
		} finally {
			RestartTimer();
		}
	}

	public virtual void Start() {
		Log.Debug($"Scheduler Start");
		lock (_threadLock) {
			if (Status != SchedulerStatus.Off)
				return;
			using (_timeLine.EnterReadScope())
				_timeLine.Select(t => t.Job).Distinct().Update(j => UpdateJobStatus((TJob)j, JobStatus.Scheduled));
			UpdateStatus(SchedulerStatus.Idle);
			RestartTimer();
		}
	}

	public virtual void Stop() {
		Log.Debug($"Scheduler Stop");
		lock (_threadLock) {
			if (Status != SchedulerStatus.Off) {
				_timer.Stop();
				using (_timeLine.EnterReadScope())
					_timeLine.Select(t => t.Job).Distinct().Update(j => UpdateJobStatus((TJob)j, JobStatus.Paused));
				UpdateStatus(SchedulerStatus.Off);
			}
		}
	}

	public bool InterruptWith(TJob job) {
		Log.Debug($"Scheduler InterruptWith {job.Name}");
		lock (_threadLock) {
			var result = false;
			var needsRestart = Status != SchedulerStatus.Off;
			if (needsRestart)
				Stop();

			try {
				var jobStatus = job.Status;
				UpdateJobStatus(job, JobStatus.Scheduled);
				ExecuteJob(job);
				UpdateJobStatus(job, jobStatus);
				result = true;
			} catch (Exception error) {
				Log.Error(error.ToDiagnosticString());
				needsRestart = false;
			}

			if (needsRestart)
				Start();

			return result;
		}
	}

	#endregion

	#region Auxillary methods

	public SchedulerSerializableSurrogate ToSerializableSurrogate() {
		using (_timeLine.EnterReadScope()) {
			return new SchedulerSerializableSurrogate {
				Jobs =
					_timeLine
						.Select(t => t.Job)
						.Distinct()
						.Select(j => j.ToSerializableSurrogate())
						.ToArray()
			};
		}
	}

	public void FromSerializableSurrogate(SchedulerSerializableSurrogate schedulerSurrogate) {
		foreach (var jobSurrogate in schedulerSurrogate.Jobs) {
			var job = new SchedulerJob();
			job.FromSerializableSurrogate(jobSurrogate);

			AddJob(job as TJob);
		}
	}

	protected void RunPendingJobs() {
		Log.Debug($"Scheduler RunPendingJobs");
		var currentStatus = Status;

		if (currentStatus == SchedulerStatus.Off)
			return;

		UpdateStatus(SchedulerStatus.On);

		// this loop is optimized for thread-safety with minimal locking
		while (true) {
			// Take the next job from the pile 
			TJobSchedule schedule;
			using (_timeLine.EnterWriteScope()) {
				if (_timeLine.Count == 0 || _timeLine.Peek().NextStartTime > DateTime.UtcNow)
					break;
				schedule = _timeLine.Pop();
			}
			// Execute the job (ignoring return value -- its for subclasses only
			if (!Policy.HasFlag(SchedulerPolicy.ForceSyncronous) && schedule.Job.Policy.HasFlag(JobPolicy.Asyncronous)) {
				Tools.Threads.QueueAction(RunSchedule, schedule);
			} else {
				RunSchedule(schedule);
			}
		}
		if (Status == SchedulerStatus.On)
			UpdateStatus(SchedulerStatus.Idle);

		RestartTimer();
	}

	private void RunSchedule(TJobSchedule schedule) {
		var start = DateTime.UtcNow;
		var job = schedule.Job;
		schedule.NotifyStart(start);
		if (schedule.ReschedulePolicy == ReschedulePolicy.OnStart) {
			if (schedule.IterationsRemaining > 0) {
				// add schedule
				_timeLine.Add(schedule);
				RestartTimer();
			}
		}
		ExecuteJob((TJob)job);
		var end = DateTime.UtcNow;
		schedule.NotifyExecution(start, end);
		if (schedule.ReschedulePolicy == ReschedulePolicy.OnFinish) {
			if (schedule.IterationsRemaining > 0) {
				// add schedule
				_timeLine.Add(schedule);
				RestartTimer();
			}
		}
	}
	private void ExecuteJob(TJob job) {
		Log.Debug($"Scheduler ExecuteJob ${job.Name}");
		Guard.ArgumentNotNull(job, "job");

		try {
			if (!job.Status.IsIn(JobStatus.Scheduled, JobStatus.Completed))
				return;

			UpdateJobStatus(job, JobStatus.Started);

			OnBeforeJobBegin?.Invoke(job);
			job.Execute();
			OnAfterJobEnd?.Invoke(job);
		} catch (Exception error) {
			OnJobError?.Invoke(job, error);

			Log.Error(error.ToDiagnosticString());
			if (Policy.HasFlag(SchedulerPolicy.RemoveJobOnError)) {
				RemoveJob(job);
				UpdateJobStatus(job, JobStatus.Error);
			}
			if (!Policy.HasFlag(SchedulerPolicy.DontThrow))
				throw;
		} finally {
			if (job.Status == JobStatus.Started)
				Tools.Exceptions.ExecuteIgnoringException(() => UpdateJobStatus(job, JobStatus.Completed));
		}
	}

	protected void RestartTimer() {
		Log.Debug($"Scheduler RestartTimer");
		if (Status == SchedulerStatus.Off || Status == SchedulerStatus.On)
			return;

		var shouldDispose = false;
		using (_timeLine.EnterReadScope()) {
			if (_timeLine.Count > 0) {
				_timer.Interval =
					_timeLine
						.Peek()
						.NextStartTime
						.Subtract(DateTime.UtcNow)
						.TotalMilliseconds
						.ClipTo(1, double.MaxValue);
				Log.Debug($"Scheduler RestartTimer Interval = {_timer.Interval}");
				_timer.Start();
			} else {
				_timer.Stop();
				shouldDispose = Policy.HasFlag(SchedulerPolicy.DisposeWhenFinished);
			}
		}
		if (shouldDispose)
			Dispose();
	}

	protected void UpdateJobStatus(TJob job, JobStatus status) {
		Log.Debug($"Scheduler UpdateJobStatus {job.Name} -> {status}");
		var oldStatus = job.Status;
		if (oldStatus != status) {
			job.Status = status;
			ExecuteSafely(() => JobStatusChanged?.Invoke(job, oldStatus, job.Status));
		}
	}

	protected void UpdateStatus(SchedulerStatus status) {
		Log.Debug($"Scheduler UpdateStatus {status}");
		lock (_threadLock) {
			var oldStatus = Status;
			if (oldStatus != status) {
				Status = status;
				ExecuteSafely(() => StatusChanged?.Invoke(oldStatus, Status));
			}
		}
	}

	protected override void FreeManagedResources() {
		_timer.Stop();
		_timer.Elapsed -= _timerHandler;
		_timeLine.Clear();
		_userPausedJobs.Clear();
		Tools.Exceptions.ExecuteIgnoringException(() => Log.Debug("Scheduler Disposed"));
	}

	private void ExecuteSafely(Action action) {
		try {
			action?.Invoke();
		} catch (Exception error) {
			Log.Error(error.ToDiagnosticString());
			if (!Policy.HasFlag(SchedulerPolicy.DontThrow))
				throw;
		}
	}

	#endregion

}


public class Scheduler<TJob> : Scheduler<TJob, IJobSchedule> where TJob : class, IJob {

	public Scheduler()
		: base() {
	}

	public Scheduler(SchedulerPolicy policy, ILogger logger = null)
		: base(policy, logger) {
	}
}


public class Scheduler : Scheduler<IJob, IJobSchedule> {
	private static volatile Scheduler _asynchronousScheduler;
	private static volatile Scheduler _synchronousScheduler;

	static Scheduler() {
		_asynchronousScheduler = new Scheduler(SchedulerPolicy.DontThrow | SchedulerPolicy.RemoveJobOnError);
		_asynchronousScheduler.Start();

		_synchronousScheduler = new Scheduler(SchedulerPolicy.DontThrow | SchedulerPolicy.RemoveJobOnError | SchedulerPolicy.ForceSyncronous);
		_synchronousScheduler.Start();
	}

	public Scheduler() : base() {
	}

	public Scheduler(SchedulerPolicy policy, ILogger logger = null) : base(policy, logger) {
	}

	public static Scheduler Asynchronous => _asynchronousScheduler;
	public static Scheduler Synchronous => _synchronousScheduler;
}
