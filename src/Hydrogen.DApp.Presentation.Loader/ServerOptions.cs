// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation.Loader;

/// <summary>
/// Data source options
/// </summary>
public class DataSourceOptions {
	/// <summary>
	/// Gets or sets the configured servers.
	/// </summary>
	public List<Uri> Servers { get; } = new();
}
