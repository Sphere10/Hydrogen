// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Application;

public class NamedLookup<T> : INamedLookup<T> {
	private readonly IReadOnlyDictionary<string, Type> _serviceMap;

	private readonly IServiceProvider _serviceProvider;

	public NamedLookup(IServiceProvider serviceProvider) {
		_serviceProvider = serviceProvider;
		if (!NamedLookupInfo.TryGetMap(typeof(T), out var serviceMap))
			throw new ArgumentException($"There is no service map for return type {typeof(T).Name} registered");
		_serviceMap = serviceMap;
	}

	public T this[string name]
		=> _serviceMap.TryGetValue(name, out var serviceType) ? (T)_serviceProvider.GetService(serviceType) : default;
}
