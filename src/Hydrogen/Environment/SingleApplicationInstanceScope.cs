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

/// <summary>
/// Used to prevent
/// </summary>
/// <example>
///		[1] Quits application if application is already running
///        static void Main(string[] args) {
///				using (new SingleApplicationScope()) {
///					...
///				}
///			}
///			
///		[2] Throws exception instead of quits
///        static void Main(string[] args) {
///			 try {
///					using (new SingleApplicationScope(throwInsteadOfQuit: true)) {
///						...
///					}
///				} catch (ApplicationAlreadyRunningException e) {
///					.. notify user
///				}
///			}
/// </example>
public class SingleApplicationInstanceScope : SyncScope {
	public Mutex _mutex;

	public SingleApplicationInstanceScope(bool throwInsteadOfQuit = false, int exitCode = -1) {
		bool singleInstance;
		_mutex = new Mutex(false, "Local\\" + Tools.Runtime.GetEntryAssembly().FullName, out singleInstance);
		if (!singleInstance) {
			if (throwInsteadOfQuit) {
				throw new ApplicationAlreadyRunningException();
			}
			Environment.Exit(exitCode);
		}
	}

	protected override void OnScopeEnd() {
		((IDisposable)_mutex).Dispose();
	}

}
