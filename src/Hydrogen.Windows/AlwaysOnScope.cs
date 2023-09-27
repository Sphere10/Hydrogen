// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows;

public class AlwaysOnScope : IDisposable {
	private readonly uint _priorState;
	public AlwaysOnScope(bool system, bool display) {
		var mode = (uint)WinAPI.KERNEL32.ES_CONTINUOUS;
		if (system)
			mode = mode | WinAPI.KERNEL32.ES_SYSTEM_REQUIRED;
		if (display)
			mode = mode | WinAPI.KERNEL32.ES_DISPLAY_REQUIRED;
		_priorState = WinAPI.KERNEL32.SetThreadExecutionState(mode);

	}

	public void Dispose() {
		WinAPI.KERNEL32.SetThreadExecutionState(_priorState);
	}
}
