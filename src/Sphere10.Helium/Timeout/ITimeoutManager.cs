using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Timeout
{
    public interface ITimeoutManager
    {
        void PutTimeoutMessageInQueue(IMessage message);

        IMessage GetTimeoutMessageFromQueue();

        void AddTimeout(TimeSpan delay, string messageId);

        void AddTimeout(DateTime processAt, string messageId);

        void RemoveTimeout(string messageId);
    }
}
