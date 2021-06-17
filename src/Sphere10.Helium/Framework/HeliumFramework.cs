using System;
using System.Collections.Generic;
using Sphere10.Framework.Application;
using Sphere10.Helium.Route;

namespace Sphere10.Helium.Framework {
	public class HeliumFramework {

		static HeliumFramework() {
			Instance = new HeliumFramework();
		}

		public static HeliumFramework Instance { get; }

		public IRouter Router { get; set; }

		public IList<PluginAssemblyHandler> PluginAssemblyHandlerList { get; set; }

		public void StartHeliumFramework() {
			var heliumAssembly = typeof(ModuleConfiguration).Assembly;
			var moduleConfiguration = (ModuleConfiguration)heliumAssembly.CreateInstance("Sphere10.Helium.ModuleConfiguration");

			if (moduleConfiguration == null)
				throw new ArgumentNullException($"moduleConfiguration");

			moduleConfiguration.RegisterComponents(ComponentRegistry.Instance);

			Router = ComponentRegistry.Instance.Resolve<Route.Router>();

			if (Router == null)
				throw new ArgumentNullException($"Router");
		}
	}
}