using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Timeout
{
    public interface ITimeoutManager
    {
        protected string TimeoutMessageId { get; set; }

        public void PutTimeoutMessageInQueue(IMessage message);

        public IMessage GetTimeoutMessageFromQueue();

        public void AddTimeout(TimeSpan delay, string messageId);
        
        public void AddTimeout(DateTime processAt, string messageId);

        public void RemoveTimeout(string messageId);
    }
}
