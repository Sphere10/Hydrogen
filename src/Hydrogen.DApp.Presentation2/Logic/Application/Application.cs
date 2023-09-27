// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Hydrogen.DApp.Presentation2.Logic {

	public abstract class Application : Disposable, IApplication {
		//private static IConfiguration Configuration { get; set; } = null!;

		private readonly ExtendedList<IPlugin> _plugins = new ExtendedList<IPlugin>();

		public event EventHandlerEx Initializing;

		public event EventHandlerEx Initialized;

		public event EventHandlerEx Finishing;

		public IReadOnlyList<IPlugin> LoadedPlugins => _plugins;

		public IApplicationBlock ActiveBlock { get; private set; }

		public IPlugin ActivePlugin { get; internal set; }

		public IApplicationScreen ActiveScreen { get; private set; }

		public async Task Initialize(WebAssemblyHostBuilder hostBuilder) {
			NotifyInitializing();
			hostBuilder.Services.AddSingleton(typeof(IApplication), this);
			//Configuration = hostBuilder.Configuration;
			//var componentRegistry = AdaptBlazorIoCContainerToSphere10Framework(hostBuilder.Services);
			//         RegisterServices(componentRegistry);
			Guard.Ensure(_plugins.Count == 0);
			_plugins.Clear();
			foreach (var pluginType in GetPlugins()) {
				var plugin = Activator.CreateInstance(pluginType) as IPlugin;
				Guard.Ensure(plugin != null, $"'{pluginType.Name}' was not an {nameof(IPlugin)}");
				//var pluginContainer = componentRegistry.CreateChildRegistry();
				plugin.Load(hostBuilder.Services);
				_plugins.Add(plugin);
			}

			Configure(hostBuilder);
			NotifyInitialized();
		}

		public async Task Finish() {
			NotifyFinising();
		}


		protected abstract IEnumerable<Type> GetPlugins();

		//protected virtual void RegisterServices(ComponentRegistry componentRegistry) {
		//}

		protected virtual void Configure(WebAssemblyHostBuilder hostBuilder) {
		}


		protected virtual void OnInitializing() {
		}

		protected virtual void OnInitialized() {
		}


		protected virtual void OnFinishing() {
		}

		//private ComponentRegistry AdaptBlazorIoCContainerToSphere10Framework(IServiceCollection serviceCollection) {
		//	///
		//	///  TODO
		//	///
		//	return ComponentRegistry.Instance.CreateChildRegistry(); 
		//}

		private void NotifyInitializing() {
			OnInitializing();
			Initializing?.Invoke();
		}

		private void NotifyInitialized() {
			OnInitialized();
			Initialized?.Invoke();
		}


		private void NotifyFinising() {
			OnFinishing();
			Finishing?.Invoke();
		}

	}
}
