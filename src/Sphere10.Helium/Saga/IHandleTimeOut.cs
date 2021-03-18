using Sphere10.Helium.Message;

namespace Sphere10.Helium.Saga
{
    public interface IHandleTimeout<in T> where T: IMessage
    {
        void Timeout(T state);
    }
}
