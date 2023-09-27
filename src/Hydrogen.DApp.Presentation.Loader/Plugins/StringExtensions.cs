// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Presentation.Loader.Plugins;

/// <summary>
/// Extension methods to help with routing
/// </summary>
public static class StringExtensions {
	/// <summary>
	/// Given a relative path to the host, excluding the host name, determines the first
	/// segment of the path which should be the app's path. e.g. /myapp/page1?param=1 => /myapp
	/// </summary>
	/// <param name="relativePath"></param>
	/// <returns></returns>
	public static string ToAppPathFromBaseRelativePath(this string relativePath) {
		relativePath = relativePath.TrimFragment();

		int indexOfSegment = relativePath.IndexOf('/', 1);
		return relativePath.Substring(0, indexOfSegment > 0 ? indexOfSegment : relativePath.Length);
	}

	/// <summary>
	/// Given a relative path to the host, removes any query parameters. 
	/// </summary>
	/// <param name="relativePath"></param>
	/// <returns></returns>
	public static string TrimQueryParameters(this string relativePath) {
		relativePath = relativePath.TrimFragment();

		int queryIndex = relativePath.IndexOf('?');

		return queryIndex > 0 ? relativePath.Substring(0, queryIndex) : relativePath;
	}

	/// <summary>
	/// Remove fragment from path.
	/// </summary>
	/// <param name="relativePath"> path</param>
	/// <returns></returns>
	public static string TrimFragment(this string relativePath) {
		int fragmentIndex = relativePath.IndexOf('#');
		return fragmentIndex > 0 ? relativePath.Substring(0, fragmentIndex) : relativePath;
	}
}
