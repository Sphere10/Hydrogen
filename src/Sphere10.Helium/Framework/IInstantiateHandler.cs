using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Framework {
	public interface IInstantiateHandler {
		public IList<PluginAssemblyHandlerDto> PluginAssemblyHandlerList { get; set; }

		public void HandleMessage(IMessage message);
	}
}