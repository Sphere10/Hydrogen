using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework.Application;

namespace Sphere10.Helium.Framework {
	public abstract class PluginModuleConfiguration : ModuleConfigurationBase {
		public override void OnInitialize() {
			base.OnInitialize();

			AutoRegisterHandlers();
		}

		private void AutoRegisterHandlers() {

			var pluginAssembly = this.GetType().Assembly; 
			//inspect the assembly and register all handlers correctly into component registry


			throw new NotImplementedException();
		}
	}
}