using Sphere10.Framework;
using Sphere10.Helium.Framework;
using Sphere10.Helium.PluginFramework;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Loader {

	/// <summary>
	/// IMPORTANT: This simulates the Sphere10 node and shows how the HeliumFrame is used.
	/// This Project will be deleted once Helium is integrated into Hydrogen.
	/// </summary>
	public class Program {
		private static IRouter _router;

		public static void Main(string[] args) {

			var logger = new ConsoleLogger();

			IHeliumPluginLoader heliumPluginLoader = new HeliumPluginLoader(logger);
			heliumPluginLoader.LoadPlugins(GetPluginRelativePathNameList());

			var heliumFramework = heliumPluginLoader.GetHeliumFramework();
			heliumFramework.ModeOfOperation = EnumModeOfOperationType.HydrogenMode;
			heliumFramework.StartHeliumFramework();
			_router = heliumFramework.Router;

			var x = heliumPluginLoader.GetEnabledPlugins();

			//SimulateMessagesBeingSentToThisNode();
		}

		private static string[] GetPluginRelativePathNameList() {
			return new[] {
					@"Sphere10.Helium.BlueService\bin\Debug\net5.0\Sphere10.Helium.BlueService.dll",
					@"Sphere10.Helium.Usage\bin\Debug\net5.0\Sphere10.Helium.Usage.dll",
					@"Sphere10.Helium.TestPlugin1\bin\Debug\net5.0\Sphere10.Helium.TestPlugin1.dll",
					@"Sphere10.Helium.TestPlugin2\bin\Debug\net5.0\Sphere10.Helium.TestPlugin2.dll",
					@"Sphere10.Helium.TestPlugin3\bin\Debug\net5.0\Sphere10.Helium.TestPlugin3.dll"
			};
		}

		//private static void SimulateMessagesBeingSentToThisNode() {
		//	Test_SendTestMessage1ToRouter();
		//	Test_SendTestMessage2ToRouter();
		//	Test_SendTestMessage3ToRouter();
		//	Test_SendTestMessage4ToRouter();
		//	Test_SendTestMessage5ToRouter();
		//}

		private static void Test_SendTestMessage1ToRouter() {
			//_router.InputMessage()
		}

		private static void Test_SendTestMessage2ToRouter() {
			//_router.InputMessage()
		}

		private static void Test_SendTestMessage3ToRouter() {
			//_router.InputMessage()
		}

		private static void Test_SendTestMessage4ToRouter() {
			//_router.InputMessage()
		}

		private static void Test_SendTestMessage5ToRouter() {
			//_router.InputMessage()
		}
	}
}