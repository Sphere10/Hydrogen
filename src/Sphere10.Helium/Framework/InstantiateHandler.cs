using System;
using System.Collections.Generic;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Framework {
	public class InstantiateHandler : IInstantiateHandler {

		public IList<PluginAssemblyHandler> PluginAssemblyHandlerList { get; set; }

		public void HandleMessage(IMessage message) {
			throw new NotImplementedException();
		}
	}
}
