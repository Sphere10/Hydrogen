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
/// Page selected event args
/// </summary>
public class AppBlockPageSelectedEventArgs : EventArgs {
	/// <summary>
	/// Gets the selected page
	/// </summary>
	public IAppBlockPage AppBlockPage { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="AppBlockPageSelectedEventArgs"/> class.
	/// </summary>
	/// <param name="appBlockPage"> selected page</param>
	public AppBlockPageSelectedEventArgs(IAppBlockPage appBlockPage) {
		AppBlockPage = appBlockPage ?? throw new ArgumentNullException(nameof(appBlockPage));
	}
}
