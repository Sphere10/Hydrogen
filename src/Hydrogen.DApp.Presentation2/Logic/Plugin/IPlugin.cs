using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Hydrogen;
using Hydrogen.Application;

namespace Hydrogen.DApp.Presentation2.Logic {

	public interface IPlugin {
		event EventHandlerEx Loaded;
		event EventHandlerEx Unloaded;

		string Name { get; }

		IApplicationBlock[] Blocks { get; }

		ComponentRegistry IoCContainer { get; }

		void Load(ComponentRegistry secureComponentRegistry);
		
		void Unload();

	}
}