// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace Hydrogen.Windows.BITS;

public class BitsError {
	private IBackgroundCopyError error;
	private BitsJob job;

	internal BitsError(BitsJob job, IBackgroundCopyError error) {
		if (null == error)
			throw new ArgumentNullException("IBackgroundCopyError");
		this.error = error;
		this.job = job;
	}

	public string Description {
		get {
			string description = string.Empty;
			try {
				this.error.GetErrorDescription(Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
			} catch (COMException exception) {
				this.job.PublishException(exception);
			}
			return description;
		}
	}

	public string ContextDescription {
		get {
			string description = string.Empty;
			try {
				this.error.GetErrorContextDescription(Convert.ToUInt32(Thread.CurrentThread.CurrentUICulture.LCID), out description);
			} catch (COMException exception) {
				this.job.PublishException(exception);
			}
			return description;
		}
	}

	public string Protocol {
		get {
			string protocol = string.Empty;
			try {
				this.error.GetProtocol(out protocol);
			} catch (COMException exception) {
				this.job.PublishException(exception);
			}
			return protocol;
		}
	}

	public BitsFile File {
		get {
			IBackgroundCopyFile errorFile;
			try {
				this.error.GetFile(out errorFile);
				return new BitsFile(this.job, errorFile);
			} catch (COMException exception) {
				this.job.PublishException(exception);
			}
			return null; //couldn't create new job
		}
	}

	public ErrorContext ErrorContext {
		get {
			BG_ERROR_CONTEXT context;
			int errorCode;
			try {
				this.error.GetError(out context, out errorCode);
				return (ErrorContext)context;
			} catch (COMException exception) {
				this.job.PublishException(exception);
			}
			return ErrorContext.Unknown;
		}
	}

	public int ErrorCode {
		get {
			BG_ERROR_CONTEXT context;
			int errorCode = 0;
			try {
				this.error.GetError(out context, out errorCode);
				return errorCode;
			} catch (COMException exception) {
				this.job.PublishException(exception);
			}
			return errorCode;
		}
	}

}
