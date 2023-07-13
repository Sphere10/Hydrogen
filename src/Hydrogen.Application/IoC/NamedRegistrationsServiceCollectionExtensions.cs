// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using Hydrogen;
using Hydrogen.Application;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionNamedRegistrationsExtensions {

	public static bool HasNamedImplementationFor<TService>(this IServiceCollection services, string name) {
		if (!NamedLookupInfo.TryGetMap(typeof(TService), out var serviceMap))
			return false;
		return serviceMap.ContainsKey(name);
	}

	public static void AddNamedTransient<TService, TImpl>(this IServiceCollection services, string name)
		=> services.AddNamed(ServiceDescriptor.Transient(typeof(TService), typeof(TImpl)), name);


	public static void AddNamedSingleton<TService, TImpl>(this IServiceCollection services, string name)
		=> services.AddNamed(ServiceDescriptor.Singleton(typeof(TService), typeof(TImpl)), name);

	public static void AddNamedSingleton<TService>(this IServiceCollection services, TService instance, string name) where TService : class
		=> services.AddNamed(ServiceDescriptor.Singleton(instance), name);


	public static void AddNamed(this IServiceCollection services, ServiceDescriptor item, string name) {
		Guard.Against(item.ImplementationType == null && item.ImplementationInstance == null, "Cannot register named implementation factories");
		Guard.Ensure(item.ServiceType != item.ImplementationType, "Cannot register named concrete types"); // can't have named concrete types

		var implementationType = item.ImplementationType ?? item.ImplementationInstance.GetType();

		// Find the NamedLookup info for type
		if (!NamedLookupInfo.TryGetMap(item.ServiceType, out var serviceMap)) {
			// First named lookup for this type, so setup the info and register the INamedLookup'
			serviceMap = new Dictionary<string, Type>();
			NamedLookupInfo.RegisterMap(item.ServiceType, serviceMap);

			// Register INamedLookup<TService> -> NamedLookup<TService>
			var namedLookupInterfaceType = typeof(INamedLookup<>).MakeGenericType(item.ServiceType);
			var namedLookupConcreteType = typeof(NamedLookup<>).MakeGenericType(item.ServiceType);
			services.Add(ServiceDescriptor.Singleton(namedLookupInterfaceType, namedLookupConcreteType));
		}

		// Register the concrete type (this is used to construct the explicit 
		if (item.ImplementationInstance != null)
			services.Add(ServiceDescriptor.Singleton(implementationType, item.ImplementationInstance));
		else
			services.Add(ServiceDescriptor.Describe(item.ImplementationType, implementationType, item.Lifetime));

		// Add the service type as a singleton factory to the concrete type
		services.Add(ServiceDescriptor.Singleton(item.ServiceType, provider => provider.GetService(implementationType)));

		// Remember the type & name to implementation type in service map
		serviceMap.Add(name, implementationType);
	}


}
