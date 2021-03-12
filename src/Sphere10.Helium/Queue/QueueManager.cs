using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public class QueueManager : IQueueManager
    {
        private readonly ILocalQueue _localQueue;

        public QueueManager(IEndpointConfiguration endpointConfiguration, ILocalQueue localQueue)
        {
            _localQueue = localQueue;
            _localQueue.FileName = endpointConfiguration.FileName;
        }

        public void FirstIn(string destination, IMessage message)
        {
            _localQueue.FirstIn(destination, message);
        }

        public void LastOut(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void TakeThisMessageOffQueue(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void PersistQueue()
        {
            throw new NotImplementedException();
        }
    }
}
