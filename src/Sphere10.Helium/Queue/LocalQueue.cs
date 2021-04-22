using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	/// <summary>
	/// CRITICAL:
	/// 1) This is a FIFO queue.
	/// 2) This is a local queue for a Helium Service.
	/// 3) Every Helium Service has it's own local queue.
	/// 4) ALL input into a Helium Service goes into this queue.
	/// 5) The local queue is also used for Send-Local messages.
	/// </summary>

	public class LocalQueue : TransactionalList<IMessage>, ILocalQueue {
		
		private readonly QueueConfigDto _queueConfigDto;

		public LocalQueue(QueueConfigDto queueConfigDto)
			: base(
				new BinaryFormattedSerializer<IMessage>(),
				queueConfigDto.LocalQueueFilePath,
				queueConfigDto.TempQueueFilePath,
				queueConfigDto.FileId,
				queueConfigDto.TransactionalPageSizeBytes,
				queueConfigDto.MaxStorageSizeBytes,
				queueConfigDto.AllocatedMemory,
				queueConfigDto.ClusterSize,
				queueConfigDto.MaxItems
			) {
			_queueConfigDto = queueConfigDto;
		}

		public void AddMessageToQueue(IMessage message) {
			Add(message);
			Commit();
		}

		public void RemoveMessageFromQueue(IMessage message) {
			Remove(message);
			Commit();
		}
	}
}
