using System;
using Sphere10.Framework;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	public class LocalQueueProcessor : ILocalQueueProcessor {
		private readonly BusConfiguration _endpointConfiguration;
		private readonly QueueConfigDto _queueConfigDto;

		private IHeliumQueue _localQueue;
		private IHeliumQueue _processingQueue;

		public LocalQueueProcessor(BusConfiguration endpointConfiguration, QueueConfigDto queueConfigDto) {
			_endpointConfiguration = endpointConfiguration;
			_queueConfigDto = queueConfigDto;

			_localQueue = SetupLocalQueue();
			_processingQueue = SetupProcessingQueue();
		}

		public void OnCommittedLocalQueue(object sender) {
			throw new NotImplementedException();
		}

		public void OnCommittedProcessingQueue(object sender) {
			throw new NotImplementedException();
		}

		public void MoveFirstMessageFromLocalToProcessing() {
			if (_localQueue.Count == 0)
				throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);

			txnScope.EnlistFile(_localQueue);
			txnScope.EnlistFile(_processingQueue);
			
			var localQueueMessage = _localQueue[^1];
				
			using (_processingQueue.EnterWriteScope()) {
				_processingQueue.Add(localQueueMessage);
			}

			_localQueue.RemoveAt(^1);
			txnScope.Commit();
		}

		public void InsertMessageInLocalQueue(IMessage message) {

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);

			txnScope.EnlistFile(_localQueue);

			using (_localQueue.EnterWriteScope()) {
				_localQueue.Add(message);
			}

			txnScope.Commit();
		}

		public IMessage TakeFirstMessageOutOfLocalQueue() {
			if (_localQueue.Count == 0)
				throw new InvalidOperationException("CRITICAL ERROR: LocalQueue is empty and should not be empty. Message missing cannot proceed.");

			using var txnScope = new FileTransactionScope(_queueConfigDto.TempDirPath, ScopeContextPolicy.None);

			txnScope.EnlistFile(_localQueue);

			var localQueueMessage = _localQueue[^1];

			_localQueue.RemoveAt(^1);

			txnScope.Commit();

			return localQueueMessage;
		}

		private IHeliumQueue SetupLocalQueue() {
			return _localQueue ??= new LocalQueue(_queueConfigDto);
		}

		private IHeliumQueue SetupProcessingQueue() {
			return _processingQueue ??= new ProcessingQueue(_queueConfigDto);
		}

	}
}
