namespace Hydrogen;

public record PagedFileDescriptor {

	public string Path { get; init; } 

	public string CaseCorrectPath => Tools.FileSystem.GetCaseCorrectFilePath(Path);

	public long PageSize { get; init; }

	public long MaxMemory { get; init; }

	public static PagedFileDescriptor From(string path) 
		=> From(path, HydrogenDefaults.TransactionalPageSize, HydrogenDefaults.MaxMemoryPerCollection);

	public static PagedFileDescriptor From(string path, long pageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection) 
		=> new() { 
			Path = path,
			PageSize = pageSize,
			MaxMemory = maxMemory
		};

}
