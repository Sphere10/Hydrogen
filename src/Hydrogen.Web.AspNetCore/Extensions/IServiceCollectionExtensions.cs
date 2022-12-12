using System;
using System.Collections.Generic;
using System.Linq;
using Hydrogen;
using Hydrogen.Application;
using Hydrogen.Web.AspNetCore;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions {
	private static readonly HashSet<IServiceCollection> _alreadyAdded = new();
	public static IServiceCollection AddHydrogenFramework(this IServiceCollection serviceCollection) {
		Guard.Against(_alreadyAdded.Contains(serviceCollection), "Hydrogen framework has already been added");
		_alreadyAdded.Add(serviceCollection);

		HydrogenFramework.Instance.RegisterAppConfig();
		HydrogenFramework.Instance.RegisterAllModuleComponents();

		var hydrogenRegistrations = ComponentRegistry.Instance.Registrations.ToArray();

		// Add all service registrations from Microsoft's IoC container to Hydrogen's ComponentRegistry IoC container
		foreach(var service in serviceCollection) {
			var activationType = service.Lifetime switch {
				ServiceLifetime.Singleton => ActivationType.Singleton,
				ServiceLifetime.Scoped => ActivationType.PerRequest,
				ServiceLifetime.Transient => ActivationType.Instance,
				_ => throw new NotSupportedException(service.Lifetime.ToString())
			};

			// Don't support PerRequest/Scoped cross-linking yet.
			if (activationType == ActivationType.PerRequest)
				continue;

			ComponentRegistry.Instance.RegisterComponentFactory(
				service.ServiceType,
				(_, overload) => {
					// In this factory we presume that the services container has been constructed. It's important
					// StartHydrogenFramework is called on host before running. 
					var servicesContainer = HydrogenFramework.Instance.GetAspNetCoreHost().Services;
					var requestedType = (Type)overload["__requestedType"];
					var obj =  servicesContainer.GetService(requestedType);
					return obj;
				},
				activationType
				);
		}

		// Add all service registrations from Hydrogen into Microsoft's IoC
		foreach(var componentRegistration in hydrogenRegistrations) {
			var serviceLifetime = componentRegistration.ActivationType switch {
				ActivationType.Instance => ServiceLifetime.Transient,
				ActivationType.Singleton => ServiceLifetime.Singleton,
				ActivationType.PerRequest => ServiceLifetime.Scoped,
				_ => throw new NotSupportedException(componentRegistration.ActivationType.ToString())
			};
	
			var descriptor = ServiceDescriptor.Describe(
				componentRegistration.InterfaceType,
				_ => ComponentRegistry.Instance.Resolve(componentRegistration.InterfaceType),
				serviceLifetime
			);
			serviceCollection.Add(descriptor);
		}
		return serviceCollection;
	}

	public static IServiceCollection AddHydrogenLogger(this IServiceCollection serviceCollection, Hydrogen.ILogger logger) {
		return serviceCollection.AddSingleton<ILoggerProvider>( _ => new HydrogenLoggingProvider(logger));
	}
}
