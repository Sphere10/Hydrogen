// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Plugins;

/// <summary>
/// App manager
/// </summary>
public interface IAppManager {
	/// <summary>
	/// Raised when an app is selected
	/// </summary>
	event EventHandler<AppSelectedEventArgs> AppSelected;

	/// <summary>
	/// Raised when an app page is selected.
	/// </summary>
	event EventHandler<AppBlockPageSelectedEventArgs> AppBlockPageSelected;

	/// <summary>
	/// Gets the available apps.
	/// </summary>
	IEnumerable<IApp> Apps { get; }

	/// <summary>
	/// Gets or sets the selected app.
	/// </summary>
	IApp? SelectedApp { get; }

	/// <summary>
	/// Selected page
	/// </summary>
	IAppBlockPage? SelectedPage { get; }
}
