using System.Diagnostics;

namespace Sphere10.Framework.Collections {

	public class ClusterSerializer : FixedSizeObjectSerializer<Cluster> {
		private readonly int _clusterDataSize;

		public ClusterSerializer(int clusterSize) : base(clusterSize + sizeof(int) + sizeof(int)) {
			_clusterDataSize = clusterSize;
		}

		public override int Serialize(Cluster cluster, EndianBinaryWriter writer) {
			Debug.Assert(cluster.Data.Length == _clusterDataSize);
			writer.Write(cluster.Number);
			writer.Write(cluster.Data);
			writer.Write(cluster.Next);
			return sizeof(int) + _clusterDataSize + sizeof(int);
		}

		public override Cluster Deserialize(int size, EndianBinaryReader reader) {
			var sector = new Cluster {
				Number = reader.ReadInt32(),
				Data = reader.ReadBytes(_clusterDataSize),
				Next = reader.ReadInt32()
			};
			return sector;
		}
	}

}
