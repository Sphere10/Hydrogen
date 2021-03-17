using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Saga
{
    public interface IStartSaga<in T> : IHandleMessage<T> where T: IMessage
    {
        /*hello*/
    }
}
