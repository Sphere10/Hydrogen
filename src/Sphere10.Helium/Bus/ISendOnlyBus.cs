using System;
using System.Collections.Generic;

namespace Sphere10.Helium.Bus
{
    public interface ISendOnlyBus : IDisposable
    {
        void Publish<T>(T message);

        void Publish<T>();

        void Publish<T>(Action<T> messageConstructor);

        ICallback Send(object message);

        ICallback Send<T>(Action<T> messageConstructor);

        ICallback Send(string destination, object message);

        ICallback Send<T>(string destination, Action<T> messageConstructor);

        ICallback Send(string destination, string correlationId, object message);

        ICallback Send<T>(
          string destination,
          string correlationId,
          Action<T> messageConstructor);

        IDictionary<string, string> OutgoingHeaders { get; }
    }
}
