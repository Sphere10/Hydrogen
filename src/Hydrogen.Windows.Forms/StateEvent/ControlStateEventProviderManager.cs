// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms;

public class ControlStateEventProviderManager {
	private readonly HashSet<Type> _controlStateEventProviders;

	static ControlStateEventProviderManager() {
		Instance = new ControlStateEventProviderManager();
	}

	public ControlStateEventProviderManager() {
		_controlStateEventProviders = new HashSet<Type>();
		Refresh();
	}

	public static ControlStateEventProviderManager Instance { get; }


	public void Refresh() {
		_controlStateEventProviders.Clear();
		if (NamedLookupInfo.TryGetMap(typeof(IControlStateEventProvider), out var serviceMap)) {
			foreach (var eventProvider in serviceMap) {
				var controlType = Tools.Object.ResolveType(eventProvider.Key);
				_controlStateEventProviders.Add(controlType);
			}
		}
	}

	public bool HasControlStateProvider<TControl>() {
		return HasControlStateEventProvider(typeof(TControl));
	}

	public bool HasControlStateEventProvider(Type type)
		=> FindControlStateEventProvider(type, false, out _);

	public bool FindControlStateEventProvider(Type controlType, bool searchSubClasses, out Type implementationForBaseType) {
		var typesToSearch = new[] { controlType };
		if (searchSubClasses)
			typesToSearch = typesToSearch.Concat(controlType.GetAncestorClasses().Where(c => c.IsAssignableTo(typeof(Control)))).ToArray();
		foreach (var type in typesToSearch) {
			if (_controlStateEventProviders.Contains(type)) {
				implementationForBaseType = type;
				return true;
			}
		}
		implementationForBaseType = null;
		return false;
	}

	public IControlStateEventProvider GetControlStateProvider<T>() where T : Control
		=> GetControlStateProvider(typeof(T));

	public IControlStateEventProvider GetControlStateProvider(Type controlType) {
		if (!TryGetControlStateProvider(controlType, out var provider))
			throw new InvalidOperationException($"No {nameof(IControlStateEventProvider)} found for {controlType.Name}");
		return provider;
	}

	public bool TryGetControlStateProvider(Type controlType, out IControlStateEventProvider provider) {

		if (FindControlStateEventProvider(controlType, true, out var actualType)) {
			provider = HydrogenFramework.Instance.ServiceProvider.GetNamedService<IControlStateEventProvider>(actualType.FullName);
			return true;
		}
		provider = null;
		return false;
	}

}
