using Sphere10.Helium.Message;

namespace Sphere10.Helium.Handle {

	public interface IHandleMessage<in T> : IHandler where T : IMessage {
		public void Handle(T message);
	}
}