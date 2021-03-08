using Sphere10.Helium.Message;

namespace Sphere10.Helium.Handler
{
    public interface IHandleMessage<in T> where T : IMessage
    {
        public void Handle(T message);
    }
}