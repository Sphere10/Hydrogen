// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths.Compiler;

public class CompilerUtil {

	public static bool IsValidVariable(string var) {
		bool retval = false;
		if (var.Length > 0) {
			// first character must be letter
			if (char.IsLetter(var[0])) {
				// all remaining characters must be letters or digits
				retval = true;
				foreach (char c in var.Substring(1).ToCharArray()) {
					if (!char.IsLetterOrDigit(c)) {
						retval = false;
						break;
					}
				}
			}
		}
		return retval;
	}


}
