namespace Sphere10.Framework {

	internal class Cluster {
		public ClusterTraits Traits { get; set; }
		public int Number { get; set; }
		public byte[] Data { get; set; }
		public int Next { get; set; }
	}
}
