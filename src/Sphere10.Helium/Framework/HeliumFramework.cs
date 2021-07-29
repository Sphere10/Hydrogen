using System;
using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Framework {
	public class HeliumFramework {

		static HeliumFramework() {
			Instance = new HeliumFramework();
		}

		public static HeliumFramework Instance { get; }
		public EnumModeOfOperationType ModeOfOperation { get; set; }
		public IRouter Router { get; set; }
		public ILogger Logger { get; set; }

		public void StartHeliumFramework() {
			var heliumAssembly = typeof(ModuleConfiguration).Assembly;
			var moduleConfiguration = (ModuleConfiguration)heliumAssembly.CreateInstance("Sphere10.Helium.ModuleConfiguration");

			if (moduleConfiguration == null)
				throw new ArgumentNullException($"moduleConfiguration");

			moduleConfiguration.RegisterComponents(ComponentRegistry.Instance);

			Router = ComponentRegistry.Instance.Resolve<Router.Router>();

			if (Router == null)
				throw new ArgumentNullException($"Router");
		}

		public void LoadHandlerTypes(IList<PluginAssemblyHandler> handlerTypeList) {
			var handlerInstigator = ComponentRegistry.Instance.Resolve<InstantiateHandler>();
			handlerInstigator.PluginAssemblyHandlerList = handlerTypeList;
		}
	}
}