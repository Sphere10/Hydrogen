using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamContainer<out TStreamListing> : IStreamContainer
		where TStreamListing : IStreamListing, new() { 

		IReadOnlyList<TStreamListing> Listings { get; }

	}


}
