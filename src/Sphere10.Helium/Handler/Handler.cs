using Sphere10.Helium.Bus;

namespace Sphere10.Helium.Handler {
	public class Handler {
		public  IBus Bus;

		public Handler(IBus bus) {
			Bus = bus;
		}
	}
}