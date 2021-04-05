namespace Sphere10.Helium.Message
{
    public interface IHandleTimeout<in T> where T: IMessage
    {
        void Timeout(T state);
    }
}
