using System;
using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Framework {
	public class HeliumFramework : IHeliumFramework {

		static HeliumFramework() {
			Instance = new HeliumFramework();
		}

		public static HeliumFramework Instance { get; }

		public EnumModeOfOperationType ModeOfOperation { get; set; }
		public IRouter Router { get; set; }
		public ILogger Logger { get; set; }

		public void StartHeliumFramework() {
			Sphere10Framework.Instance.StartFramework();

			if (Logger == null) throw new ArgumentNullException($"Logger", "HeliumFramework CANNOT start without a logger.");

			Router = ComponentRegistry.Instance.Resolve<Router.Router>();

			if (Router == null) throw new ArgumentNullException($"Router");

			Router.Logger = Logger;
		}

		public void LoadHandlerTypes(IList<PluginAssemblyHandler> handlerTypeList) {
			var handlerInstigator = ComponentRegistry.Instance.Resolve<InstantiateHandler>();
			handlerInstigator.PluginAssemblyHandlerList = handlerTypeList;
		}
	}
}