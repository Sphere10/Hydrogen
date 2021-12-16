using System;
using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	public class PrivateQueue : TransactionalList<IMessage>, IHeliumQueue {
		public event EventHandler MessageCommitted;

		public PrivateQueue(PrivateQueueSettings privateQueueSettings)
			: base(
				new BinaryFormattedSerializer<IMessage>(),
				privateQueueSettings.Path,
				privateQueueSettings.TempDirPath,
				privateQueueSettings.FileId,
				privateQueueSettings.TransactionalPageSizeBytes,
				privateQueueSettings.MaxStorageSizeBytes,
				privateQueueSettings.AllocatedMemory,
				privateQueueSettings.ClusterSize,
				privateQueueSettings.MaxItems
			) {
		}

		protected override void OnCommitted() {
			var unused = MessageCommitted;
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