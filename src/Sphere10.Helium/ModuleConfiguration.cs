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
			#region Config DTO
			if (!registry.HasImplementationFor<BusConfigurationDto>())
				registry.RegisterComponentInstance(new BusConfigurationDto(), "BusConfigurationDto");

			if (!registry.HasImplementationFor<LocalQueueConfigDto>())
				registry.RegisterComponentInstance<LocalQueueConfigDto>(new LocalQueueConfigDto(), "LocalQueueConfigDto");

			if (!registry.HasImplementationFor<PrivateQueueConfigDto>())
				registry.RegisterComponentInstance<PrivateQueueConfigDto>(new PrivateQueueConfigDto(), "PrivateQueueConfigDto");

			if (!registry.HasImplementationFor<RouterConfigDto>())
				registry.RegisterComponentInstance<RouterConfigDto>(new RouterConfigDto(), "RouterConfigDto");
			#endregion

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
						container.Resolve<IHeliumQueue>("LocalQueue"), 
						container.Resolve<LocalQueueConfigDto>("LocalQueueConfigDto")));

			if (!registry.HasImplementationFor<ILocalQueueProcessor>())
				registry.RegisterComponentFactory<ILocalQueueProcessor>(
					container => new LocalQueueProcessor(
						container.Resolve<BusConfigurationDto>("BusConfigurationDto"),
						container.Resolve<LocalQueueConfigDto>("LocalQueueConfigDto"),
						container.Resolve<IInstantiateHandler>("InstantiateHandler"),
						container.Resolve<IHeliumQueue>("LocalQueue"),
						container.Resolve<ILocalQueueInput>("LocalQueueInput"))); 
			#endregion
			
			if (!registry.HasImplementationFor<IRetryManager>())
				registry.RegisterComponent<IRetryManager, RetryManager>(ActivationType.Singleton);

			if (!registry.HasImplementationFor<IRouter>())
				registry.RegisterComponentFactory<IRouter>(
					container => new Router.Router(
						container.Resolve<ILocalQueueInput>("LocalQueueInput")));

			if (!registry.HasImplementationFor<IInstantiateHandler>())
				registry.RegisterComponent<IInstantiateHandler, InstantiateHandler>(ActivationType.Singleton);
		}
	}
}