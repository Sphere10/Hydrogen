// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using System.Diagnostics;

namespace Hydrogen;

public class SoftwareException : ApplicationException {

	private int _threadID = -1;
	private int _terminalServicesSessionID = -1;
	private System.Threading.ThreadState _threadState;
	private int _processID = -1;
	private string _machineName = string.Empty;
	private string _processName = string.Empty;
	private string _userName = string.Empty;
	private int _currentCultureID = -1;
	private int _currentUICultureID = -1;


	public SoftwareException()
		: this("An unexpected error has occured") {
	}

	public SoftwareException(Result result) : this(result.ErrorMessages.ToParagraphCase()) {
	}

	public SoftwareException(string error)
		: this(error, new object[0]) {
	}

	public SoftwareException(string error, params object[] formatArgs)
		: this(null, error, formatArgs) {
	}

	public SoftwareException(Exception innerException, string error, params object[] formatArgs)
		: this(formatArgs.Length > 0 ? string.Format(error, formatArgs) : error, innerException) {
	}

	public SoftwareException(string error, Exception innerException)
		: base(error, innerException) {
		try {
			Process currentProcess = Process.GetCurrentProcess();
			TerminalServicesSessionID = currentProcess.SessionId;
			ProcessID = currentProcess.Id;
			ProcessName = currentProcess.ProcessName;
			MachineName = currentProcess.MachineName;
			CurrentCultureID = Thread.CurrentThread.CurrentCulture.LCID;
			CurrentUICultureID = Thread.CurrentThread.CurrentUICulture.LCID;
			ThreadID = Thread.CurrentThread.ManagedThreadId;
			ThreadState = Thread.CurrentThread.ThreadState;
			TerminalServicesSessionID = -1;
			ProcessID = -1;
			ProcessName = "unknown";
			MachineName = "unknown";
			CurrentCultureID = -1;
			CurrentUICultureID = -1;
			ThreadID = -1;
		} catch {
			// do nothing
		}
	}


	public int TerminalServicesSessionID {
		get { return _terminalServicesSessionID; }
		set { _terminalServicesSessionID = value; }
	}

	public int ThreadID {
		get { return _threadID; }
		set { _threadID = value; }
	}

	public System.Threading.ThreadState ThreadState {
		get { return _threadState; }
		set { _threadState = value; }
	}

	public int ProcessID {
		get { return _processID; }
		set { _processID = value; }
	}

	public string MachineName {
		get { return _machineName; }
		set { _machineName = value; }
	}

	public string ProcessName {
		get { return _processName; }
		set { _processName = value; }
	}

	public string UserName {
		get { return _userName; }
		set { _userName = value; }
	}


	public int CurrentCultureID {
		get { return _currentCultureID; }
		set { _currentCultureID = value; }
	}

	public int CurrentUICultureID {
		get { return _currentUICultureID; }
		set { _currentUICultureID = value; }
	}

}
