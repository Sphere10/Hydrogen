using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Helium.Framework;
using Sphere10.Helium.HeliumNode;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Retry;
using Sphere10.Helium.Router;

namespace Sphere10.Helium {
	public class ModuleConfiguration : ModuleConfigurationBase {
		public override int Priority => int.MinValue;

		public override void RegisterComponents(ComponentRegistry registry) {
			if (!registry.HasImplementationFor<ILogger>())
				registry.RegisterComponent<ILogger, ConsoleLogger>("ConsoleLogger", ActivationType.Singleton);

			#region Queues
			if (!registry.HasImplementationFor<IHeliumQueue>()) {
				registry.RegisterComponent<IHeliumQueue, LocalQueue>("LocalQueue", ActivationType.Singleton);
				registry.RegisterComponent<IHeliumQueue, PrivateQueue>("PrivateQueue", ActivationType.Singleton);
				registry.RegisterComponent<IHeliumQueue, RouterQueue>("RouterQueue", ActivationType.Singleton);
			}
			#endregion

			if (!registry.HasImplementationFor<IInstantiateHandler>())
				registry.RegisterComponent<IInstantiateHandler, InstantiateHandler>(ActivationType.Singleton);

			#region LocalQueue Processing
			if (!registry.HasImplementationFor<ILocalQueueInputProcessor>())
				registry.RegisterComponentFactory<ILocalQueueInputProcessor>(
					container => new LocalQueueInputProcessor(
						container.Resolve<IHeliumQueue>("LocalQueue"),
						container.Resolve<ILogger>("ConsoleLogger")));

			if (!registry.HasImplementationFor<ILocalQueueOutputProcessor>())
				registry.RegisterComponentFactory<ILocalQueueOutputProcessor>(
					container => new LocalQueueOutputProcessor(
						container.Resolve<IInstantiateHandler>(),
						container.Resolve<IHeliumQueue>("LocalQueue"),
						container.Resolve<ILocalQueueInputProcessor>(),
						container.Resolve<ILogger>("ConsoleLogger")));

			if (!registry.HasImplementationFor<IPrivateQueueInputProcessor>())
				registry.RegisterComponentFactory<IPrivateQueueInputProcessor>(
					container => new PrivateQueueInputProcessor(
						container.Resolve<ILogger>("ConsoleLogger")));
			#endregion

			if (!registry.HasImplementationFor<IRouter>())
				registry.RegisterComponentFactory<IRouter>(
					container => new Router.Router(
						container.Resolve<ILocalQueueInputProcessor>()));
			
			if (!registry.HasImplementationFor<IRetryManager>())
				registry.RegisterComponent<IRetryManager, RetryManager>(ActivationType.Singleton);

			if (!registry.HasImplementationFor<IConfigureHeliumNode>())
				registry.RegisterComponentFactory<IConfigureHeliumNode>(
					container => new ConfigureHeliumNode(
						container.Resolve<ILocalQueueInputProcessor>(),
						container.Resolve<IPrivateQueueInputProcessor>()));

			registry.RegisterInitializationTask<SetupFoldersInitTask>(); //Setup working folder is here
		}
	}
}