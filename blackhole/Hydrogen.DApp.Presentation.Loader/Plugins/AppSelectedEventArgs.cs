// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Plugins;

/// <summary>
/// Args for app selected event.
/// </summary>
public class AppSelectedEventArgs : EventArgs {
	/// <summary>
	/// Gets the newly selected app.
	/// </summary>
	public IApp SelectedApp { get; }

	public AppSelectedEventArgs(IApp selectedApp) {
		SelectedApp = selectedApp;
	}
}
