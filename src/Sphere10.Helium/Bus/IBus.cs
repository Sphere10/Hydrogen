using System;

namespace Sphere10.Helium.Bus
{
    public interface IBus : ISendOnlyBus, IDisposable
    {
        void Subscribe<T>();

        void Unsubscribe<T>();

        ICallback SendLocal<T>(Action<T> messageConstructor);

        ICallback Defer(TimeSpan delay, object message);

        ICallback Defer(DateTime processAt, object message);

        void Reply<T>(Action<T> messageConstructor);

        void Return<T>(T errorEnum);

        void HandleCurrentMessageLater();

        void ForwardCurrentMessageTo(string destination);

        IMessageContext CurrentMessageContext { get; }

        void SetMessageHeader(object message, string headerName, string headerValue);

    }
}
