// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public static class BoolExtensions {
	public static string YesNo(this bool value) {
		if (value)
			return "Yes";
		else
			return "No";
	}

	public static string OpenClosed(this bool value) {
		if (value)
			return "Closed";
		else
			return "Open";
	}
}
