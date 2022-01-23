using System.IO;
using System.Text.RegularExpressions;

namespace Sphere10.Framework {

	public interface IClusteredStorageHeader : IStreamStorageHeader {
		int ClusterSize { get; set; }
		int TotalClusters { get; set; }
	}

}
