// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;

namespace Hydrogen {

	public interface IStreamMappedList<TItem> : IExtendedList<TItem> {
		IClusteredStorage Storage { get; }

		IItemSerializer<TItem> ItemSerializer { get; }

		IEqualityComparer<TItem> ItemComparer { get; }

		ClusteredStreamScope EnterAddScope(TItem item);

		ClusteredStreamScope EnterInsertScope(int index, TItem item);

		ClusteredStreamScope EnterUpdateScope(int index, TItem item);
	}

}
