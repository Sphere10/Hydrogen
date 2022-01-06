namespace Sphere10.Framework {
	internal class Cluster {
		public ClusterTraits Traits { get; set; }
		public int Prev { get; set; }
		public int Next { get; set; }
		public byte[] Data { get; set; }

		public override string ToString() => $"[Cluster] Traits: {Traits}, Prev: {Prev}, Next: {Next}, Data: {Data.ToHexString(true)}";
	}
}
