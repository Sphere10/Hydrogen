using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Saga
{
    public interface IEndSaga<T> : IHandleMessage<T> where T : IMessage
    {
    }
}