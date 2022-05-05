using NUnit.Framework;

namespace Hydrogen.Tests;

internal class ClusteredStoragePolicyTestValuesAttribute : ValuesAttribute {
	public ClusteredStoragePolicyTestValuesAttribute()
		: base(ClusteredStoragePolicy.Default, ClusteredStoragePolicy.BlobOptimized, ClusteredStoragePolicy.Debug, ClusteredStoragePolicy.Default | ClusteredStoragePolicy.CacheRecords) {
	}
}