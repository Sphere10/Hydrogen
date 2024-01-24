namespace Hydrogen;

public record TransactionalFileDescriptor : PagedFileDescriptor {
	
	public string PagesDirectoryPath { get; init; }

	public new static TransactionalFileDescriptor From(string path) 
		=> From(path, HydrogenDefaults.TransactionalPageFolder,  HydrogenDefaults.TransactionalPageSize, HydrogenDefaults.MaxMemoryPerCollection);

	public new static TransactionalFileDescriptor From(string path, long pageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection)
		=> From(path, HydrogenDefaults.TransactionalPageFolder, pageSize, maxMemory);

	public static TransactionalFileDescriptor From(string path, string pagesDirectoryPath, long pageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection) 
		=> new() { 
			Path = path,
			PagesDirectoryPath = pagesDirectoryPath,
			PageSize = pageSize,
			MaxMemory = maxMemory
		};

}
