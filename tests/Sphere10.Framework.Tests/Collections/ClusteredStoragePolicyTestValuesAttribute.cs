using NUnit.Framework;

namespace Sphere10.Framework.Tests;

internal class ClusteredStoragePolicyTestValuesAttribute : ValuesAttribute {
	public ClusteredStoragePolicyTestValuesAttribute()
		: base(ClusteredStoragePolicy.Default, ClusteredStoragePolicy.BlobOptimized, ClusteredStoragePolicy.Debug, ClusteredStoragePolicy.Default | ClusteredStoragePolicy.CacheRecords) {
	}
}
