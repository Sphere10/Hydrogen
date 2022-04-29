//-----------------------------------------------------------------------
// <copyright file="PollDrivenActiveApplicationMonitor.cs" company="Sphere 10 Software">
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
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Sphere10.Framework;


namespace Sphere10.Framework.Windows {

	public class PollDrivenActiveApplicationMonitor : IActiveApplicationMonitor {
		public event EventHandler<ApplicationChangedEvent> ApplicationChanged;
		private IntPtr _activeWindowHandle;
		private string _activeProcess;
		private DateTime _lastAppEventRaised = DateTime.UtcNow;

		//public bool Disabled { get; private set; }


		protected virtual void OnActiveApplicationChanged(ApplicationChangedEvent applicationChangedEvent) {
		}


		protected virtual void FireApplicationChangedEVent(ApplicationChangedEvent applicationChangedEvent) {
			OnActiveApplicationChanged(applicationChangedEvent);
			if (ApplicationChanged != null) {
				ApplicationChanged(this, applicationChangedEvent);
			}
		}

		public string GetActiveApplicationName() {
			try {
				//if (!Disabled) {
					// get the users most foreground window and assume that the mouse/key activity
					// is directed at the process hosting that window. 
					var hwnd = WinAPI.USER32.GetForegroundWindow();

					// Only get the process name if the window has changed since last time this we
					// determine the source process name.
					if (hwnd != _activeWindowHandle || _activeProcess == string.Empty) {
						_activeWindowHandle = hwnd;
						uint activeProcessID = 0;
                        WinAPI.USER32.GetWindowThreadProcessId(hwnd, out activeProcessID);
						string newActiveProcess = 
							Process.GetProcessById(
							(int)activeProcessID).MainModule.FileName;

						// Raise on application changed event
						if (newActiveProcess != _activeProcess && _activeProcess != string.Empty) {
							var now = DateTime.UtcNow;
							var duration = now.Subtract(_lastAppEventRaised);
							FireApplicationChangedEVent(
								new ApplicationChangedEvent(
									newActiveProcess,
									_activeProcess,
									now,
									duration
								)
							);
							_lastAppEventRaised = now;
						}
						_activeProcess = newActiveProcess;
					}
			//	}
			} catch {
				//if (!Disabled) {
				//    Disable(TimeSpan.FromSeconds(10));
				//}

			}
			return _activeProcess;
		}
	}
}
