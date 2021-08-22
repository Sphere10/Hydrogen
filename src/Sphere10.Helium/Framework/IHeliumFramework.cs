using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Framework {
	public interface IHeliumFramework {
		EnumModeOfOperationType ModeOfOperation { get; set; }
		IRouter Router { get; set; }
		ILogger Logger { get; set; }
		public LocalQueueSettings QueueSettings { get; set; }

		void StartHeliumFramework();

		void LoadHandlerTypes(IList<PluginAssemblyHandler> handlerTypeList);
	}
}
