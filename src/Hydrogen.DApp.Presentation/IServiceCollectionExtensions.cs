// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.DApp.Presentation;

/// <summary>
/// Service collection extension methods
/// </summary>
// ReSharper disable once InconsistentNaming -- extension methods on interface.
public static class IServiceCollectionExtensions {
	/// <summary>
	/// Locates view model classes in the given assembly and adds them to the service collection.
	/// Naive name-based location strategy -- classes with names ending in 'ViewModel' will be added
	/// as transient services.
	/// </summary>
	/// <param name="serviceCollection"></param>
	/// <param name="assembly"></param>
	/// <returns> service collection with new service descriptors added</returns>
	public static IServiceCollection AddViewModelsFromAssembly(this IServiceCollection serviceCollection,
	                                                           Assembly assembly) {
		var vms = assembly.ExportedTypes
			.Where(x => x.Name.Contains("ViewModel", StringComparison.OrdinalIgnoreCase))
			.Where(x => !x.IsAbstract && !x.IsInterface);

		foreach (Type type in vms) {
			serviceCollection.Add(ServiceDescriptor.Transient(type, type));
		}

		return serviceCollection;
	}
}
