using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public class QueueManager : IQueueManager
    {
        public void PutMessageInQueue(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void TakeMessageFromQueue(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void TakeThisMessageFromQueue(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void PersistQueue()
        {
            throw new NotImplementedException();
        }
    }
}
