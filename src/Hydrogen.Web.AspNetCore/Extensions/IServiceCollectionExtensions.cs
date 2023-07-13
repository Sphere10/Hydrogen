// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Application;
using Hydrogen.Web.AspNetCore;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions {
	public static IServiceCollection AddHydrogenFramework(this IServiceCollection serviceCollection) {
		HydrogenFramework.Instance.RegisterModules(serviceCollection);
		return serviceCollection;
	}

	public static IServiceCollection AddHydrogenLogger(this IServiceCollection serviceCollection, Hydrogen.ILogger logger) {
		return serviceCollection.AddTransient<ILoggerProvider>(_ => new HydrogenLoggerProvider(logger));
	}

}
