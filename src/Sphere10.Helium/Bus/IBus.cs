using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Bus
{
    public interface IBus : ISendOnlyBus
    {
        void Subscribe<TK>();

        void Unsubscribe<TK>();

        ICallback SendLocal<TK>(IMessage message);

        ICallback SendLocal<TK>(IMessage message, IMessageHeader messageHeader);

        ICallback RegisterTimeout(TimeSpan delay, IMessage message);

        ICallback RegisterTimeout(DateTime processAt, IMessage message);

        void Reply<TK>(Action<TK> messageConstructor);

        void Return<TK>(TK errorEnum);

    }
}
