// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Windows.BITS;

public class BitsJobs : Dictionary<Guid, BitsJob>, IDisposable {
	private IEnumBackgroundCopyJobs jobList;
	BitsManager manager;
	private bool disposed;

	internal BitsJobs(BitsManager manager, IEnumBackgroundCopyJobs jobList) {
		this.manager = manager;
		this.jobList = jobList;
		this.Update();
	}

	internal void Update(IEnumBackgroundCopyJobs jobList) {
		this.jobList = jobList;
		this.Update();
	}

	internal void Update() {
		uint count;
		IBackgroundCopyJob currentJob;
		uint fetchedCount = 0;
		BitsJob job;
		Dictionary<Guid, BitsJob> currentList = new Dictionary<Guid, BitsJob>();
		foreach (KeyValuePair<Guid, BitsJob> entry in this) {
			currentList.Add(entry.Key, entry.Value);
		}
		this.jobList.Reset();
		this.Clear();
		this.jobList.GetCount(out count);
		for (int i = 0; i < count; i++) {
			this.jobList.Next(1, out currentJob, out fetchedCount);
			if (fetchedCount == 1) {
				Guid guid;
				currentJob.GetId(out guid);
				if (currentList.ContainsKey(guid)) {
					job = currentList[guid];
					currentList.Remove(guid);
				} else {
					job = new BitsJob(manager, currentJob);
				}
				this.Add(job.JobId, job);
			}
		}
		foreach (BitsJob disposeJob in currentList.Values) {
			manager.NotifyOnJobRemoval(disposeJob);
			disposeJob.Dispose();
		}
	}

	internal IEnumBackgroundCopyJobs Jobs {
		get { return this.jobList; }
	}

	#region IDisposable Members

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (!this.disposed) {
			if (disposing) {
				//TODO: release COM resource
				this.jobList = null;
			}
		}
		disposed = true;
	}

	#endregion

}
