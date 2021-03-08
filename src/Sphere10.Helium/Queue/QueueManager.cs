using System;
using Sphere10.Framework;
using Sphere10.Framework.Collections;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public class QueueManager : IQueueManager
    {
        private readonly IBusConfiguration _busConfiguration;
        private readonly ILocalQueue _localQueue;

        public QueueManager(IBusConfiguration busConfiguration, ILocalQueue localQueue)
        {
            _busConfiguration = busConfiguration;
            _localQueue = localQueue;
        }
        
        public void FirstIn(string destination, IMessage message, string fileName)
        {

            //observable list//

            //Wrap this inside the localQueue//
            var txnFile = new TransactionalFileMappedBuffer(
                fileName, 
                _busConfiguration.PageSize, 
                _busConfiguration.InMemoryPages);
            
            var stream = new ExtendedMemoryStream(txnFile);

            var list = new FixedClusterMappedList<IMessage>(
                _busConfiguration.ClusterSize, 
                _busConfiguration.ListingClusterCount, 
                _busConfiguration.StorageClusterCount, 
                new MessageSerializer(), 
                stream);

            list.Add(message);
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
