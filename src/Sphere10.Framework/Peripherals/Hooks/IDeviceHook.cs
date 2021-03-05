//-----------------------------------------------------------------------
// <copyright file="IDeviceHook.cs" company="Sphere 10 Software">
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


namespace Sphere10.Framework  {

	public interface IDeviceHook : IDisposable  {
		DeviceHookStatus Status { get; }
		bool ProcessAsyncronously { get; set; }
		void InstallHook();
		void StartHook();
		void StopHook();
		void DisableHook(TimeSpan timespan);
		void UninstallHook();
	}
}
