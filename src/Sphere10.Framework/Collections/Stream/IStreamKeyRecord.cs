namespace Sphere10.Framework {
	public interface IStreamKeyRecord: IStreamRecord {
		public int KeyChecksum { get; set; }
	}
}
