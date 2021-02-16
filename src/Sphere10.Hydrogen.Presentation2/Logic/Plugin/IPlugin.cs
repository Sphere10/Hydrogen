using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Sphere10.Framework;
using Sphere10.Framework.Application;

namespace Sphere10.Hydrogen.Presentation2.Logic {

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