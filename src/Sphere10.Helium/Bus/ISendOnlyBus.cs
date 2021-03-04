using System;
using Sphere10.Helium.MessageType;

namespace Sphere10.Helium.Bus
{
    public interface ISendOnlyBus : IDisposable
    {
        void SendAndForget(string destination, IMessage message);

        void SendAndForget(string destination, IMessage message, IMessageHeader messageHeader);

        ICallback SendAndResponse(string destination, IMessage message);

        ICallback SendAndResponse(string destination, IMessage message, IMessageHeader messageHeader);
    }
}
