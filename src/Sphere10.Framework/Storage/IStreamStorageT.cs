using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamStorageT<out TStreamListing> : IStreamStorage
		where TStreamListing : IStreamListing, new() { 

		IReadOnlyList<TStreamListing> Listings { get; }

	}

}
