//-----------------------------------------------------------------------
// <copyright file="ControlStateManager.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Application;

namespace Hydrogen.Windows.Forms {

    public class ControlStateEventProviderManager {
        private readonly HashSet<Type> _controlStateEventProviders;
        private int _lastKnownComponentRegistryState;

        static ControlStateEventProviderManager() {
            Instance = new ControlStateEventProviderManager();
        }

        public ControlStateEventProviderManager() {
            _controlStateEventProviders = new HashSet<Type>();
            _lastKnownComponentRegistryState = -1;
        }

        public static ControlStateEventProviderManager Instance { get; }


        public void Refresh() {
            _controlStateEventProviders.Clear();
            foreach (var registration in ComponentRegistry.Instance.Registrations.Where(r => r.InterfaceType == typeof(IControlStateEventProvider))) {
                _controlStateEventProviders.Add(TypeResolver.Resolve(registration.Name));
            }
            _lastKnownComponentRegistryState = ComponentRegistry.Instance.State;
        }

        public bool HasControlStateProvider<TControl>() {
            if (_lastKnownComponentRegistryState != ComponentRegistry.Instance.State)
                Refresh();
            return HasControlStateEventProvider(typeof(TControl));
        }

        public bool HasControlStateEventProvider(Type type)
	        => FindControlStateEventProvider(type, false, out _);

        public bool FindControlStateEventProvider(Type controlType, bool searchSubClasses, out Type implementationForBaseType) {
	        if (_lastKnownComponentRegistryState != ComponentRegistry.Instance.State)
		        Refresh();

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
            if (_lastKnownComponentRegistryState != ComponentRegistry.Instance.State)
                Refresh();

            if (FindControlStateEventProvider(controlType, true, out var actualType)) {
	            provider = ComponentRegistry.Instance.Resolve<IControlStateEventProvider>(actualType.FullName);
                return true;
            }
            provider = null;
            return false;
        }

    }
}
