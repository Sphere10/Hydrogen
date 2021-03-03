
namespace Sphere10.Helium.Handler
{
    public interface IHandleMessage<T>
    {
        public void Handle(T message);
    }
}