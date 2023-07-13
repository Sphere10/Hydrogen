// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation2.Logic {
	public interface IApplicationBlock {
		int Position { get; }
		string Title { get; }
		string IconUrl { get; }
		string Tooltip { get; }
		IReadOnlyList<IApplicationMenu> Menus { get; }


	}

}
