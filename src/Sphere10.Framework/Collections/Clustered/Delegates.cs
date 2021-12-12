using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public delegate TListing ClusteredListingActivator<in TItem, out TListing>(object source, TItem item, int itemSizeBytes, int clusterStartIndex);

}
