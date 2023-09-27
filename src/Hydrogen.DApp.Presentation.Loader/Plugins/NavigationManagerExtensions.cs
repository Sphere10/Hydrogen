// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Microsoft.AspNetCore.Components;

namespace Hydrogen.DApp.Presentation.Loader.Plugins;

public static class NavigationManagerExtensions {
	/// <summary>
	/// Returns the path relative to the base including a initial slash as per page route constraints.
	/// </summary>
	/// <param name="navigationManager"></param>
	/// <param name="path"> path to convert</param>
	/// <returns> path relative to base.</returns>
	public static string ToBaseRelativePathWithSlash(this NavigationManager navigationManager, string path) {
		string baseRealtivePath = navigationManager.ToBaseRelativePath(path);
		return "/" + baseRealtivePath;
	}
}
