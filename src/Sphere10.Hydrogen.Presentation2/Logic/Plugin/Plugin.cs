using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sphere10.Framework;
using Sphere10.Framework.Application;

namespace Sphere10.Hydrogen.Presentation2.Logic {

	public class Plugin : IPlugin {
		public event EventHandlerEx Loaded;
		public event EventHandlerEx Unloaded;

		public Plugin(string name, IEnumerable<IApplicationBlock> blocks) {
			Name = name;
			Blocks = blocks.ToArray();
		}

		public string Name { get; init; }

		public IApplicationBlock[] Blocks { get; init; }

		public ComponentRegistry IoCContainer { get; private set; }

		public virtual void Load() {
			NotifyLoaded();
		}

		public void Load(ComponentRegistry secureComponentRegistry) {
			IoCContainer = secureComponentRegistry;
			NotifyLoaded();
		}

		public virtual void Unload() {
			IoCContainer.Dispose();
			NotifyUnloaded();
		}

		protected virtual void OnLoaded() {
		}

		protected virtual void OnUnloaded() {
		}

		internal void NotifyLoaded() {
			OnLoaded();
			Loaded?.Invoke();
		}

		internal void NotifyUnloaded() {
			OnUnloaded();
			Unloaded?.Invoke();
		}

	}

}