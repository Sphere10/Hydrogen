using System;
using Sphere10.Helium.MessageType;

namespace Sphere10.Helium.Bus
{
    public interface IBus : ISendOnlyBus, IDisposable
    {
        void Subscribe<TK>();

        void Unsubscribe<TK>();

        ICallback SendLocal<TK>(IMessage message);

        ICallback RegisterTimeout(TimeSpan delay, IMessage message);

        ICallback RegisterTimeout(TimeSpan delay, IMessage message, IMessageHeader messageHeader);

        ICallback RegisterTimeout(DateTime processAt, IMessage message);

        ICallback RegisterTimeout(DateTime processAt, IMessageHeader messageHeader);

        void Reply<TK>(Action<TK> messageConstructor);

        void Return<TK>(TK errorEnum);

        IMessageHeader GetCurrentMessageHeader { get; }
    }
}
