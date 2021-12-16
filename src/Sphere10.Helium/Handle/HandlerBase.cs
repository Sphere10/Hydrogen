using Sphere10.Helium.Bus;

namespace Sphere10.Helium.Handle {
	public abstract class HandlerBase : IHandler {

		protected HandlerBase(IBus bus) {
			Bus = bus;
		}

		public IBus Bus { get; }
	}
}