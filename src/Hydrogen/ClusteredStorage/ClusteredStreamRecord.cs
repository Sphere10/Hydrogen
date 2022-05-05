using System.Runtime.InteropServices;

namespace Hydrogen {

	//[StructLayout(LayoutKind.Sequential)]
	public struct ClusteredStreamRecord {

		public ClusteredStreamTraits Traits { get; set; }

		public int Size { get; set; }

		public int StartCluster { get; set; }

		public int KeyChecksum { get; set; }

		public byte[] Key { get; set; }

		public override string ToString() => $"[{nameof(ClusteredStreamRecord)}] {nameof(Size)}: {Size}, {nameof(StartCluster)}: {StartCluster}, {nameof(Traits)}: {Traits}, {nameof(KeyChecksum)}: {KeyChecksum}, {nameof(Key)}: {Key?.ToHexString(true)}";
	}
	
}
