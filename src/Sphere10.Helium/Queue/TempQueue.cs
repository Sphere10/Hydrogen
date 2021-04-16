using System;
using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	public class TempQueue : TransactionalList<IMessage>, ITempQueue {


		public TempQueue(IObjectSerializer<IMessage> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int maxStorageBytes, int memoryCacheBytes, int maxItems, bool readOnly = false) : base(serializer, filename, uncommittedPageFileDir, fileID, maxStorageBytes, memoryCacheBytes, maxItems, readOnly) {
		}

		public TempQueue(IObjectSerializer<IMessage> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int maxStorageBytes, int memoryCacheBytes, int clusterSize, int maxItems, bool readOnly = false) : base(serializer, filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, maxStorageBytes, memoryCacheBytes, clusterSize, maxItems, readOnly) {
		}

		public TempQueue(string filename, string uncommittedPageFileDir, Guid fileID, int memoryCacheBytes, IObjectSerializer<IMessage> serializer, bool readOnly = false) : base(filename, uncommittedPageFileDir, fileID, memoryCacheBytes, serializer, readOnly) {
		}

		public TempQueue(string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int memoryCacheBytes, int clusterSize, IObjectSerializer<IMessage> serializer, bool readOnly = false) : base(filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, memoryCacheBytes, clusterSize, serializer, readOnly) {
		}

		public void AddMessageToQueue(IMessage message) {
			throw new NotImplementedException();
		}

		public void RemoveMessageFromQueue(IMessage message) {
			throw new NotImplementedException();
		}
	}
}
