using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Helium.Handle;

namespace Sphere10.Helium.Framework {
	public abstract class PluginModuleConfiguration : ModuleConfigurationBase {
		// track registrations here
		private readonly IList<Type> _registeredTypes;


		public PluginModuleConfiguration() {
			_registeredTypes = new List<Type>();
		}

		/// <summary>
		/// Registers all the plugin handlers and sagas automatically.
		/// </summary>
		/// <param name="registry"></param>
		public override void RegisterComponents(ComponentRegistry registry) {
			base.RegisterComponents(registry);

			var pluginAssembly = this.GetType().Assembly;
			//inspect the assembly and register all handlers correctly into component registry
			var possibleTypes = pluginAssembly.GetTypes().Where(t => !t.IsAnonymousType() && !t.IsInterface && !t.IsAbstract && !t.IsGenericType && t.IsClass).ToArray();
			var handlers = possibleTypes.Where(t => t.IsDescendantOfType(typeof(IHandler))).ToArray();
			var sagas = handlers.Where(t => t.IsDescendantOfType(typeof(Saga.Saga))).ToArray();
			var standAloneHandlers = handlers.Except(sagas);

			// Register handlers
			foreach (var handler in standAloneHandlers) {
				// figure out all the messages this handler handles
				var iHandleMessageInterfaces = handler.GetAncestorTypes().Where(t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(IHandleMessage<>)).ToArray();
				if (iHandleMessageInterfaces.Length != 1)
					throw new HeliumException($"Invalid Handler: Handler '{handler.Name}' handled more than one message");
				registry.RegisterComponent(iHandleMessageInterfaces[0], handler, options: ActivationType.Instance);
				_registeredTypes.Add(handler);
			}

			// Register Sagas
			foreach (var saga in sagas) {
				// figure out all the messages this handler handles
				var iHandleMessageInterfaces = saga.GetAncestorTypes().Where(t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(IHandleMessage<>)).ToArray();
				foreach (var iHandleMessageT in iHandleMessageInterfaces) {
					registry.RegisterComponent(iHandleMessageT, saga, options: ActivationType.Instance);
				}
				_registeredTypes.Add(saga);
			}
		}

		public override void DeregisterComponents(ComponentRegistry registry) {
			base.DeregisterComponents(registry);
			foreach(var type in _registeredTypes)
				registry.DeregisterComponent(type);
		}


	}
}