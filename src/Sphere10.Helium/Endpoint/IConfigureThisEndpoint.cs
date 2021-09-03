namespace Sphere10.Helium.Endpoint {
	public interface IConfigureThisEndpoint {
		public void SetupEndpoint(EndPointSettings endPointSettings);
		public void CheckSettings();
	}
}