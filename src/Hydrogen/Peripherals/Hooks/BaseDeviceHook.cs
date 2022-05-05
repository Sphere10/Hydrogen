//-----------------------------------------------------------------------
// <copyright file="BaseDeviceHook.cs" company="Sphere 10 Software">
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
using System.Threading;


namespace Hydrogen {

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
}

