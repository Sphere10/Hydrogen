using Sphere10.Helium.PluginFramework;

namespace Sphere10.Helium.Loader {

	/// <summary>
	/// IMPORTANT: This simulates is the Sphere10 node.
	/// This Project will be deleted once Helium is integrated into Hydrogen.
	/// </summary>
	public class Program {
		public static Route.IRouter Router;

		public static void Main(string[] args) {
			var heliumPluginLoader = new HeliumPluginLoader();

			var heliumFramework = heliumPluginLoader.GetHeliumFramework();
			heliumFramework.StartHeliumFramework();

			Router = heliumFramework.Router;

			heliumPluginLoader.LoadPlugins(GetRelativeAssemblyPathNameList());
		}

		private static string[] GetRelativeAssemblyPathNameList() {
			return new[] {
					@"Sphere10.Helium.BlueService\bin\Debug\net5.0\Sphere10.Helium.BlueService.dll",
					@"Sphere10.Helium.Usage\bin\Debug\net5.0\Sphere10.Helium.Usage.dll"
			};
		}
	}
}