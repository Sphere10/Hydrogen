// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.BITS;

/// <summary>
/// Encapsulates a job by name. Will create if doesn't exist, or fetch if already exists.
/// Good for monitoring jobs over application load boundary.
/// </summary>
public abstract class JobEx {
	private BitsJob _underlyingJob;

	public JobEx() {

	}

	public void Initialize() {
		BitsManager bitsManager = new BitsManager();
		bitsManager.OnInterfaceError += new EventHandler<BitsInterfaceNotificationEventArgs>(bitsManager_OnInterfaceError);
		BitsJobs jobs = bitsManager.EnumJobs(Owner);
		bool foundJob = false;
		foreach (BitsJob job in jobs.Values) {
			if (job.DisplayName == Name) {
				// if not transferring, then it is dangling so remove it
#warning Fix behaviour here
				if (true) {
					//job.State != JobState.Transferring) {
					job.Cancel();
				} else {
					UnderlyingJob = job;
					foundJob = true;
				}
			}
		}
		if (!foundJob) {
			UnderlyingJob = bitsManager.CreateJob(Name, JobType);
			OnCreate(UnderlyingJob);
		}
		UnderlyingJob.OnJobErrorEvent += new EventHandler<JobErrorNotificationEventArgs>(UnderlyingJob_OnJobErrorEvent);
		UnderlyingJob.OnJobTransferredEvent += new EventHandler<JobNotificationEventArgs>(UnderlyingJob_OnJobTransferredEvent);
	}

	void bitsManager_OnInterfaceError(object sender, BitsInterfaceNotificationEventArgs e) {
		if (e.Job == UnderlyingJob) {
			OnError(
				string.Format(
					"{0}.{1}.",
					e.Message,
					e.Description
				)
			);
		}
	}

	public void Start() {
		Initialize();

		UnderlyingJob.Resume();
	}

	abstract public JobType JobType { get; }

	abstract public string Name { get; }

	abstract public JobOwner Owner { get; }

	abstract public bool DeleteOnFailure { get; }

	public BitsJob UnderlyingJob {
		get { return _underlyingJob; }
		set { _underlyingJob = value; }
	}

	public abstract void OnCreate(BitsJob jobToConfigure);

	public virtual void OnError(string error) {
		if (DeleteOnFailure) {
			UnderlyingJob.Cancel();
		}
	}

	public abstract void OnCompleted();

	#region Handlers

	void UnderlyingJob_OnJobTransferredEvent(object sender, JobNotificationEventArgs e) {
		OnCompleted();
	}

	void UnderlyingJob_OnJobErrorEvent(object sender, JobErrorNotificationEventArgs e) {
		OnError(
			string.Format(
				"Failed to transfer '{0}'. {1}",
				e.Error.File.RemoteName,
				e.Error.Description
			)
		);
	}

	#endregion

}
