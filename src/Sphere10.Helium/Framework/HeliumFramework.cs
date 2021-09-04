using System;
using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Helium.HeliumNode;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Framework {
	public class HeliumFramework : IHeliumFramework {
		private IInstantiateHandler _instantiateHandler;

		static HeliumFramework() {
			Instance = new HeliumFramework();
		}

		public static HeliumFramework Instance { get; }

		public EnumModeOfOperationType ModeOfOperation { get; set; }
		public IRouter Router { get; set; }
		public ILogger Logger { get; set; }

		public void StartHeliumFramework(HeliumNodeSettings endPointSettings) {
			Sphere10Framework.Instance.StartFramework();
			
			if (Logger == null) throw new ArgumentNullException($"Logger", "HeliumFramework CANNOT start without a logger.");
			
			Router = ComponentRegistry.Instance.Resolve<IRouter>();
			if (Router == null) throw new ArgumentNullException($"Router", "HeliumFramework CANNOT work without a Router.");
			Router.Logger = Logger;

			_instantiateHandler = ComponentRegistry.Instance.Resolve<IInstantiateHandler>();
			ComponentRegistry.Instance.Resolve<ILocalQueueOutputProcessor>();

			var configureThisEndpoint = ComponentRegistry.Instance.Resolve<IConfigureHeliumNode>();
			configureThisEndpoint.SetupEndpoint(endPointSettings);
			configureThisEndpoint.CheckSettings();
		}

		public void LoadHandlerTypes(IList<PluginAssemblyHandlerDto> handlerTypeList) {
			_instantiateHandler.PluginAssemblyHandlerList = handlerTypeList;
		}
	}
}