namespace Sphere10.Helium.HeliumNode {
	public interface IConfigureHeliumEndpoint {
		public void SetupEndpoint(HeliumNodeSettings endPointSettings);
		public void CheckSettings();
	}
}