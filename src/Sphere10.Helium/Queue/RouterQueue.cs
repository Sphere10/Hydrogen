using System;
using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	public class RouterQueue : TransactionalList<IMessage>, IRouterQueue {


		public RouterQueue(IItemSerializer<IMessage> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int maxStorageBytes, int memoryCacheBytes, int maxItems, bool readOnly = false) : base(serializer, filename, uncommittedPageFileDir, fileID, maxStorageBytes, memoryCacheBytes, maxItems, readOnly) {
		}

		public RouterQueue(IItemSerializer<IMessage> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int maxStorageBytes, int memoryCacheBytes, int clusterSize, int maxItems, bool readOnly = false) : base(serializer, filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, maxStorageBytes, memoryCacheBytes, clusterSize, maxItems, readOnly) {
		}

		public RouterQueue(string filename, string uncommittedPageFileDir, Guid fileID, int memoryCacheBytes, IItemSerializer<IMessage> serializer, bool readOnly = false) : base(filename, uncommittedPageFileDir, fileID, memoryCacheBytes, serializer, readOnly) {
		}

		public RouterQueue(string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int memoryCacheBytes, int clusterSize, IItemSerializer<IMessage> serializer, bool readOnly = false) : base(filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, memoryCacheBytes, clusterSize, serializer, readOnly) {
		}

		public void AddMessage(IMessage message) {
			throw new NotImplementedException();
		}

		public bool DeleteMessage(IMessage message) {
			throw new NotImplementedException();
		}

		public IMessage RetrieveMessage() {
			throw new NotImplementedException();
		}
	}
}
