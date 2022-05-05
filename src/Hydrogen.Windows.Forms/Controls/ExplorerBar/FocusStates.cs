//-----------------------------------------------------------------------
// <copyright file="FocusStates.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.Forms {

	/// <summary>
	/// Defines the state of an Expandos title bar
	/// </summary>
	public enum FocusStates {
		/// <summary>
		/// Normal state
		/// </summary>
		None = 0,

		/// <summary>
		/// The mouse is over the title bar
		/// </summary>
		Mouse = 1
	}

}
