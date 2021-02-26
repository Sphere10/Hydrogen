using System.Diagnostics;

namespace Sphere10.Framework.Collections {

	public class SectorSerialier : ConstantObjectSerializer<Sector> {
		private readonly int _sectorDataSize;
		
		public SectorSerialier(int sectorSize) : base(sectorSize + sizeof(int)) {
			_sectorDataSize = sectorSize;
		}

		public override int Serialize(Sector sector, EndianBinaryWriter writer) {
			Debug.Assert(sector.Data.Length == _sectorDataSize);
			writer.Write(sector.Number);
			writer.Write(sector.Data);
			writer.Write(sector.Next);
			return sizeof(int) + _sectorDataSize + sizeof(int);
		}

		public override Sector Deserialize(int size, EndianBinaryReader reader) {
			var sector = new Sector();
			sector.Number = reader.ReadInt32();
			sector.Data = reader.ReadBytes(_sectorDataSize);
			sector.Next = reader.ReadInt32();
			return sector;
		}
	}

}
