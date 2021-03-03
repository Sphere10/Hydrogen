using Sphere10.Helium.Handler;

namespace Sphere10.Helium.Saga
{
    public interface IStartSaga<T> : IHandleMessage<T>
    {
    }
}
