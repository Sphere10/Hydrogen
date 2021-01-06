using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Services;

namespace VelocityNET.Presentation.Hydrogen.Loader.Services {


	public class ComponentFactory : IComponentFactory {
		private IServiceProvider ServiceProvider { get; }

		private static readonly BindingFlags _injectablePropertyBindingFlags
			= BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private readonly SynchronizedDictionary<Type, Action<IServiceProvider, IComponent>> _cachedInitializers;

		private readonly IComponentActivator _componentActivator;

		public ComponentFactory(IServiceProvider serviceProvider, IComponentActivator componentActivator) {
			_cachedInitializers = new SynchronizedDictionary<Type, Action<IServiceProvider, IComponent>>();
			ServiceProvider = serviceProvider;
			_componentActivator = componentActivator ?? throw new ArgumentNullException(nameof(componentActivator));
		}


		public IComponent Activate(Type componentType) {
			var component = _componentActivator.CreateInstance(componentType);
			if (component is null) 
				throw new InvalidOperationException($"The component activator returned a null value for a component of type {componentType.FullName}.");

			Inject(ServiceProvider, component);
			return component;
		}

		private void Inject(IServiceProvider serviceProvider, IComponent instance) {

			var instanceType = instance.GetType();
			if (!_cachedInitializers.TryGetValue(instanceType, out var initializer)) {
				initializer = CreateInitializer(instanceType);
				_cachedInitializers.Add(instanceType, initializer);
			}
			initializer(serviceProvider, instance);
		}

		private Action<IServiceProvider, IComponent> CreateInitializer(Type type) {

			// Do all the reflection up front
			var injectableProperties =
				type
					.GetProperties(_injectablePropertyBindingFlags, true)
					.Where(p => p.IsDefined(typeof(InjectAttribute)));

			var injectables = injectableProperties.Select(property =>
			(
				propertyName: property.Name,
				propertyType: property.PropertyType,
				setter: new PropertySetter(type, property)
			)).ToArray();

			return Initialize;

		
			void Initialize(IServiceProvider serviceProvider, IComponent component) {
				foreach (var (propertyName, propertyType, setter) in injectables) {
					var serviceInstance = serviceProvider.GetService(propertyType);
					if (serviceInstance == null) 
						throw new InvalidOperationException($"Cannot provide a value for property '{propertyName}' on type '{type.FullName}'. There is no registered service of type '{propertyType}'.");
					setter.SetValue(component, serviceInstance);
				}
			}
		}
	}

}