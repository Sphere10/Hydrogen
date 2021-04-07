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

	public class LocalQueue : TransactionalList<IMessage>, ILocalQueue {
		private readonly QueueConfigDto _queueConfigDto;

		public LocalQueue(QueueConfigDto queueConfigDto)
			: base(
				new BinaryFormattedSerializer<IMessage>(), 
				queueConfigDto.FilePath,
				queueConfigDto.TempDirectoryPath,
				queueConfigDto.ID,
				queueConfigDto.TransactionalPageSize,
				queueConfigDto.MaxSizeBytes,
				queueConfigDto.AllocatedMemory,
				queueConfigDto.ClusterSize,
				queueConfigDto.MaxItems
			) {
			_queueConfigDto = queueConfigDto;
		}

		public void FirstIn(string destination, IMessage message) {
			this.Add(message);
		}
	}
}
