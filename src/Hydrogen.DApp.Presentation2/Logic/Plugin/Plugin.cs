// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hydrogen.DApp.Presentation2.Logic {

	public class Plugin : IPlugin {
		public event EventHandlerEx Loaded;
		public event EventHandlerEx Unloaded;

		public Plugin(string name, IEnumerable<IApplicationBlock> blocks) {
			Name = name;
			Blocks = blocks.ToArray();
		}

		public string Name { get; init; }

		public IApplicationBlock[] Blocks { get; init; }

		public IServiceProvider IoCContainer { get; private set; }

		public virtual void Load() {
			NotifyLoaded();
		}

		public void Load(IServiceCollection serviceCollection) {
			//IoCContainer = secureComponentRegistry;
			NotifyLoaded();
		}

		public virtual void Unload() {
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
