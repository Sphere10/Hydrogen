namespace Hydrogen.ObjectSpaces;

public class ObjectSpaceFile {
	public string FilePath { get; set; }

	public string PageFileDir { get; set; }

	public long PageSize { get; set; }

	public long MaxMemory { get; set; }

	public long ClusterSize { get; set; }

	public StreamContainerPolicy ContainerPolicy { get; set; }
}
