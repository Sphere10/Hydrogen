using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamMappedList<TItem> : IExtendedList<TItem> {
		IClusteredStorage Storage { get; }

		IItemSerializer<TItem> ItemSerializer { get; }

		IEqualityComparer<TItem> ItemComparer { get; }

		ClusteredStreamScope EnterAddScope(TItem item);

		ClusteredStreamScope EnterInsertScope(int index, TItem item);

		ClusteredStreamScope EnterUpdateScope(int index, TItem item);
	}

}
