using System;
using Sphere10.Framework;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	public class LocalQueueProcessor : ILocalQueueProcessor {
		private readonly BusConfiguration _endpointConfiguration;
		private readonly QueueConfigDto _queueConfigDto;
		private readonly IInstantiateHandler _instantiateHandler;

		public IHeliumQueue LocalQueue { get; set; }
		public IHeliumQueue ProcessingQueue { get; set; }

		public LocalQueueProcessor(BusConfiguration endpointConfiguration, QueueConfigDto queueConfigDto, IInstantiateHandler instantiateHandler) {
			_endpointConfiguration = endpointConfiguration;
			_queueConfigDto = queueConfigDto;
			_instantiateHandler = instantiateHandler;

			LocalQueue = SetupLocalQueue();
			ProcessingQueue = SetupProcessingQueue();
		}
		
		public void OnCommittedLocalQueue(object sender) {
			throw new NotImplementedException();
		}

		public void OnCommittedProcessingQueue(object sender) {
			throw new NotImplementedException();
		}

		public void MoveFirstMessageFromLocalToProcessing() {
			if (LocalQueue.Count == 0)
				throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);

			txnScope.EnlistFile(LocalQueue, false);
			txnScope.EnlistFile(ProcessingQueue, false);
			
			var localQueueMessage = LocalQueue[^1];
				
			using (ProcessingQueue.EnterWriteScope()) {
				ProcessingQueue.Add(localQueueMessage);
			}

			LocalQueue.RemoveAt(^1);
			txnScope.Commit();
		}

		public void InsertMessageInLocalQueue(IMessage message) {

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);

			txnScope.EnlistFile(LocalQueue, false);

			using (LocalQueue.EnterWriteScope()) {
				LocalQueue.Add(message);
			}

			txnScope.Commit();
		}

		public IMessage TakeFirstMessageOutOfLocalQueue() {
			if (LocalQueue.Count == 0)
				throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);

			txnScope.EnlistFile(LocalQueue, false);

			var localQueueMessage = LocalQueue[^1];

			LocalQueue.RemoveAt(^1);

			txnScope.Commit();

			return localQueueMessage;
		}

		private IHeliumQueue SetupLocalQueue() {
			return LocalQueue ??= new LocalQueue(_queueConfigDto);
		}

		private IHeliumQueue SetupProcessingQueue() {
			return ProcessingQueue ??= new ProcessingQueue(_queueConfigDto);
		}
	}
}