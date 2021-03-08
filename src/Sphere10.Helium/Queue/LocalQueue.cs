using Sphere10.Framework;
using Sphere10.Framework.Collections;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    public class LocalQueue : ILocalQueue
    {
        private readonly QueueConfigDto _queueConfigDto;
        private readonly TransactionalFileMappedBuffer _txnFile;
        private readonly ExtendedMemoryStream _stream;
        private readonly FixedClusterMappedList<IMessage> _list;

        public string FileName { get; set; }

        public LocalQueue(QueueConfigDto queueConfigDto)
        {
            _queueConfigDto = queueConfigDto;


            _txnFile = new TransactionalFileMappedBuffer(
                FileName,
                _queueConfigDto.PageSize,
                _queueConfigDto.InMemoryPages);

            _stream = new ExtendedMemoryStream(_txnFile);

            _list = new FixedClusterMappedList<IMessage>(
                _queueConfigDto.ClusterSize,
                _queueConfigDto.ListingClusterCount,
                _queueConfigDto.StorageClusterCount,
                new MessageSerializer(),
                _stream);
        }

        public void FirstIn(string destination, IMessage message)
        {
            _list.Add(message);
        }
    }
}
