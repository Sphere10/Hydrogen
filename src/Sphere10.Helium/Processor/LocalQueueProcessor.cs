using System;
using System.IO;
using Sphere10.Framework.Application;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	/// <summary>
	/// IMPORTANT: This class deals exclusively with ALL Reading/Deleting/Re-Inserting of the LocalQueue.
	/// </summary>
	public class LocalQueueProcessor : ILocalQueueProcessor {
		private readonly IInstantiateHandler _instantiateHandler;
		private readonly IHeliumQueue _localQueue; //SHALL be used soon.
		private readonly ILocalQueueInput _localQueueInput;

		public LocalQueueProcessor(IInstantiateHandler instantiateHandler, 
		                           IHeliumQueue localQueue, 
		                           ILocalQueueInput localQueueInput) {

			_instantiateHandler = instantiateHandler;
			_localQueue = localQueue;
			_localQueueInput = localQueueInput;

			var localQueueSettings = GlobalSettings.Get<LocalQueueSettings>();

			if (!Directory.Exists(localQueueSettings.TempDirPath))
				Directory.CreateDirectory(localQueueSettings.TempDirPath);

			if (File.Exists(localQueueSettings.Path))
				File.Delete(localQueueSettings.Path);

			if (_localQueue.RequiresLoad)
				_localQueue.Load();
		}
		
		public void OnCommittedLocalQueue(object sender) {
			throw new NotImplementedException("TODO:Jake =>1");
		}

		public void OnCommittedProcessingQueue(object sender) {
			throw new NotImplementedException("TODO:Jake =>2");
		}

		public void MoveFirstMessageFromLocalToProcessing() {
			//if (LocalQueue.Count == 0)
			//	throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

			////using var txnScope = new FileTransactionScope(LocalQueueSettings.TempDirPath, ScopeContextPolicy.None);

			//txnScope.EnlistFile(LocalQueue, false);
			//txnScope.EnlistFile(ProcessingQueue, false);
			
			//var localQueueMessage = LocalQueue[^1];
				
			//using (ProcessingQueue.EnterWriteScope()) {
			//	ProcessingQueue.Add(localQueueMessage);
			//}

			//LocalQueue.RemoveAt(^1);
			//txnScope.Commit();
		}

		//public void InsertMessageInLocalQueue(IMessage message) {

			//using var txnScope = new FileTransactionScope(LocalQueueSettings.TempDirPath, ScopeContextPolicy.None);

			//txnScope.EnlistFile(LocalQueue, false);

			//using (LocalQueue.EnterWriteScope()) {
			//	LocalQueue.Add(message);
			//}

			//txnScope.Commit();
		//}

		public IMessage TakeFirstMessageOutOfLocalQueue() {
			//if (LocalQueue.Count == 0)
			//	throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

			//using var txnScope = new FileTransactionScope(LocalQueueSettings.TempDirPath, ScopeContextPolicy.None);

			//txnScope.EnlistFile(LocalQueue, false);

			//var localQueueMessage = LocalQueue[^1];

			//LocalQueue.RemoveAt(^1);

			//txnScope.Commit();

			//return localQueueMessage;
			return null;
		}
	}
}