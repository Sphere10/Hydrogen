using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IClusteredList<TItem> : IExtendedList<TItem> {
		IClusteredStorage Storage { get; }

		IItemSerializer<TItem> ItemSerializer { get; }

		IEqualityComparer<TItem> ItemComparer { get; }

		public ClusteredStreamScope EnterAddScope(TItem item);

		public ClusteredStreamScope EnterInsertScope(int index, TItem item);

		public ClusteredStreamScope EnterUpdateScope(int index, TItem item);
	}


}
