using System.Linq;

namespace Hydrogen.Data;

public static class IFileStoreExtensions  {
	public static string RegisterFile<TFileKeyType>(this IFileStore<TFileKeyType> fileStoreBase, TFileKeyType fileKey) {
		return fileStoreBase.RegisterMany(new[] { fileKey }).Single();
	}

	public static void Delete<TFileKeyType>(this IFileStore<TFileKeyType> fileStoreBase, TFileKeyType fileKey) {
		fileStoreBase.DeleteMany(new[] { fileKey });
	}
}
