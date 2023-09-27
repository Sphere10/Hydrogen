// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.DApp.Presentation2.Logic {

	public interface IPlugin {
		event EventHandlerEx Loaded;
		event EventHandlerEx Unloaded;

		string Name { get; }

		IApplicationBlock[] Blocks { get; }

		IServiceProvider IoCContainer { get; }

		void Load(IServiceCollection secureComponentRegistry);

		void Unload();

	}
}
