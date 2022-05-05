//-----------------------------------------------------------------------
// <copyright file="NumericUpDownExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hydrogen {

	public static class NumericUpDownExtensions {

		public static double GetValueDouble(this NumericUpDown numericUpDown){
			return (double)numericUpDown.Value;
		}

		public static int GetValueInt(this NumericUpDown numericUpDown) {
			return(int)Math.Round(numericUpDown.Value,0);
		}

	}
}
