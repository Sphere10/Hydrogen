using System;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Timeout
{
    public class Timeout : ITimeout
    {
        string ITimeout.TimeoutMessageId { get; set; } = Config.Config.TimeoutMessageId;
        
        private readonly IQueueManager _queueManager;
        
        public Timeout(IQueueManager queueManager)
        {
            _queueManager = queueManager;
        }
        
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
