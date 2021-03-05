using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Timeout
{
    public class TimeoutManager : ITimeoutManager
    {
        public void PutTimeoutMessageInQueue(IMessage message)
        {
            throw new NotImplementedException();
        }

        public IMessage GetTimeoutMessageFromQueue()
        {
            throw new NotImplementedException();
        }

        public void AddTimeout(TimeSpan delay, string messageId)
        {
            throw new NotImplementedException();
        }

        public void AddTimeout(DateTime processAt, string messageId)
        {
            throw new NotImplementedException();
        }

        public void RemoveTimeout(string messageId)
        {
            throw new NotImplementedException();
        }
    }
}
