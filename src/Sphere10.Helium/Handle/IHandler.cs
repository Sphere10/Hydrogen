using Sphere10.Helium.Bus;

namespace Sphere10.Helium.Handle {
	public interface IHandler {
		public IBus Bus { get; }
	}
}