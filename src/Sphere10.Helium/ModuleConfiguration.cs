using Sphere10.Framework.Application;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Framework;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Retry;
using Sphere10.Helium.Router;

namespace Sphere10.Helium {
	public class ModuleConfiguration : ModuleConfigurationBase {
		public override int Priority => int.MinValue;

		public override void RegisterComponents(ComponentRegistry registry) {
			#region Queues
			if (!registry.HasImplementationFor<IHeliumQueue>()) {
				registry.RegisterComponent<IHeliumQueue, LocalQueue>("LocalQueue", ActivationType.Singleton);
				registry.RegisterComponent<IHeliumQueue, PrivateQueue>("PrivateQueue", ActivationType.Singleton);
				registry.RegisterComponent<IHeliumQueue, RouterQueue>("RouterQueue", ActivationType.Singleton);
			}
			#endregion
			
			#region LocalQueue Processing
			if (!registry.HasImplementationFor<ILocalQueueInput>())
				registry.RegisterComponentFactory<ILocalQueueInput>(
					container => new LocalQueueInput(
						container.Resolve<IHeliumQueue>("LocalQueue")));

			if (!registry.HasImplementationFor<ILocalQueueProcessor>())
				registry.RegisterComponentFactory<ILocalQueueProcessor>(
					container => new LocalQueueProcessor(
						container.Resolve<IInstantiateHandler>("InstantiateHandler"),
						container.Resolve<IHeliumQueue>("LocalQueue"),
						container.Resolve<ILocalQueueInput>("LocalQueueInput")));
			#endregion

			if (!registry.HasImplementationFor<IRouter>())
				registry.RegisterComponentFactory<IRouter>(
					container => new Router.Router(
						container.Resolve<ILocalQueueInput>("LocalQueueInput")));

			if (!registry.HasImplementationFor<IRetryManager>())
				registry.RegisterComponent<IRetryManager, RetryManager>(ActivationType.Singleton);

			if (!registry.HasImplementationFor<IInstantiateHandler>())
				registry.RegisterComponent<IInstantiateHandler, InstantiateHandler>(ActivationType.Singleton);

			registry.RegisterInitializationTask<SetupFoldersInitTask>(); //Setup working folder is here
		}
	}
}