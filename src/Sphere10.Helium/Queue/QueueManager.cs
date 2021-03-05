using System;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public class QueueManager : IQueueManager
    {
        public void FirstIn(IMessage message)
        {
            throw new NotImplementedException();
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
