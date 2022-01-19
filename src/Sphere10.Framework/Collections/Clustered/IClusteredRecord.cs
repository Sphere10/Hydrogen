namespace Sphere10.Framework {

	public interface IClusteredRecord : IStreamRecord {
		int StartCluster { get; set; }
	}
}
