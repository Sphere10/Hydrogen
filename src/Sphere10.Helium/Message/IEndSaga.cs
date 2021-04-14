using Sphere10.Helium.Handler;

namespace Sphere10.Helium.Message {
	public interface IEndSaga<T> : IHandleMessage<T> where T : IMessage {
	}
}