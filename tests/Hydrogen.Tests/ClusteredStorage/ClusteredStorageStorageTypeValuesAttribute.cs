using NUnit.Framework;

namespace Hydrogen.Tests;

internal class ClusteredStorageStorageTypeValuesAttribute : ValuesAttribute {
	public ClusteredStorageStorageTypeValuesAttribute()
		: base(StreamPersistedCollectionTestsBase.StorageType.MemoryStream) {
	}
}
