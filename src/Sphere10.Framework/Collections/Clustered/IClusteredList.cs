using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IClusteredList<TItem> : IExtendedList<TItem> {
		IClusteredStorage Storage { get; }

		IItemSerializer<TItem> ItemSerializer { get; }

		IEqualityComparer<TItem> ItemComparer { get; }

		public ClusteredStorageScope EnterAddScope(TItem item);

		public ClusteredStorageScope EnterInsertScope(int index, TItem item);

		public ClusteredStorageScope EnterUpdateScope(int index, TItem item);
	}


}
