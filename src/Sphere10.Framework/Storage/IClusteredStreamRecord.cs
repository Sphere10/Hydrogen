namespace Sphere10.Framework {

	public interface IClusteredStreamRecord : IStreamRecord {
		int StartCluster { get; set; }
	}
}
