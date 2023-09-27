// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Application;

public class ModuleConfiguration : ModuleConfigurationBase {
	
	public override void RegisterComponents(IServiceCollection serviceCollection) {

		if (HydrogenFramework.Instance.Options.HasFlag(HydrogenFrameworkOptions.EnsureSystemDataDirGloballyAccessible)) {
			serviceCollection.AddInitializer<EnsureSystemDataDirGloballyAccessibleInitializer>();
		}
	}

}
