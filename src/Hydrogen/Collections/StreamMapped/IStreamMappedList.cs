// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public interface IStreamMappedList<TItem> : IExtendedList<TItem>, ILoadable {
	IClusteredStorage Storage { get; }

	IItemSerializer<TItem> ItemSerializer { get; }

	IEqualityComparer<TItem> ItemComparer { get; }

	ClusteredStreamScope EnterAddScope(TItem item);

	ClusteredStreamScope EnterInsertScope(long index, TItem item);

	ClusteredStreamScope EnterUpdateScope(long index, TItem item);
}
