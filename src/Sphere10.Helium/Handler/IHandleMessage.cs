
using Sphere10.Helium.MessageType;

namespace Sphere10.Helium.Handler
{
    public interface IHandleMessage<T> where T : IMessage
    {
        public void Handle(T message);
    }
}