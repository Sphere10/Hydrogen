// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;

namespace Hydrogen {

	public enum AMSOTS : byte{
		[Description("N/A")]
		NotApplicable = 0,
		
		[Description("W-OTS")]
		WOTS = 1,

		[Description("W-OTS+")]
		WOTS_Plus = 2,

		[Description("W-OTS#")]
		WOTS_Sharp = 3,
	}

}
