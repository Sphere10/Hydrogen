using System;
using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Framework {
	public class HeliumFramework : IHeliumFramework {
		private ILocalQueueOutputProcessor _outputProcssor;

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

			Router = ComponentRegistry.Instance.Resolve<IRouter>();

			if (Router == null) throw new ArgumentNullException($"Router", "HeliumFramework CANNOT work without a Router.");

			Router.Logger = Logger;

			// Create an instance of the local quue output processor so that it exists, it's constructor
			// will subscribe to input queue committed, and will process event accordingly. 
			_outputProcssor = ComponentRegistry.Instance.Resolve<ILocalQueueOutputProcessor>();

		}

		public void LoadHandlerTypes(IList<PluginAssemblyHandler> handlerTypeList) {
			var handlerInstigator = ComponentRegistry.Instance.Resolve<InstantiateHandler>();
			handlerInstigator.PluginAssemblyHandlerList = handlerTypeList;
		}
	}
}