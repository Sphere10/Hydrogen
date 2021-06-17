using Sphere10.Framework.Application;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Retry;
using Sphere10.Helium.Route;

namespace Sphere10.Helium {
	public class ModuleConfiguration : ModuleConfigurationBase {
		public override int Priority => int.MinValue;

		public override void RegisterComponents(ComponentRegistry registry) {

			if (!registry.HasImplementationFor<IHeliumQueue>())
				registry.RegisterComponent<IHeliumQueue, LocalQueue>(ActivationType.Singleton);

			if (!registry.HasImplementationFor<IHeliumQueue>())
				registry.RegisterComponent<IHeliumQueue, RouterQueue>(ActivationType.Singleton);

			if (!registry.HasImplementationFor<IRetryManager>())
				registry.RegisterComponent<IRetryManager, RetryManager>(ActivationType.Singleton);

			if (!registry.HasImplementationFor<IRouter>())
				registry.RegisterComponent<IRouter, Route.Router>(ActivationType.Singleton);
		}
	}
}
