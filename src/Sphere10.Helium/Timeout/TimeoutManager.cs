using System;
using Sphere10.Helium.Configuration;
using Sphere10.Helium.Message;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Timeout
{
    public class TimeoutManager : ITimeoutManager
    {
        string ITimeoutManager.TimeoutMessageId { get; set; } = Global.TimeoutMessageId;
        
        private readonly ILocalQueueProcessor _queueManager;
        
        public TimeoutManager(ILocalQueueProcessor queueManager)
        {
            _queueManager = queueManager;
        }
        
		public void PutTimeoutMessageInQueue(ITimeout message) {
			throw new NotImplementedException();
		}

		ITimeout ITimeoutManager.GetTimeoutMessageFromQueue() {
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
