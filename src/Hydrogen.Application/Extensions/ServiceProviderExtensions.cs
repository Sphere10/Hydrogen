// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Application;

public static class ServiceProviderExtensions {
	public static bool TryGetService<T>(this IServiceProvider serviceProvider, out T impl) {
		var impls = serviceProvider.GetServices<T>().ToArray();
		impl = impls.LastOrDefault();
		return impls.Any();
	}

	public static T GetServiceOrThrow<T>(this IServiceProvider serviceProvider) {
		if (!serviceProvider.TryGetService(out T impl)) 
			throw new InvalidOperationException($"No service of type {typeof(T).Name} was found.");
		return impl;
	}
}
