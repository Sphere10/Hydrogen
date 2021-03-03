namespace Sphere10.Helium.Saga
{
    public interface IHandleTimeout<T>
    {
        void Timeout(T state);
    }
}
