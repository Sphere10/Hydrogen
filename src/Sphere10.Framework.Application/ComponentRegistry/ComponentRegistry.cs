//-----------------------------------------------------------------------
// <copyright file="ComponentRegistry.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Xml.Serialization;
using Sphere10.Framework;
using TinyIoC;

namespace Sphere10.Framework.Application {

	/// <summary>
	/// Sphere 10 one stop shop for all IoC and Service Locator functionality.
	/// <remarks>
	/// Under root configuration element add
	///   &lt;configSections&gt;
	///    &lt;section name = "ComponentRegistry" type ="Sphere10.Framework.GenericSectionHandler, Sphere10.Framework"/&gt;
	///  &lt;/configSections&gt;
	/// 
	/// In Program.cs or Web startup, use
	///  ...
	///  ComponentRegistry.Instance.RegisterAppConfig();
	/// 
	/// if using application framework, this this is called automatically on start
	/// </remarks>
	/// </summary>
	[XmlRoot]
    public class ComponentRegistry : IDisposable {
        private readonly object _threadLock;
        private readonly TinyIoCContainer _tinyIoCContainer;
        private readonly IList<Registration> _registrations;
        private readonly LookupEx<Type, Registration> _registrationsByInterfaceType;
        private readonly LookupEx<Type, Registration> _registrationsByImplementationType;
		private readonly ComponentRegistry _parent;

        static ComponentRegistry() {
            Instance = new ComponentRegistry(TinyIoCContainer.Current);
        }

        private ComponentRegistry(TinyIoCContainer tinyIoCContainer) {
            _threadLock = new object();
            _tinyIoCContainer = tinyIoCContainer;
            _registrations = new List<Registration>();
            _registrationsByInterfaceType = new LookupEx<Type, Registration>();
            _registrationsByImplementationType = new LookupEx<Type, Registration>();
            State = 0;
            _parent = null;
        }

        private ComponentRegistry(ComponentRegistry parent) 
            : this(parent._tinyIoCContainer.GetChildContainer()) {
            //_threadLock = new object();
            //_tinyIoCContainer = parent._tinyIoCContainer.GetChildContainer();
            //_registrations = new List<Registration>(parent._registrations);
            //_registrationsByInterfaceType = new LookupEx<Type, Registration>(parent._registrationsByInterfaceType);
            //_registrationsByImplementationType = new LookupEx<Type, Registration>(parent._registrationsByImplementationType);
            //State = 0;
            _parent = parent;
        }

        public static ComponentRegistry Instance { get; }

        
        public IEnumerable<Registration> Registrations => (_parent?.Registrations ?? Enumerable.Empty<Registration>()).Concat(_registrations);

        public int State { get; private set; }

        #region Public Methods

        public ComponentRegistry CreateChildRegistry() {
            return new ComponentRegistry(this);
        }
		
        public void RegisterDefinition(ComponentRegistryDefinition componentRegistryDefinition) {
            if (componentRegistryDefinition.RegistrationsDefinition == null)
                return;
            foreach (var registration in componentRegistryDefinition.RegistrationsDefinition) {
                TypeSwitch.For(registration,
                    TypeSwitch.Case<ComponentRegistryDefinition.AssemblyRegistrationDefinition>(assemblyRegistration =>
                        RegisterInternalAssemblyRegistration(
                            componentRegistryDefinition,
                            assemblyRegistration
                        )
                    ),
                    TypeSwitch.Case<ComponentRegistryDefinition.ComponentRegistrationDefinition>(componentRegistration =>
                        RegisterInternalComponentRegistration(
                            componentRegistryDefinition,
                            componentRegistration
                        )
                    ),
                    TypeSwitch.Case<ComponentRegistryDefinition.ProxyInterfaceRegistrationDefinition>(proxyRegistration =>
                        RegisterInternalProxyRegistration(
                            componentRegistryDefinition,
                            proxyRegistration
                        )
                    ),
                    TypeSwitch.Case<ComponentRegistryDefinition.ComponentSetRegistrationDefinition>(multipleComponentsRegistration =>
                        RegisterInternalMultipleComponentsRegistration(
                            componentRegistryDefinition,
                            multipleComponentsRegistration
                        )
                    ),
                    TypeSwitch.Default(() => { throw new NotSupportedException(registration.GetType().FullName); })
                );
            }
        }

        public void RegisterComponentInstance<TInterface>(TInterface instance, string name = null)
            where TInterface : class {
            lock (_threadLock) {
                _tinyIoCContainer.Register(instance, name ?? string.Empty);
                RegisterInternal(Registration.From(instance, name ?? string.Empty));
            }
        }

	    public void RegisterComponent<TInterface, TImplementation>(ActivationType activation) 
			where TInterface : class
			where TImplementation : class, TInterface {
			RegisterComponent<TInterface, TImplementation>(null, activation);
	    }

		public void RegisterComponent<TInterface, TImplementation>(string resolveKey = null, ActivationType activation = ActivationType.Instance)
            where TInterface : class
            where TImplementation : class, TInterface {
            lock (_threadLock) {
				var tinyOptions = _tinyIoCContainer.Register<TInterface, TImplementation>(resolveKey ?? string.Empty);
				switch (activation) {
					case ActivationType.Instance:
						tinyOptions.AsMultiInstance();
						break;
					case ActivationType.Singleton:
						tinyOptions.AsSingleton();
						break;
					case ActivationType.PerRequest:
						tinyOptions.AsPerRequestSingleton();
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(activation), activation, null);
				}
				RegisterInternal(Registration.From<TInterface, TImplementation>(resolveKey ?? string.Empty, activation));
            }
        }

        public void RegisterComponent(Type interfaceType, Type componentType, string resolveKey = null, ActivationType options = ActivationType.Instance) {
            lock (_threadLock) {
                var tinyOptions = _tinyIoCContainer.Register(interfaceType, componentType, resolveKey ?? string.Empty);
                switch (options) {
                    case ActivationType.Instance:
                        tinyOptions.AsMultiInstance();
                        break;
                    case ActivationType.Singleton:
                        tinyOptions.AsSingleton();
                        break;
                    case ActivationType.PerRequest:
                        tinyOptions.AsPerRequestSingleton();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(options), options, null);
                }
                RegisterInternal(Registration.From(interfaceType, componentType, resolveKey ?? string.Empty, options));
            }
        }

		public void DeregisterComponent<TImplementation>(string resolveKey = null)
			=> DeregisterComponent(typeof(TImplementation), resolveKey);

		public void DeregisterComponent(Type type, string resolveKey = null) {
			lock (_threadLock) {
				_tinyIoCContainer.Unregister(type, resolveKey);
				var registration = _registrations.SingleOrDefault(r => r.ImplementationType == type);
				if (registration != null)
					DeregisterInternal(registration);
			}
		}

        public void RegisterProxyComponent<TInterface, TProxy>(string resolveKey = null) 
            where TInterface : class 
            where TProxy : class {
            lock (_threadLock) {
                _tinyIoCContainer.Register((container, overloads) => container.Resolve<TProxy>(overloads) as TInterface, resolveKey ?? string.Empty);
                RegisterInternal(Registration.FromProxy<TInterface, TProxy>(resolveKey ?? string.Empty));
            }
        }

        public void RegisterProxyComponent(Type interfaceType, Type proxyType, string resolveKey = null) {
            lock (_threadLock) {
                if (!proxyType.IsAssignableFrom(proxyType))
                    throw new SoftwareException("Unable to register proxy component '{0}' as it is not a sub-type of '{1}'", proxyType.FullName, interfaceType.FullName);
                _tinyIoCContainer.Register(interfaceType, (container, overloads) => container.Resolve(proxyType, overloads), resolveKey ?? string.Empty);
                RegisterInternal(Registration.FromProxy(interfaceType, proxyType, resolveKey ?? string.Empty));

            }
        }

        public void RegisterComponentFactory<TInterface>(Func<ComponentRegistry, TInterface> factory) {
	        _tinyIoCContainer.Register(typeof(TInterface), (container, overloads) => factory(this));
        }

		public TInterface Resolve<TInterface>(string resolveKey = null) where TInterface : class {
            var resolvedImplementation = _tinyIoCContainer.Resolve<TInterface>(resolveKey ?? string.Empty);
            //if (!TryResolve(out resolvedImplementation, name)) {
            //    throw new SoftwareException(
            //        "No component has been registered for service type '{0}' with name {1}",
            //        typeof (I).Name,
            //        string.IsNullOrEmpty(name) ? "(none)" : ("'" + name + "'")
            //        );
            //}
            return resolvedImplementation;
        }

        public object Resolve(Type type, string resolveKey = null)  {
            var resolvedImplementation = _tinyIoCContainer.Resolve(type, resolveKey ?? string.Empty);
            return resolvedImplementation;
        }

        public bool TryResolve<TInterface>(out TInterface component, string resolveKey = null) where TInterface : class {
            return _tinyIoCContainer.TryResolve(resolveKey ?? string.Empty, out component);
        }

        public IEnumerable<TInterface> ResolveAll<TInterface>() where TInterface : class {
            return _tinyIoCContainer.ResolveAll<TInterface>();
        }

        public IEnumerable<object> ResolveAll(Type type) {
            return _tinyIoCContainer.ResolveAll(type, true);
        }

        public bool HasImplementation<TImplementation>(string resolveKey = null) {
            return HasImplementation(typeof(TImplementation), resolveKey);
        }

        public bool HasImplementation(Type type, string resolveKey = null) {
			return 
                (_parent?.HasImplementation(type, resolveKey) ?? false) ||
                (!string.IsNullOrWhiteSpace(resolveKey)
				? _registrationsByImplementationType[type].Any(r => r.ResolveKey == resolveKey)
				: _registrationsByImplementationType.CountForKey(type) > 0);
		}

        public bool HasImplementationFor<TInterface>(string resolveKey = null) {
            return HasImplementationFor(typeof(TInterface), resolveKey);
        }

        public bool HasImplementationFor(Type type, string resolveKey = null) {
			return 
                (_parent?.HasImplementationFor(type, resolveKey) ?? false) ||
                (!string.IsNullOrWhiteSpace(resolveKey)
				? _registrationsByInterfaceType[type].Any(r => r.ResolveKey == resolveKey)
				: _registrationsByInterfaceType.CountForKey(type) > 0);
		}

        public void Dispose() {
            _tinyIoCContainer.Dispose();
            State = -1;
            this._registrations.Clear();
            this._registrationsByImplementationType.Clear();
            this._registrationsByInterfaceType.Clear();
        }

        #endregion

        #region Private Methods

        private void RegisterInternalAssemblyRegistration(ComponentRegistryDefinition componentRegistryDefinition, ComponentRegistryDefinition.AssemblyRegistrationDefinition assemblyRegistrationDefinition) {
            TypeResolver.LoadAssembly(assemblyRegistrationDefinition.Dll, componentRegistryDefinition.PluginFolder);
        }

        private void RegisterInternalComponentRegistration( ComponentRegistryDefinition componentRegistryDefinition, ComponentRegistryDefinition.ComponentRegistrationDefinition componentRegistrationDefinition) {
			if (string.IsNullOrWhiteSpace(componentRegistrationDefinition.Implementation))
				throw new ArgumentException("No implementation field provided in component registry definition");
            var interfaceType = TypeResolver.Resolve(componentRegistrationDefinition.Interface, assemblyHint: componentRegistrationDefinition.Dll);
            var implementationType = TypeResolver.Resolve(componentRegistrationDefinition.Implementation, componentRegistrationDefinition.Dll, componentRegistryDefinition.PluginFolder);
            var activation = componentRegistrationDefinition.Activation;

            RegisterComponent(
                interfaceType,
                implementationType,
                componentRegistrationDefinition.ResolveKey,
                activation ?? ActivationType.Instance
            );
        }

        private void RegisterInternalProxyRegistration(ComponentRegistryDefinition componentRegistryDefinition, ComponentRegistryDefinition.ProxyInterfaceRegistrationDefinition proxyInterfaceRegistrationDefinition) {
            var interfaceType = TypeResolver.Resolve(proxyInterfaceRegistrationDefinition.Interface);
            var implementationType = TypeResolver.Resolve(proxyInterfaceRegistrationDefinition.Proxy);

            RegisterProxyComponent(
                interfaceType,
                implementationType
            );
        }

        private void RegisterInternalMultipleComponentsRegistration(ComponentRegistryDefinition componentRegistryDefinition, ComponentRegistryDefinition.ComponentSetRegistrationDefinition componentSetRegistrationDefinition) {
            if (componentSetRegistrationDefinition.RegistrationsDefinition == null)
                return;

            foreach (var registration in componentSetRegistrationDefinition.RegistrationsDefinition) {
                TypeSwitch.For(registration,
                    TypeSwitch.Case<ComponentRegistryDefinition.ComponentRegistrationDefinition>(componentRegistration => {
                        componentRegistration.Interface = componentSetRegistrationDefinition.Interface;
                        RegisterInternalMutipleComponentComponentRegistration(
                            componentRegistryDefinition,
                            componentRegistration
                        );
                    }),
                    TypeSwitch.Case<ComponentRegistryDefinition.ProxyInterfaceRegistrationDefinition>(proxyRegistration => {
                        proxyRegistration.Interface = componentSetRegistrationDefinition.Interface;
                        RegisterInternalMultipleComponentProxyRegistration(
                            componentRegistryDefinition,
                            proxyRegistration
                        );
                    }),
                    TypeSwitch.Default(() => throw new NotSupportedException(registration.GetType().FullName))
                );

            }
        }

        private void RegisterInternalMutipleComponentComponentRegistration(ComponentRegistryDefinition componentRegistryDefinition, ComponentRegistryDefinition.ComponentRegistrationDefinition componentRegistrationDefinition) {
            var interfaceType = TypeResolver.Resolve(componentRegistrationDefinition.Interface, assemblyHint: componentRegistrationDefinition.Dll);
            var implementationType = TypeResolver.Resolve(componentRegistrationDefinition.Implementation, componentRegistrationDefinition.Dll, componentRegistryDefinition.PluginFolder);
            RegisterComponent(
                interfaceType,
                implementationType,
                componentRegistrationDefinition.ResolveKey ?? implementationType.FullName,
                componentRegistrationDefinition.Activation ?? ActivationType.Instance
            );
        }

        private void RegisterInternalMultipleComponentProxyRegistration(ComponentRegistryDefinition componentRegistryDefinition, ComponentRegistryDefinition.ProxyInterfaceRegistrationDefinition proxyInterfaceRegistrationDefinition) {
            var interfaceType = TypeResolver.Resolve(proxyInterfaceRegistrationDefinition.Interface);
            var implementationType = TypeResolver.Resolve(proxyInterfaceRegistrationDefinition.Proxy);
            RegisterProxyComponent(
                interfaceType,
                implementationType,
                implementationType.FullName
            );
        }

        private void RegisterInternal(Registration registration) {
            lock (_threadLock) {
                _registrations.Add(registration);
                _registrationsByInterfaceType.Add(registration.InterfaceType, registration);
                _registrationsByImplementationType.Add(registration.ImplementationType, registration);
                State++;
            }
        }

		private void DeregisterInternal(Registration registration) {
			lock (_threadLock) {
				_registrations.Remove(registration);
				_registrationsByInterfaceType.Remove(registration.InterfaceType, registration);
				_registrationsByImplementationType.Add(registration.ImplementationType, registration);
			}
		}

        #endregion

        #region Inner Classes

        public class Registration {

            public Registration(Type interfaceType, Type implementationType, string resolveKey, ActualActivationType activationType) {
                InterfaceType = interfaceType;
                ImplementationType = implementationType;
                ResolveKey = resolveKey;
                ActivationType = activationType;
            }

            public Type InterfaceType { get; set; }

            public Type ImplementationType { get; set; }

            public string ResolveKey { get; set; }

            public ActualActivationType ActivationType { get; set; }

            public static Registration From<TInterface>(TInterface instance, string name)
                where TInterface : class {
                return new Registration(typeof(TInterface), instance.GetType(), name, ActualActivationType.ExistingInstance);
            }

            public static Registration From<TInterface, TImplementation>(string name, ActivationType activationType) {
                return new Registration(typeof(TInterface), typeof(TImplementation), name, Convert(activationType));
            }

            public static Registration From(Type @interface, Type implementation, string name, ActivationType activationType) {
                return new Registration(@interface, implementation, name, Convert(activationType));
            }

            public static Registration FromProxy<TInterface, TImplementation>(string name) {
                return new Registration(typeof(TInterface), typeof(TImplementation), name, ActualActivationType.Proxy);
            }

            public static Registration FromProxy(Type @interface, Type implementation, string name) {
                return new Registration(@interface, implementation, name, ActualActivationType.Proxy);
            }

            private static ActualActivationType Convert(ActivationType activationType) {
                switch (activationType) {
                    case Application.ActivationType.Instance:
                        return ActualActivationType.NewInstance;
                    case Application.ActivationType.Singleton:
                        return ActualActivationType.Singleton;
                    case Application.ActivationType.PerRequest:
                        return ActualActivationType.PerRequest;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(activationType), activationType, null);
                }
            }
        }


        public enum ActualActivationType {
            ExistingInstance,
            NewInstance,
            Singleton,
            PerRequest,
            Proxy
        }

        #endregion

    }
}
