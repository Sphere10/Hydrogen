namespace Sphere10.Framework {
	internal class Cluster {
		public ClusterTraits Traits { get; set; }
		public int Prev { get; set; }
		public int Next { get; set; }
		public byte[] Data { get; set; }
		public override string ToString() => $"[{nameof(Cluster)}] {nameof(Traits)}: {Traits}, {nameof(Prev)}: {Prev}, {nameof(Next)}: {Next}, {nameof(Data)}: {Data.ToHexString(true)}";
	}
}
