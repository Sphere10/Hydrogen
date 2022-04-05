using NUnit.Framework;

namespace Sphere10.Framework.Tests;

internal class ClusteredStorageStorageTypeValuesAttribute : ValuesAttribute {
	public ClusteredStorageStorageTypeValuesAttribute()
		: base(StreamPersistedTestsBase.StorageType.MemoryStream) {
	}
}
