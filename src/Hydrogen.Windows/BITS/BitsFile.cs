// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.InteropServices;

namespace Hydrogen.Windows.BITS;

public class BitsFile : IDisposable {
	private IBackgroundCopyFile file;
	private FileProgress progress;
	private bool disposed;
	private BitsJob job;

	internal BitsFile(BitsJob job, IBackgroundCopyFile file) {
		if (null == file)
			throw new ArgumentNullException("IBackgroundCopyFile");
		this.file = file;
		this.job = job;
	}

	#region public properties

	public string LocalName {
		get {
			string name = string.Empty;
			try {
				this.file.GetLocalName(out name);
			} catch (COMException exception) {
				this.job.PublishException(exception);
			}
			return name;
		}
	}

	public string RemoteName {
		get {
			string name = string.Empty;
			try {
				this.file.GetRemoteName(out name);
			} catch (COMException exception) {
				this.job.PublishException(exception);
			}
			return name;
		}
	}

	public FileProgress Progress {
		get {
			if (null == this.progress) {
				BG_FILE_PROGRESS progress;
				try {
					this.file.GetProgress(out progress);
					this.progress = new FileProgress(progress);
				} catch (COMException exception) {
					this.job.PublishException(exception);
				}
			}
			return this.progress;
		}
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
				this.file = null;
			}
		}
		disposed = true;
	}

	#endregion

}
