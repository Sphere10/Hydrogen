// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class NoOpActiveApplicationMonitor : IActiveApplicationMonitor {
	public event EventHandler<ApplicationChangedEvent> ApplicationChanged;

	public string GetActiveApplicationName() {
		return "N/A";
	}
}
