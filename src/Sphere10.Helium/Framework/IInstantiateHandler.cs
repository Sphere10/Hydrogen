using System.Collections.Generic;

namespace Sphere10.Helium.Framework {
	public interface IInstantiateHandler {
		public IList<PluginAssemblyHandlerDto> PluginAssemblyHandlerList { get; set; }
	}
}