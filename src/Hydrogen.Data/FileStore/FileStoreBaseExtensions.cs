using System.Linq;

namespace Hydrogen.Data;

public static class FileStoreBaseExtensions  {
	public static string RegisterFile<TFileKeyType>(this FileStoreBase<TFileKeyType> fileStore, TFileKeyType fileKey) {
		return fileStore.RegisterMany(new[] { fileKey }).Single();
	}

	public static void Delete<TFileKeyType>(this FileStoreBase<TFileKeyType> fileStore, TFileKeyType fileKey) {
		fileStore.DeleteMany(new[] { fileKey });
	}
}
