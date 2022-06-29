namespace Hydrogen.Data;

public static class FileStoreBaseExtensions  {
	public static void RegisterFile<TfileKeyType>(this FileStoreBase<TfileKeyType> fileStore, TfileKeyType fileKey) {
		fileStore.RegisterMany(new[] { fileKey });
	}

	public static void Delete<TfileKeyType>(this FileStoreBase<TfileKeyType> fileStore, TfileKeyType fileKey) {
		fileStore.DeleteMany(new[] { fileKey });
	}
}
