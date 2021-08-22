using System;
using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	/// <summary>
	/// CRITICAL: The LocalQueue is a FIFO queue for the Helium Framework.
	/// ALL input messages into Helium must go into this LocalQueue.
	/// </summary>
	public class LocalQueue : TransactionalList<IMessage>, IHeliumQueue {

		public event EventHandler MessageCommitted;

		public LocalQueue(LocalQueueSettings localQueueSettings)
			: base(
				new BinaryFormattedSerializer<IMessage>(),
				localQueueSettings.Path,
				localQueueSettings.TempDirPath,
				localQueueSettings.FileId,
				localQueueSettings.TransactionalPageSizeBytes,
				localQueueSettings.MaxStorageSizeBytes,
				localQueueSettings.AllocatedMemory,
				localQueueSettings.ClusterSize,
				localQueueSettings.MaxItems
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
			var result = Remove(message);
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
