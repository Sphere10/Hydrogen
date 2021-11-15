using System;

namespace Sphere10.Framework {
	public class ClusteredItemAccessedArgs<TItem, TListing> : EventArgs where TListing : IItemListing {
		public ListOperationType OperationType { get; init; }
		public int ListingIndex { get; init; }
		public TListing Listing { get; init; }
		public TItem Item { get; init; }
		public byte[] SerializedItem { get; init; }
	}
}
