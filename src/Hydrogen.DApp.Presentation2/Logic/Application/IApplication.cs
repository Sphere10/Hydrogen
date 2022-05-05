using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Hydrogen;

namespace Hydrogen.DApp.Presentation2.Logic {

	public interface IApplication : IDisposable {
		event EventHandlerEx Initializing;
		event EventHandlerEx Initialized;
		event EventHandlerEx Finishing;
		IReadOnlyList<IPlugin> LoadedPlugins { get; }
		IApplicationBlock ActiveBlock { get; }
		IPlugin ActivePlugin { get; }
		IApplicationScreen ActiveScreen { get;  }
		Task Initialize(WebAssemblyHostBuilder hostBuilder);
		Task Finish();
	}

}
