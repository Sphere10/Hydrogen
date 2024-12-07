// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation.Plugins;

public interface IApp : IRoutablePage, INamedItem, IIconItem {
	/// <summary>
	/// Gets the app blocks that are part of this 
	/// </summary>
	IEnumerable<IAppBlock> AppBlocks { get; }
}
