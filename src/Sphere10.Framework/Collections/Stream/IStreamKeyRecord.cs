namespace Sphere10.Framework {
	public interface IStreamKeyRecord: IClusteredRecord {
		public int KeyChecksum { get; set; }
	}
}
