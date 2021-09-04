namespace Sphere10.Helium.HeliumNode {
	public interface IConfigureHeliumNode {
		public void SetupEndpoint(HeliumNodeSettings endPointSettings);
		public void CheckSettings();
	}
}