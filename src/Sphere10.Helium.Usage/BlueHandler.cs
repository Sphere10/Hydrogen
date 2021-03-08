using System;
using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Usage
{
    public class BlueHandler : IHandleMessage<BlueHandlerMessage>
    {
        public void Handle(BlueHandlerMessage message)
        {
            throw new NotImplementedException();
        }
    }

    public record BlueHandlerMessage : IMessage
    {
        public string Id { get; set; }
    }
}
