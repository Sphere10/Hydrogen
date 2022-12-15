using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Application;

internal static class NamedLookupInfo {
	private static readonly Dictionary<Type, Dictionary<string, Type>> ServiceMaps = new();

	public static void RegisterMap(Type serviceType, Dictionary<string, Type> serviceMap) {
		ServiceMaps.Add(serviceType, serviceMap);
	}

	public static bool TryGetMap(Type serviceType, out Dictionary<string, Type> serviceMap) {
		return ServiceMaps.TryGetValue(serviceType, out serviceMap);
	}
	
	public static Dictionary<string, Type> GetMap(Type serviceType) {
		if (!TryGetMap(serviceType, out var serviceMap)) 
			throw new InvalidOperationException($"There is no service map for return type {serviceType.Name} registered");
		return serviceMap;
	}
}
