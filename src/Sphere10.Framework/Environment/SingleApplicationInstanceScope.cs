//-----------------------------------------------------------------------
// <copyright file="SingleApplicationInstanceScope.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Reflection;

namespace Sphere10.Framework {

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
	public class SingleApplicationInstanceScope : IDisposable {
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

		public void Dispose() {
			((IDisposable)_mutex).Dispose();
		}
	}

}
