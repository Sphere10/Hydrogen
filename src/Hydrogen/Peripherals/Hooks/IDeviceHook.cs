// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IDeviceHook : IDisposable {
	DeviceHookStatus Status { get; }
	bool ProcessAsyncronously { get; set; }

	void InstallHook();

	void StartHook();

	void StopHook();

	void DisableHook(TimeSpan timespan);

	void UninstallHook();
}
