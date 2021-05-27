using System;
using System.IO;
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

	public class LocalQueue : TransactionalList<IMessage>, IHeliumQueue {
		
		public event EventHandler MessageCommitted;

		public LocalQueue(QueueConfigDto queueConfigDto)
			: base(
				new BinaryFormattedSerializer<IMessage>(),
				queueConfigDto.Path,
				queueConfigDto.TempDirPath,
				queueConfigDto.FileId,
				queueConfigDto.TransactionalPageSizeBytes,
				queueConfigDto.MaxStorageSizeBytes,
				queueConfigDto.AllocatedMemory,
				queueConfigDto.ClusterSize,
				queueConfigDto.MaxItems
			) {
		}

		protected override void OnCommitted() {
			var handler = MessageCommitted;

			base.OnCommitted();
		}

		public void AddMessage(IMessage message) {
			Add(message);
		}

		public bool DeleteMessage(IMessage message) {
			var result =Remove(message);
			return result;
		}
		
		public IMessage ReadMessage() {
			var message = Read(0);
			return message;
		}

		public IMessage RemoveMessage() {
			var message = this[^1];
			this.RemoveAt(^1);
			return message;
		}
	}
}
