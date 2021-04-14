namespace Sphere10.Helium.Bus {
	public interface IConfigurationSource {
		T GetConfiguration<T>() where T : class, new();
	}
}
