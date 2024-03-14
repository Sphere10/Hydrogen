namespace Hydrogen;

public record HydrogenFileDescriptor : TransactionalFileDescriptor {

	public int ClusterSize { get; init; }

	public ClusteredStreamsPolicy ContainerPolicy { get; init; }

	public Endianness Endianness { get; init; }

	public new static HydrogenFileDescriptor From(string path) 
		=> From(path, HydrogenDefaults.TransactionalPageFolder, HydrogenDefaults.TransactionalPageSize, HydrogenDefaults.MaxMemoryPerCollection, HydrogenDefaults.ClusterSize, HydrogenDefaults.ContainerPolicy);

	public new static HydrogenFileDescriptor From(string path, long pageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection)
		=> From(path, HydrogenDefaults.TransactionalPageFolder, pageSize, maxMemory);

	private new static HydrogenFileDescriptor From(string path, string pagesDirectoryPath, long pageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection) 
		=> From(path, pagesDirectoryPath, pageSize, maxMemory, HydrogenDefaults.ClusterSize, HydrogenDefaults.ContainerPolicy);


	public static HydrogenFileDescriptor From(string path, long pageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection, int clusterSize = HydrogenDefaults.ClusterSize, ClusteredStreamsPolicy containerPolicy = HydrogenDefaults.ContainerPolicy)
		=> From(path, HydrogenDefaults.TransactionalPageFolder, pageSize, maxMemory, clusterSize, containerPolicy);

	public static HydrogenFileDescriptor From(string path, string pagesDirectoryPath, long pageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection, int clusterSize = HydrogenDefaults.ClusterSize, ClusteredStreamsPolicy containerPolicy = HydrogenDefaults.ContainerPolicy, Endianness endianness = HydrogenDefaults.Endianness) 
		=> new() { 
			Path = path,
			PagesDirectoryPath = pagesDirectoryPath,
			PageSize = pageSize,
			MaxMemory = maxMemory,
			ClusterSize = clusterSize,
			ContainerPolicy = containerPolicy,
			Endianness = endianness
		};
}
