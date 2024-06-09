// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;


namespace Hydrogen;

public abstract class BaseDeviceHook : IDeviceHook {
	protected object _syncObject;

	public BaseDeviceHook() {
		_syncObject = new object();
		ProcessAsyncronously = true;
		Status = DeviceHookStatus.Uninstalled;
		Disposed = false;
		Disposing = false;
	}

	~BaseDeviceHook() {
		if (!Disposed) {
			Dispose();
		}
	}

	public bool ProcessAsyncronously { get; set; }
	public DeviceHookStatus Status { get; protected set; }
	protected bool Disposed { get; set; }
	protected bool Disposing { get; set; }

	public abstract void InstallHook();

	public virtual void StartHook() {
		lock (_syncObject) {
			if (Status == DeviceHookStatus.Uninstalled) {
				InstallHook();
			}
			Status = DeviceHookStatus.Active;
		}
	}

	public virtual void StopHook() {
		lock (_syncObject) {
			if (Status != DeviceHookStatus.Uninstalled) {
				Status = DeviceHookStatus.Disabled;
			}
		}
	}

	public virtual void DisableHook(TimeSpan timespan) {
		lock (_syncObject) {
			StopHook();
			Tools.Lambda.ActionAsAsyncronous(() => {
				Thread.Sleep(timespan);
				StartHook();
			});
		}
	}

	public abstract void UninstallHook();

	public virtual void Dispose() {
		Disposing = true;
		lock (_syncObject) {
			try {
				StopHook();
			} finally {
				if (Status != DeviceHookStatus.Uninstalled) {
					UninstallHook();
				}
				Disposed = true;
			}
		}
	}

	protected void ContinueOutsideCriticalExecutionContext(Action action) {
		if (ProcessAsyncronously) {
			action.AsAsyncronous().Invoke();
		} else {
			action.Invoke();
		}
	}


}
