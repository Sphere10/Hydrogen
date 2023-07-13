// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace Hydrogen;

public static class NumericUpDownExtensions {

	public static double GetValueDouble(this NumericUpDown numericUpDown) {
		return (double)numericUpDown.Value;
	}

	public static int GetValueInt(this NumericUpDown numericUpDown) {
		return (int)Math.Round(numericUpDown.Value, 0);
	}

}
