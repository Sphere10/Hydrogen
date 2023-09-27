// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Hydrogen.Windows.BITS;

public class BitsJob : IDisposable {

	#region member fields

	private IBackgroundCopyJob job;
	IBackgroundCopyJob2 job2;
	BitsManager manager;
	private bool disposed;

	private BitsFiles files;
	private JobTimes jobTimes;
	private ProxySettings proxySettings;
	private BitsError error;
	private JobProgress progress;

	private Guid guid;

	//notification
	private EventHandler<JobNotificationEventArgs> onJobModified;
	private EventHandler<JobNotificationEventArgs> onJobTransfered;
	private EventHandler<JobErrorNotificationEventArgs> onJobErrored;

	#endregion

	#region .ctor

	internal BitsJob(BitsManager manager, IBackgroundCopyJob job) {
		this.manager = manager;
		this.job = job;
		this.job2 = this.job as IBackgroundCopyJob2;
		this.NotificationInterface = manager.NotificationHandler;
	}

	#endregion

	#region public properties

	/// <summary>
	/// Display Name, max 256 chars
	/// </summary>
	public string DisplayName {
		get {
			try {
				string displayName;
				this.job.GetDisplayName(out displayName);
				return displayName;
			} catch (COMException exception) {
				manager.PublishException(this, exception);
				return string.Empty;
			}
		}
		set {
			try {
				this.job.SetDisplayName(value);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
		}
	}

	/// <summary>
	/// Description, max 1024 chars
	/// </summary>
	public string Description {
		get {
			try {
				string description;
				this.job.GetDescription(out description);
				return description;
			} catch (COMException exception) {
				manager.PublishException(this, exception);
				return string.Empty;
			}
		}
		set {
			try {
				this.job.SetDescription(value);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
		}
	}

	/// <summary>
	/// SID of the job owner
	/// </summary>
	public string Owner {
		get {
			try {
				string owner;
				this.job.GetOwner(out owner);
				return owner;
			} catch (COMException exception) {
				manager.PublishException(this, exception);
				return string.Empty;
			}
		}
	}

	/// <summary>
	/// resolved owner name from job owner SID
	/// </summary>
	public string OwnerName {
		get {
			try {
				return Utils.GetName(Owner);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
				return string.Empty;
			}
		}
	}

	/// <summary>
	/// Job priority
	/// can not be set for jobs already in state Canceled or Acknowledged
	/// </summary>
	public JobPriority Priority {
		get {
			BG_JOB_PRIORITY priority = BG_JOB_PRIORITY.BG_JOB_PRIORITY_NORMAL;
			try {
				this.job.GetPriority(out priority);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return (JobPriority)priority;
		}
		set {
			try {
				this.job.SetPriority((BG_JOB_PRIORITY)value);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
		}

	}

	public JobProgress Progress {
		get {
			try {
				BG_JOB_PROGRESS progress;
				this.job.GetProgress(out progress);
				this.progress = new JobProgress(progress);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return this.progress;
		}
	}

	public BitsFiles EnumFiles() {
		try {
			IEnumBackgroundCopyFiles fileList = null;
			this.job.EnumFiles(out fileList);
			this.files = new BitsFiles(this, fileList);
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
		return this.files;
	}

	public BitsFiles Files {
		get { return this.files; }
	}

	public uint ErrorCount {
		get {
			uint count = 0;
			try {
				this.job.GetErrorCount(out count);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return count;
		}
	}

	public BitsError Error {
		get {
			try {
				JobState state = this.State;
				if (state == JobState.Error || state == JobState.TransientError) {

					if (null == this.error) {
						IBackgroundCopyError error;
						this.job.GetError(out error);
						if (null != error) {
							this.error = new BitsError(this, error);
						}
					}
				}
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return this.error;
		}
	}

	public Guid JobId {
		get {
			try {
				if (this.guid == Guid.Empty) {
					this.job.GetId(out guid);
				}
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return this.guid;
		}
	}

	public JobState State {
		get {
			BG_JOB_STATE state = BG_JOB_STATE.BG_JOB_STATE_UNKNOWN;
			try {
				this.job.GetState(out state);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return (JobState)state;
		}
	}

	public JobTimes JobTimes {
		get {
			try {
				if (null == this.jobTimes) {
					BG_JOB_TIMES times;
					this.job.GetTimes(out times);
					this.jobTimes = new JobTimes(times);
				}
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return this.jobTimes;
		}
	}

	public JobType JobType {
		get {
			BG_JOB_TYPE jobType = BG_JOB_TYPE.BG_JOB_TYPE_UNKNOWN;
			try {
				this.job.GetType(out jobType);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return (JobType)jobType;
		}
	}

	public ProxySettings ProxySettings {
		get {
			if (null == proxySettings) {
				this.proxySettings = new ProxySettings(this.job);
			}
			return this.proxySettings;
		}
	}

	public void Suspend() {
		try {
			this.job.Suspend();
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void Resume() {
		try {
			this.job.Resume();
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void Cancel() {
		try {
			this.job.Cancel();
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void Complete() {
		try {
			this.job.Complete();
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void TakeOwnership() {
		try {
			this.job.TakeOwnership();
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void AddFile(string remoteName, string localName) {
		try {
			this.job.AddFile(remoteName, localName);
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void AddFiles(ArrayList files) {
		try {
			uint count = Convert.ToUInt32(files.Count);
			BG_FILE_INFO[] fileArray = files.ToArray(typeof(BG_FILE_INFO)) as BG_FILE_INFO[];
			this.job.AddFileSet(count, ref fileArray);
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void AddCredentials(BitsCredentials credentials) {
		try {
			if (job2 != null) // only supported from IBackgroundCopyJob2 and above
			{
				BG_AUTH_CREDENTIALS bgCredentials = new BG_AUTH_CREDENTIALS();
				bgCredentials.Scheme = (BG_AUTH_SCHEME)credentials.AuthenticationScheme;
				bgCredentials.Target = (BG_AUTH_TARGET)credentials.AuthenticationTarget;
				bgCredentials.Credentials.Basic.Password = credentials.Password.ToString();
				bgCredentials.Credentials.Basic.UserName = credentials.UserName.ToString();
				job2.SetCredentials(ref bgCredentials);
			}
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void RemoveCredentials(BitsCredentials credentials) {
		try {
			if (job2 != null) // only supported from IBackgroundCopyJob2 and above
			{
				job2.RemoveCredentials((BG_AUTH_TARGET)credentials.AuthenticationTarget, (BG_AUTH_SCHEME)credentials.AuthenticationScheme);
			}
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public void RemoveCredentials(AuthenticationTarget target, AuthenticationScheme scheme) {
		try {
			if (job2 != null) // only supported from IBackgroundCopyJob2 and above
			{
				job2.RemoveCredentials((BG_AUTH_TARGET)target, (BG_AUTH_SCHEME)scheme);
			}
		} catch (COMException exception) {
			manager.PublishException(this, exception);
		}
	}

	public NotificationFlags NotificationFlags {
		get {
			uint flags = 0;
			try {
				job.GetNotifyFlags(out flags);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return (NotificationFlags)flags;
		}
		set {
			try {
				job.SetNotifyFlags((uint)value);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
		}
	}

	internal IBackgroundCopyCallback NotificationInterface {
		get {
			object notificationInterface = null;
			try {
				job.GetNotifyInterface(out notificationInterface);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
			return notificationInterface as IBackgroundCopyCallback;
		}
		set {
			try {
				job.SetNotifyInterface(value);
			} catch (COMException exception) {
				manager.PublishException(this, exception);
			}
		}
	}

	#endregion

	#region notification

	internal void OnJobTransferred(object sender, NotificationEventArgs e) {
		if (this.onJobTransfered != null)
			this.onJobTransfered(sender, new JobNotificationEventArgs());
	}

	internal void OnJobModified(object sender, NotificationEventArgs e) {
		if (this.onJobModified != null)
			this.onJobModified(sender, new JobNotificationEventArgs());
	}

	internal void OnJobError(object sender, ErrorNotificationEventArgs e) {
		if (null != this.onJobErrored)
			this.onJobErrored(sender, new JobErrorNotificationEventArgs(e.Error));
	}

	#endregion

	#region public events

	public event EventHandler<JobNotificationEventArgs> OnJobModifiedEvent {
		add { this.onJobModified += value; }
		remove { this.onJobModified -= value; }
	}

	public event EventHandler<JobNotificationEventArgs> OnJobTransferredEvent {
		add { this.onJobTransfered += value; }
		remove { this.onJobTransfered -= value; }
	}

	public event EventHandler<JobErrorNotificationEventArgs> OnJobErrorEvent {
		add { this.onJobErrored += value; }
		remove { this.onJobErrored -= value; }
	}

	#endregion

	#region internal

	internal IBackgroundCopyJob Job {
		get { return this.job; }
	}

	internal void PublishException(COMException exception) {
		this.manager.PublishException(this, exception);
	}

	#endregion

	#region IDisposable Members

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (!this.disposed) {
			if (disposing) {
				//TODO: release COM resource
				this.job = null;
			}
		}
		disposed = true;
	}

	#endregion

}
