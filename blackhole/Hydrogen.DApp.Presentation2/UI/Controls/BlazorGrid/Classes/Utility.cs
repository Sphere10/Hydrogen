// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text.RegularExpressions;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public static class Utility {

		public static bool IsNullOrEmpty(this string text) {
			return string.IsNullOrEmpty(text);
		}
		public static string AddSpacesForCamelCase(string text) {
			if (text.IsNullOrEmpty()) return string.Empty;

			return Regex.Replace(text, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
		}
	}
}
