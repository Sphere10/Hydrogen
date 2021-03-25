using Sphere10.Framework;
using Sphere10.Framework.Collections;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue
{
    /// <summary>
    ///
    /// CRITICAL:
    /// 1) This is a local queue for a Helium Service.
    /// 2) Every Helium Service has it's own local queue.
    /// 3) This local queue is the Input into a Helium Service from other external or other internal Helium Services that sends messages to this Helium Service.
    /// 4) The local queue is also used for Send-Local messages.
    /// 
    /// </summary>

    public class LocalQueue : ILocalQueue
    {
        private readonly QueueConfigDto _queueConfigDto;
        private readonly TransactionalFileMappedBuffer _txnFile;
        private readonly ExtendedMemoryStream _stream;
        private readonly StreamMappedFixedClusteredList<IMessage> _list;

        public string FileName { get; set; }

        public LocalQueue(QueueConfigDto queueConfigDto)
        {
            _queueConfigDto = queueConfigDto;


            _txnFile = new TransactionalFileMappedBuffer(
                FileName,
                _queueConfigDto.PageSize,
                _queueConfigDto.InMemoryPages);

            _stream = new ExtendedMemoryStream(_txnFile);

            _list = new StreamMappedFixedClusteredList<IMessage>(
                _queueConfigDto.ClusterSize,
                _queueConfigDto.ListingClusterCount,
                new MessageSerializer(),
                _stream);
        }

        public void FirstIn(string destination, IMessage message)
        {
            _list.Add(message);
        }
    }
}
