using System;
using System.Diagnostics;

namespace Sphere10.Framework {

	internal class ClusterSerializer : FixedSizeObjectSerializer<Cluster> {
		private readonly int _clusterDataSize;

		public ClusterSerializer(int clusterSize) : base(clusterSize + sizeof(int) + sizeof(int)  + sizeof(int)) {
			_clusterDataSize = clusterSize;
		}

		public override int Serialize(Cluster cluster, EndianBinaryWriter writer) {
			Debug.Assert(cluster.Data.Length == _clusterDataSize);
			
			writer.Write((int)cluster.Traits);
			writer.Write(cluster.Number);
			writer.Write(cluster.Data);
			writer.Write(cluster.Next);
			
			return sizeof(int) + _clusterDataSize + sizeof(int)  + sizeof(int);
		}

		public override Cluster Deserialize(int size, EndianBinaryReader reader) {
			var cluster = new Cluster {
				Traits = (ClusterTraits)reader.ReadInt32(),
				Number = reader.ReadInt32(),
				Data = reader.ReadBytes(_clusterDataSize),
				Next = reader.ReadInt32()
			};
			return cluster;
		}
	}

}
